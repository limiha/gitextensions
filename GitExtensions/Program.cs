using System;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Windows.Forms;
using GitCommands;
using GitCommands.Utils;
using GitUI;
using GitUI.CommandsDialogs;
using GitUI.CommandsDialogs.SettingsDialog;
using GitUI.CommandsDialogs.SettingsDialog.Pages;
using GitUI.Infrastructure.Telemetry;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Threading;
using Microsoft.WindowsAPICodePack.Dialogs;
using ResourceManager;

namespace GitExtensions
{
    internal static class Program
    {
        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            ////if (Environment.OSVersion.Version.Major >= 6)
            ////{
            ////    SetProcessDPIAware();
            ////}

            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // This form created to obtain UI synchronization context only
            using (new Form())
            {
                // Store the shared JoinableTaskContext
                ThreadHelper.JoinableTaskContext = new JoinableTaskContext();
            }

            string[] args = Environment.GetCommandLineArgs();
            var commands = new GitUICommands(GetWorkingDir(args));
            Application.Run(new FormBrowse(commands, ""));
        }

        [CanBeNull]
        private static string GetWorkingDir(string[] args)
        {
            string workingDir = null;

            if (args.Length >= 3)
            {
                // there is bug in .net
                // while parsing command line arguments, it unescapes " incorrectly
                // https://github.com/gitextensions/gitextensions/issues/3489
                string dirArg = args[2].TrimEnd('"');
                if (!string.IsNullOrWhiteSpace(dirArg))
                {
                    if (!Directory.Exists(dirArg))
                    {
                        dirArg = Path.GetDirectoryName(dirArg);
                    }

                    workingDir = GitModule.TryFindGitWorkingDir(dirArg);

                    if (Directory.Exists(workingDir))
                    {
                        workingDir = Path.GetFullPath(workingDir);
                    }

                    // Do not add this working directory to the recent repositories. It is a nice feature, but it
                    // also increases the startup time
                    ////if (Module.ValidWorkingDir())
                    ////   Repositories.RepositoryHistory.AddMostRecentRepository(Module.WorkingDir);
                }
            }

            if (args.Length <= 1 && workingDir == null && AppSettings.StartWithRecentWorkingDir)
            {
                if (GitModule.IsValidGitWorkingDir(AppSettings.RecentWorkingDir))
                {
                    workingDir = AppSettings.RecentWorkingDir;
                }
            }

            if (args.Length > 1 && workingDir == null)
            {
                // If no working dir is yet found, try to find one relative to the current working directory.
                // This allows the `fileeditor` command to discover repository configuration which is
                // required for core.commentChar support.
                workingDir = GitModule.TryFindGitWorkingDir(Environment.CurrentDirectory);
            }

            return workingDir;
        }
    }
}
