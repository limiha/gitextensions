﻿using System;
using System.IO;
using GitCommands;
using GitUIPluginInterfaces;

namespace GitUI.UserControls.RevisionGridClasses
{
    public class IndexChangedEventArgs : EventArgs
    {
        public IndexChangedEventArgs(bool isIndexChanged)
        {
            IsIndexChanged = isIndexChanged;
        }

        public bool IsIndexChanged { get; private set; }
    }

    public sealed class IndexWatcher : IDisposable
    {
        public event EventHandler<IndexChangedEventArgs> Changed;

        private readonly IGitUICommandsSource UICommandsSource;

        private GitUICommands UICommands
        {
            get
            {
                return UICommandsSource.UICommands;
            }
        }

        private GitModule Module { get { return UICommands.Module; } }
        private IGitModuleState ModuleState => UICommands.ModuleState;

        public IndexWatcher(IGitUICommandsSource aUICommandsSource)
        {
            UICommandsSource = aUICommandsSource;
            UICommandsSource.GitUICommandsChanged += UICommandsSource_GitUICommandsChanged;
            GitIndexWatcher = new FileSystemWatcher();
            RefsWatcher = new FileSystemWatcher();
            SetFileSystemWatcher();

            IndexChanged = true;
            GitIndexWatcher.Changed += fileSystemWatcher_Changed;
            RefsWatcher.Changed += fileSystemWatcher_Changed;
        }

        void UICommandsSource_GitUICommandsChanged(object sender, GitUICommandsChangedEventArgs e)
        {
            Clear();
        }

        private void SetFileSystemWatcher()
        {
            if (!Module.IsValidGitWorkingDir())
            {
                GitIndexWatcher.EnableRaisingEvents = false;
                RefsWatcher.EnableRaisingEvents = false;
            }
            else
            {
                try
                {
                    enabled = AppSettings.UseFastChecks;

                    _gitDirPath = ModuleState.WorkingDirGitDir;

                    GitIndexWatcher.Path = _gitDirPath;
                    GitIndexWatcher.Filter = "index";
                    GitIndexWatcher.IncludeSubdirectories = false;
                    GitIndexWatcher.EnableRaisingEvents = enabled;

                    RefsWatcher.Path = Path.Combine(Module.GitCommonDirectory, "refs");
                    RefsWatcher.IncludeSubdirectories = true;
                    RefsWatcher.EnableRaisingEvents = enabled;
                }
                catch
                {
                    enabled = false;
                }
            }
        }

        private bool indexChanged;
        public bool IndexChanged
        {
            get
            {
                if (!enabled)
                    return true;

                if (_gitDirPath != ModuleState.WorkingDirGitDir)
                    return true;

                return indexChanged;
            }
            set
            {
                indexChanged = value;
                GitIndexWatcher.EnableRaisingEvents = !IndexChanged;

                if (Changed != null)
                    Changed(this, new IndexChangedEventArgs(IndexChanged));
            }
        }

        private bool enabled;
        private string _gitDirPath;
        private FileSystemWatcher GitIndexWatcher { get; set; }
        private FileSystemWatcher RefsWatcher { get; set; }

        private void fileSystemWatcher_Changed(object sender, FileSystemEventArgs e)
        {
            IndexChanged = true;
        }

        public void Reset()
        {
            IndexChanged = false;
            RefreshWatcher();
        }

        public void Clear()
        {
            IndexChanged = true;
            RefreshWatcher();
        }

        private void RefreshWatcher()
        {
            if (_gitDirPath != ModuleState.WorkingDirGitDir || enabled != AppSettings.UseFastChecks)
                SetFileSystemWatcher();
        }

        public void Dispose()
        {
            enabled = false;
            GitIndexWatcher.EnableRaisingEvents = false;
            GitIndexWatcher.Changed -= fileSystemWatcher_Changed;
            RefsWatcher.Changed -= fileSystemWatcher_Changed;
            GitIndexWatcher.Dispose();
            RefsWatcher.Dispose();
        }
    }
}
