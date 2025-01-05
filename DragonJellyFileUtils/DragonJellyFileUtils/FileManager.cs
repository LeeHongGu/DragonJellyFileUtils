using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJellyFileUtils
{
    public static class FileManager
    {
        public static async Task CopyFileAsync(string sourceFilePath, string destinationFilePath, bool overwrite = false, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(sourceFilePath))
                throw new ArgumentException("Source file path is required.", nameof(sourceFilePath));

            if (!File.Exists(sourceFilePath))
                throw new FileNotFoundException("Source file not found.", sourceFilePath);

            if (string.IsNullOrWhiteSpace(destinationFilePath))
                throw new ArgumentException("Destination file path is required.", nameof(destinationFilePath));

            if (!overwrite && File.Exists(destinationFilePath))
                throw new IOException("Destination file already exists.");

            var destinationDirectory = Path.GetDirectoryName(destinationFilePath);
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }

             using FileStream sourceStream = new(sourceFilePath, FileMode.Open, FileAccess.Read, FileShare.Read, 4096, true);
            using FileStream destinationStream = new(destinationFilePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            await sourceStream.CopyToAsync(destinationStream, cancellationToken);
        }

        public static async Task MoveFileAsync(string sourceFilePath, string destinationFilePath, CancellationToken cancellationToken = default)
        {
            await CopyFileAsync(sourceFilePath, destinationFilePath, overwrite: true, cancellationToken);
            await SecureDeleteFileAsync(sourceFilePath);
        }

        public static async Task CreateEmptyFileAsync(string filePath)
        {
            if (string.IsNullOrWhiteSpace(filePath))
                throw new ArgumentException("File path is required.", nameof(filePath));

            if (File.Exists(filePath))
                throw new IOException("File already exists.");

            using FileStream fileStream = new(filePath, FileMode.Create, FileAccess.Write, FileShare.None, 4096, true);
            await fileStream.FlushAsync();
        }

        public static async Task SecureDeleteFileAsync(string filePath)
        {
            if (!File.Exists(filePath))
                return;

            var fileSize = new FileInfo(filePath).Length;

            using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Write, FileShare.None, bufferSize: 4096, useAsync: true))
            {
                var buffer = new byte[4096];
                var random = new Random();

                long bytesWritten = 0;

                while (bytesWritten < fileSize)
                {
                    random.NextBytes(buffer);
                    var bytesToWrite = (fileSize - bytesWritten) > buffer.Length ? buffer.Length : (int)(fileSize - bytesWritten);
                    await fileStream.WriteAsync(buffer.AsMemory(0, bytesToWrite));
                    bytesWritten += bytesToWrite;
                }

                await fileStream.FlushAsync();
            }

            File.Delete(filePath);
        }
    }

}
