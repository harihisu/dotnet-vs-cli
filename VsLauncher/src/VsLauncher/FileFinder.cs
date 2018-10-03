using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;

namespace VsLauncher
{
    public class FileFinder
    {
        public const string FolderPrefix = "Microsoft Visual Studio*";
        public readonly string InstallationRootFolder;

        public FileFinder()
        {
            InstallationRootFolder = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        }

        public string GetLatestVersionExecutable()
        {
            var allExecutables = GetAllVsExecutables();

            return GetLatestVersionExecutableOfList(allExecutables)?.Path;
        }

        public string GetMatchingVersionExecutable(string desiredVersion)
        {
            var allExecutables = GetAllVsExecutables();
            List<int> desiredVersionNumbers;
            try
            {
                desiredVersionNumbers = desiredVersion.Split('.')
                    .Select(s => int.Parse(s))
                    .ToList();

                if (desiredVersionNumbers.Count > 3 || desiredVersionNumbers.Count < 1)
                {
                    // we only support 1 to 3 parts
                    throw new Exception();
                }
            }
            catch (Exception)
            {
                throw new Exception("Cannot parse version number. Make sure to use the right format MAJOR[.MINOR[.BUILD]]." +
                    " Alphabetical characters not supported yet");
            }

            var filteredExecutables = allExecutables.Where(i => i.VersionMajor == desiredVersionNumbers[0]);

            if (desiredVersionNumbers.Count > 1)
            {
                filteredExecutables = filteredExecutables.Where(i => i.VersionMinor == desiredVersionNumbers[1]);
            }
            if (desiredVersionNumbers.Count > 2)
            {
                filteredExecutables = filteredExecutables.Where(i => i.VersionBuild == desiredVersionNumbers[2]);
            }

            return GetLatestVersionExecutableOfList(filteredExecutables)?.Path;
        }

        public IEnumerable<string> FindAllFiles(string rootFolder, string fileNamePattern)
        {
            return Directory.EnumerateFiles(rootFolder, fileNamePattern, SearchOption.AllDirectories);
        }

        private FileDetails GetLatestVersionExecutableOfList(IEnumerable<FileDetails> fileList)
        {
            return fileList.OrderByDescending(i => i, new FileDetailsComparer())
                .FirstOrDefault();
        }

        private IEnumerable<FileDetails> GetAllVsExecutables()
        {
            var installedVsFolders = Directory.GetDirectories(InstallationRootFolder, FolderPrefix);

            var allDevEnvs = installedVsFolders.SelectMany(vsFolder =>
            {
                return FindAllFiles(vsFolder, "devenv.exe");
            })
            .Select(GetFileDetails);

            return allDevEnvs;
        }

        public FileDetails GetFileDetails(string filePath)
        {
            var fileVersionInfo = FileVersionInfo.GetVersionInfo(filePath);

            return new FileDetails
            {
                Path = filePath,
                VersionMajor = fileVersionInfo.ProductMajorPart,
                VersionMinor = fileVersionInfo.ProductMinorPart,
                VersionBuild = fileVersionInfo.ProductBuildPart
            };
        }
    }
}
