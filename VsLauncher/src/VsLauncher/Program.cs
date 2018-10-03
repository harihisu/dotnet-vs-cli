using McMaster.Extensions.CommandLineUtils;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace VsLauncher
{
    public class Program
    {
        public static int Main(string[] args)
        {
            var app = new CommandLineApplication();

            app.HelpOption();
            var optionVersion = app.Option("-v|--version",
                "VS version number (e.g. 15.7). If none specified will get the latest one.",
                CommandOptionType.SingleValue);

            var optionSolutionFolderPath = app.Option("-p|--solution-folder-path",
                "Solution root folder. If none specified will get current folder.",
                CommandOptionType.SingleValue);

            var optionSolutionFileName = app.Option("-s|--solution-file-name",
                "Solution exact file name. If none specified will get the first found.",
                CommandOptionType.SingleValue);

            var optionSudo = app.Option("--sudo",
                "Start Visual Studio as Administrator.",
                CommandOptionType.NoValue);

            app.OnExecute(() =>
            {
                var version = optionVersion.HasValue()
                    ? optionVersion.Value()
                    : string.Empty;

                var solutionFolderPath = optionSolutionFolderPath.HasValue()
                    ? optionSolutionFolderPath.Value()
                    : ".";

                var solutionFileName = optionSolutionFileName.HasValue()
                    ? optionSolutionFileName.Value()
                    : "*.sln";

                var runAsAdmin = optionSudo.HasValue();

                OnExecute(version, solutionFolderPath, solutionFileName, runAsAdmin);
            });

            return app.Execute(args);
        }

        //[Option(ShortName = "v", Description = "VS version number (e.g. 15.7). If none specified will get the latest one.")]
        //public string Version { get; }

        //[Option(ShortName = "p", Description = "Solution root folder. If none specified will get current folder.")]
        //public string SolutionFolderPath
        //{
        //    get => _SolutionFolderPath;
        //    set
        //    {
        //        if (string.IsNullOrWhiteSpace(value))
        //        {
        //            _SolutionFolderPath = ".";
        //        }
        //        else
        //        {
        //            _SolutionFolderPath = value;
        //        }
        //    }
        //}

        //private string _SolutionFolderPath;

        //[Option(ShortName = "s", Description = "Solution exact file name. If none specified will get the first found.")]
        //public string SolutionFileName
        //{
        //    get => _SolutionFileName;
        //    set
        //    {
        //        if (string.IsNullOrWhiteSpace(value) || !value.EndsWith(".sln"))
        //        {
        //            _SolutionFileName = "*.sln";
        //        }
        //        else
        //        {
        //            _SolutionFileName = value;
        //        }
        //    }
        //}

        //private string _SolutionFileName;

        //[Option("--sudo", "Start VS as Administrator.", CommandOptionType.SingleValue)]
        //public string Sudo { get => string.Empty; set { _RunAsAdmin = true; } }
        //private bool _RunAsAdmin = false;

        private static void OnExecute(string vsVersion, string solutionFolderPath, string solutionFileName, bool runAsAdmin)
        {
            var fileFinder = new FileFinder();
            var vsExePath = string.Empty;
            try
            {
                if (!string.IsNullOrWhiteSpace(vsVersion))
                {
                    vsExePath = fileFinder.GetMatchingVersionExecutable(vsVersion);
                }
                else
                {
                    vsExePath = fileFinder.GetLatestVersionExecutable();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            var slnFile = fileFinder.FindAllFiles(solutionFolderPath, solutionFileName).FirstOrDefault();

            if (string.IsNullOrWhiteSpace(slnFile))
            {
                Console.WriteLine("Cannot find solution file");
            }

            StartVs(vsExePath, slnFile, runAsAdmin);
        }

        private static void StartVs(string vsExe, string slnFile, bool runAsAdmin)
        {
            var psi = new ProcessStartInfo
            {
                UseShellExecute = true,
                FileName = vsExe,
                Arguments = slnFile,
            };

            if (runAsAdmin)
            {
                psi.Verb = "runas";
            }

            Process.Start(psi);
        }
    }
}
