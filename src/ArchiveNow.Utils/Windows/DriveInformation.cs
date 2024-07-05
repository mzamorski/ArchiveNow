using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace ArchiveNow.Utils.Windows
{
    public class DriveInformation
    {
        [DllImport("Kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool GetVolumeInformation(
            string rootPathName,
            StringBuilder volumeName,
            int volumeNameSize,
            out uint volumeSerialNumber,
            out uint maximumComponentLength,
            out uint fileSystemFlags,
            StringBuilder fileSystemName,
            int fileSystemNameSize);

        public string VolumeName { get; private set; }

        public string FileSystemName { get; private set; }

        public uint SerialNumber { get; private set; }

        public string DriveLetter { get; private set; }

        public uint MaxPathLength { get; set; }

        public static DriveInformation Get(string driveLetter)
        {
            const int VOLUME_NAME_SIZE = 255;
            const int FILE_SYSTEM_NAME_BUFFER_SIZE = 255;

            var volumeNameBuffer = new StringBuilder(VOLUME_NAME_SIZE);
            var fileSystemNameBuffer = new StringBuilder(FILE_SYSTEM_NAME_BUFFER_SIZE);

            if (!GetVolumeInformation(
                $"{driveLetter}",
                volumeNameBuffer,
                VOLUME_NAME_SIZE,
                out var volumeSerialNumber,
                out var maxPathLength,
                out _,
                fileSystemNameBuffer,
                FILE_SYSTEM_NAME_BUFFER_SIZE))
            {
                // Something failed, returns null
                throw new Win32Exception("GetVolumeInformation");
            }

            return new DriveInformation
            {
                DriveLetter = driveLetter,
                FileSystemName = fileSystemNameBuffer.ToString(),
                VolumeName = volumeNameBuffer.ToString(),
                SerialNumber = volumeSerialNumber,
                MaxPathLength = maxPathLength
            };
        }
    }
}
