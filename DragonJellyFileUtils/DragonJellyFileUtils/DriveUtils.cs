using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DragonJellyFileUtils
{
    public static class DriveUtils
    {
        public static (bool CanMove, long AvailableSpace, long RequiredSpace) CanMoveItem(string sourcePath, string destinationDrive)
        {
            if (string.IsNullOrWhiteSpace(sourcePath))
            {
                throw new ArgumentException("Source folder path is required.", nameof(sourcePath));
            }

            if (string.IsNullOrWhiteSpace(destinationDrive))
            {
                throw new ArgumentException("Destination drive path is required.", nameof(destinationDrive));
            }

            if (!destinationDrive.EndsWith(Path.DirectorySeparatorChar.ToString()))
            {
                destinationDrive += Path.DirectorySeparatorChar;
            }

            long itemSize = File.GetAttributes(sourcePath).HasFlag(FileAttributes.Directory)
                ? GetDirectorySize(new DirectoryInfo(sourcePath))
                : new FileInfo(sourcePath).Length;

            if (!DriveInfo.GetDrives().Any(d => string.Equals(d.Name, destinationDrive, StringComparison.OrdinalIgnoreCase)))
            {
                throw new DriveNotFoundException($"Destination drive not found: {destinationDrive}");
            }

            var driveInfo = new DriveInfo(destinationDrive);
            var availableFreeSpace = driveInfo.AvailableFreeSpace;

            return (availableFreeSpace >= itemSize, availableFreeSpace, itemSize);
        }

        public static long GetDirectorySize(DirectoryInfo directoryInfo)
        {
            ArgumentNullException.ThrowIfNull(directoryInfo);

            if (!directoryInfo.Exists)
            {
                throw new DirectoryNotFoundException("Directory not found.");
            }

            var fileSizes = directoryInfo.GetFiles()
                                         .AsParallel()
                                         .Sum(file => file.Length);

            var directorySizes = directoryInfo.GetDirectories()
                                              .AsParallel()
                                              .Sum(dir => GetDirectorySize(dir));

            return fileSizes + directorySizes;
        }
    }

}
