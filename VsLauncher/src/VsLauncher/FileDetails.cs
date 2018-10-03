using System.Collections.Generic;
using System.Linq;

namespace VsLauncher
{
    public class FileDetails
    {
        public string Path { get; set; }

        public int VersionMajor { get; set; }

        public int VersionMinor { get; set; }

        public int VersionBuild { get; set; }
    }

    public class FileDetailsComparer : IComparer<FileDetails>
    {
        public int Compare(FileDetails x, FileDetails y)
        {
            if (x.VersionMajor < y.VersionMajor)
            {
                return -1;
            }
            else if (x.VersionMajor > y.VersionMajor)
            {
                return 1;
            }

            if (x.VersionMinor < y.VersionMinor)
            {
                return -1;
            }
            else if (x.VersionMinor > y.VersionMinor)
            {
                return 1;
            }

            if (x.VersionBuild < y.VersionBuild)
            {
                return -1;
            }
            else if (x.VersionBuild > y.VersionBuild)
            {
                return 1;
            }

            return 0;
        }
    }
}
