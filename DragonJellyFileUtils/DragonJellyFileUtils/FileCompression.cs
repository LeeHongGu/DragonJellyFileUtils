using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJellyFileUtils
{
    public static class FileCompression
    {
        public static async Task CompressAsync(string? sourcePath, string? destinationPath = null, CustomCompessionLevel compressionLevel = CustomCompessionLevel.Optimal, IProgress<ProgressReport>? progress = null, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("Source path is required.", nameof(sourcePath));

            if (string.IsNullOrWhiteSpace(destinationPath))
                destinationPath = Path.Combine(Path.GetDirectoryName(sourcePath) ?? string.Empty, Path.GetFileNameWithoutExtension(sourcePath) + ".zip");

            if (!destinationPath.EndsWith(".zip", StringComparison.OrdinalIgnoreCase))
                destinationPath = Path.ChangeExtension(destinationPath, ".zip");

            if (!overwrite && File.Exists(destinationPath))
                throw new IOException("Destination file already exists.");

            var destinationDirectory = Path.GetDirectoryName(destinationPath);
            if (!string.IsNullOrWhiteSpace(destinationDirectory) && !Directory.Exists(destinationDirectory))
                Directory.CreateDirectory(destinationDirectory);

            var level = compressionLevel switch
            {
                CustomCompessionLevel.Fastest => System.IO.Compression.CompressionLevel.Fastest,
                CustomCompessionLevel.Optimal => System.IO.Compression.CompressionLevel.Optimal,
                CustomCompessionLevel.NoCompression => System.IO.Compression.CompressionLevel.NoCompression,
                _ => System.IO.Compression.CompressionLevel.Optimal,
            };

            await Task.Run(() =>
            {
                long totalBytes = 0;
                long bytesProcessed = 0;

                using FileStream zipToOpen = new(destinationPath, FileMode.Create);
                using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Create);

                if (File.GetAttributes(sourcePath).HasFlag(FileAttributes.Directory))
                {
                    var directoryInfo = new DirectoryInfo(sourcePath);
                    totalBytes = GetDirectorySize(directoryInfo);

                    foreach (var file in directoryInfo.GetFiles("*", SearchOption.AllDirectories))
                    {
                        cancellationToken.ThrowIfCancellationRequested();
                        string entryName = Path.GetRelativePath(directoryInfo.FullName, file.FullName);
                        var entry = archive.CreateEntry(entryName, level);
                        using FileStream fs = file.OpenRead();
                        using Stream entryStream = entry.Open();
                        fs.CopyTo(entryStream, bufferSize: 81920);
                        bytesProcessed += file.Length;
                        progress?.Report(new ProgressReport { TotalBytes = totalBytes, BytesProcessed = bytesProcessed });
                    }
                }
                else
                {
                    totalBytes = GetFileSize(sourcePath);
                    var fileInfo = new FileInfo(sourcePath);
                    var entry = archive.CreateEntry(fileInfo.Name, level);
                    using FileStream fs = fileInfo.OpenRead();
                    using Stream entryStream = entry.Open();
                    fs.CopyTo(entryStream, bufferSize: 81920);
                    bytesProcessed += fileInfo.Length;
                    progress?.Report(new ProgressReport { TotalBytes = totalBytes, BytesProcessed = bytesProcessed });
                }
            }, cancellationToken);
        }

        public static async Task DecompressAsync(string sourcePath, string destinationPath, IProgress<ProgressReport> progress = null, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
                throw new ArgumentException("Source path is required.", nameof(sourcePath));

            if (string.IsNullOrWhiteSpace(destinationPath))
                throw new ArgumentException("Destination path is required.", nameof(destinationPath));

            if (!Directory.Exists(destinationPath))
                Directory.CreateDirectory(destinationPath);

            await Task.Run(() =>
            {
                long totalBytes = 0;
                long bytesProcessed = 0;

                using FileStream zipToOpen = new(sourcePath, FileMode.Open);
                using ZipArchive archive = new(zipToOpen, ZipArchiveMode.Read);

                foreach (var entry in archive.Entries)
                {
                    totalBytes += entry.Length;
                }

                foreach (var entry in archive.Entries)
                {
                    cancellationToken.ThrowIfCancellationRequested();
                    string entryPath = Path.GetFullPath(Path.Combine(destinationPath, entry.FullName));

                    if (!entryPath.StartsWith(destinationPath, StringComparison.Ordinal))
                        throw new IOException("Entry is outside the target directory.");

                    if (entry.FullName.EndsWith(Path.DirectorySeparatorChar) || entry.FullName.EndsWith(Path.AltDirectorySeparatorChar))
                    {
                        Directory.CreateDirectory(entryPath);
                    }
                    else
                    {
                        Directory.CreateDirectory(Path.GetDirectoryName(entryPath));
                        if (File.Exists(entryPath) && !overwrite)
                            throw new IOException($"File '{entryPath}' already exists.");

                        using Stream entryStream = entry.Open();
                        using FileStream fs = new(entryPath, FileMode.Create);
                        entryStream.CopyTo(fs, bufferSize: 81920);
                        bytesProcessed += entry.Length;
                        progress?.Report(new ProgressReport { TotalBytes = totalBytes, BytesProcessed = bytesProcessed });
                    }
                }
            }, cancellationToken);
        }

        private static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            ArgumentNullException.ThrowIfNull(directoryInfo);

            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException("Directory not found.");
            }

            long size = 0;

            foreach (FileInfo file in directoryInfo.GetFiles())
            {
                size += file.Length;
            }

            foreach (DirectoryInfo dir in directoryInfo.GetDirectories())
            {
                size += GetDirectorySize(dir);
            }

            return size;
        }

        private static long GetFileSize(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
            {
                throw new ArgumentException("File path is required.", nameof(filePath));
            }

            if (!File.Exists(filePath))
            {
                throw new FileNotFoundException("File not found.", filePath);
            }

            return new FileInfo(filePath).Length;
        }
    }
}
