using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using GitCommands;
using GitCommands.Config;
using GitCommands.Git;
using GitCommands.Git.Commands;
using GitCommands.Gpg;
using GitCommands.Submodules;
using GitCommands.UserRepositoryHistory;
using GitCommands.Utils;
using GitExtUtils;
using GitExtUtils.GitUI;
using GitExtUtils.GitUI.Theming;
using GitUI.CommandsDialogs.BrowseDialog;
using GitUI.CommandsDialogs.BrowseDialog.DashboardControl;
using GitUI.CommandsDialogs.WorktreeDialog;
using GitUI.HelperDialogs;
using GitUI.Hotkey;
using GitUI.Infrastructure.Telemetry;
using GitUI.Properties;
using GitUI.Script;
using GitUI.Shells;
using GitUI.UserControls;
using GitUI.UserControls.RevisionGrid;
using GitUIPluginInterfaces;
using GitUIPluginInterfaces.RepositoryHosts;
using JetBrains.Annotations;
using Microsoft.VisualStudio.Threading;
using Microsoft.Win32;
using Microsoft.WindowsAPICodePack.Taskbar;
using ResourceManager;

namespace GitUI.CommandsDialogs
{
    public sealed partial class FormBrowse : Form // GitModuleForm, IBrowseRepo
    {
        #region Translation

        private readonly TranslationString _noSubmodulesPresent = new TranslationString("No submodules");
        private readonly TranslationString _topProjectModuleFormat = new TranslationString("Top project: {0}");
        private readonly TranslationString _superprojectModuleFormat = new TranslationString("Superproject: {0}");
        private readonly TranslationString _goToSuperProject = new TranslationString("Go to superproject");

        private readonly TranslationString _indexLockCantDelete = new TranslationString("Failed to delete index.lock.");

        private readonly TranslationString _loading = new TranslationString("Loading...");

        private readonly TranslationString _noReposHostPluginLoaded = new TranslationString("No repository host plugin loaded.");
        private readonly TranslationString _noReposHostFound = new TranslationString("Could not find any relevant repository hosts for the currently open repository.");

        private readonly TranslationString _configureWorkingDirMenu = new TranslationString("Configure this menu");

        private readonly TranslationString _updateCurrentSubmodule = new TranslationString("Update current submodule");

        private readonly TranslationString _pullFetch = new TranslationString("Fetch");
        private readonly TranslationString _pullFetchAll = new TranslationString("Fetch all");
        private readonly TranslationString _pullFetchPruneAll = new TranslationString("Fetch and prune all");
        private readonly TranslationString _pullMerge = new TranslationString("Pull - merge");
        private readonly TranslationString _pullRebase = new TranslationString("Pull - rebase");
        private readonly TranslationString _pullOpenDialog = new TranslationString("Open pull dialog");

        private readonly TranslationString _buildReportTabCaption = new TranslationString("Build Report");
        private readonly TranslationString _consoleTabCaption = new TranslationString("Console");

        private readonly TranslationString _noWorkingFolderText = new TranslationString("No working directory");
        private readonly TranslationString _commitButtonText = new TranslationString("Commit");

        private readonly TranslationString _undoLastCommitText = new TranslationString("You will still be able to find all the commit's changes in the staging area\n\nDo you want to continue?");
        private readonly TranslationString _undoLastCommitCaption = new TranslationString("Undo last commit");

        #endregion

        private readonly SplitterManager _splitterManager = new SplitterManager(new AppSettingsPath("FormBrowse"));
        [NotNull]
        private readonly FormBrowseMenus _formBrowseMenus;
        private readonly IAppTitleGenerator _appTitleGenerator;
        private readonly WindowsJumpListManager _windowsJumpListManager;
        private readonly ISubmoduleStatusProvider _submoduleStatusProvider;
        private readonly FormBrowseDiagnosticsReporter _formBrowseDiagnosticsReporter;
        private readonly ShellProvider _shellProvider = new ShellProvider();
        private Dashboard _dashboard;

        [Flags]
        private enum UpdateTargets
        {
            None = 1,
            DiffList = 2,
            FileTree = 4,
            CommitInfo = 8
        }

        private UpdateTargets _selectedRevisionUpdatedTargets = UpdateTargets.None;

        public RevisionGridControl RevisionGridControl { get => RevisionGrid; }

        [Obsolete("For VS designer and translation test only. Do not remove.")]
        private FormBrowse()
        {
            InitializeComponent();
        }

        private bool _skip = true;

        public FormBrowse([NotNull] GitUICommands commands, string filter, ObjectId selectCommit = null)
            ////: base(commands)
        {
            InitializeComponent();

            if (_skip)
            {
                return;
            }

            new ToolStripItem[]
            {
                translateToolStripMenuItem,
                recoverLostObjectsToolStripMenuItem,
                branchSelect,
                toolStripButtonPull,
                pullToolStripMenuItem,
                pullToolStripMenuItem1,
                mergeToolStripMenuItem,
                rebaseToolStripMenuItem1,
                fetchToolStripMenuItem,
                fetchAllToolStripMenuItem,
                fetchPruneAllToolStripMenuItem,
                toolStripButtonPush,
                pushToolStripMenuItem,
                branchToolStripMenuItem,
            }.ForEach(ColorHelper.AdaptImageLightness);

            _formBrowseDiagnosticsReporter = new FormBrowseDiagnosticsReporter(this);

            commandsToolStripMenuItem.DropDownOpening += CommandsToolStripMenuItem_DropDownOpening;

            MainSplitContainer.Visible = false;
            MainSplitContainer.SplitterDistance = DpiUtil.Scale(260);

            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await TaskScheduler.Default;
                PluginRegistry.Initialize();
                await ThreadHelper.JoinableTaskFactory.SwitchToMainThreadAsync();
                RegisterPlugins();
            }).FileAndForget();

            var repositoryDescriptionProvider = new RepositoryDescriptionProvider(new GitDirectoryResolver());
            _appTitleGenerator = new AppTitleGenerator(repositoryDescriptionProvider);
            _windowsJumpListManager = new WindowsJumpListManager(repositoryDescriptionProvider);

            if (!EnvUtils.RunningOnWindows())
            {
                toolStripSeparator6.Visible = false;
                PuTTYToolStripMenuItem.Visible = false;
            }

            RevisionGrid.SelectionChanged += (sender, e) =>
            {
                _selectedRevisionUpdatedTargets = UpdateTargets.None;
                RefreshSelection();
            };
            RevisionGrid.RevisionGraphLoaded += (sender, e) =>
            {
                if (sender is null || MainSplitContainer.Panel1Collapsed)
                {
                    // - the event is either not originated from the revision grid, or
                    // - the left panel is hidden
                    return;
                }

                // Apply filtering when:
                // 1. don't show reflogs, and
                // 2. one of the following
                //      a) show the current branch only, or
                //      b) filter on specific branch
                bool isFiltering = !AppSettings.ShowReflogReferences
                                && (AppSettings.ShowCurrentBranchOnly || AppSettings.BranchFilterEnabled);
            };

            pullToolStripMenuItem1.Tag = AppSettings.PullAction.None;
            mergeToolStripMenuItem.Tag = AppSettings.PullAction.Merge;
            rebaseToolStripMenuItem1.Tag = AppSettings.PullAction.Rebase;
            fetchToolStripMenuItem.Tag = AppSettings.PullAction.Fetch;
            fetchAllToolStripMenuItem.Tag = AppSettings.PullAction.FetchAll;
            fetchPruneAllToolStripMenuItem.Tag = AppSettings.PullAction.FetchPruneAll;

            FillNextPullActionAsDefaultToolStripMenuItems();
            RefreshDefaultPullAction();

            _submoduleStatusProvider = SubmoduleStatusProvider.Default;
            _submoduleStatusProvider.StatusUpdating += SubmoduleStatusProvider_StatusUpdating;
            _submoduleStatusProvider.StatusUpdated += SubmoduleStatusProvider_StatusUpdated;

            FillBuildReport(revision: null); // Ensure correct page visibility
            RevisionGrid.ShowBuildServerInfo = true;

            _formBrowseMenus = new FormBrowseMenus(mainMenuStrip);
            RevisionGrid.MenuCommands.MenuChanged += (sender, e) => _formBrowseMenus.OnMenuCommandsPropertyChanged();
            SystemEvents.SessionEnding += (sender, args) => SaveApplicationSettings();

            ManageWorktreeSupport();

            WorkaroundToolbarLocationBug();

            var toolBackColor = SystemColors.Window;
            var toolForeColor = SystemColors.WindowText;
            BackColor = toolBackColor;
            ForeColor = toolForeColor;
            mainMenuStrip.BackColor = toolBackColor;
            mainMenuStrip.ForeColor = toolForeColor;

            InitToolStripStyles(toolForeColor, toolBackColor);

            foreach (var control in this.FindDescendants())
            {
                control.AllowDrop = true;
                control.DragEnter += FormBrowse_DragEnter;
                control.DragDrop += FormBrowse_DragDrop;
            }

            if (selectCommit != null)
            {
                RevisionGrid.InitialObjectId = selectCommit;
            }

            UpdateCommitButtonAndGetBrush(null, AppSettings.ShowGitStatusInBrowseToolbar);

            // Populate terminal tab after translation within InitializeComplete
            FillTerminalTab();

            FillUserShells(defaultShell: BashShell.ShellName);

            RevisionGrid.ToggledBetweenArtificialAndHeadCommits += (s, e) => FocusRevisionDiffFileStatusList();

            return;

            void WorkaroundToolbarLocationBug()
            {
                ////// Layout engine bug (?) which may change the order of toolbars
                ////// if the 1st one becomes longer than the 2nd toolbar's Location.X
                ////// the layout engine will be place the 2nd toolbar first
                ////toolPanel.TopToolStripPanel.Controls.Clear();
                ////toolPanel.TopToolStripPanel.Controls.Add(ToolStripFilters);
                ////toolPanel.TopToolStripPanel.Controls.Add(ToolStripMain);
            }

            void FocusRevisionDiffFileStatusList()
            {
            }

            void ManageWorktreeSupport()
            {
                if (!GitVersion.Current.SupportWorktree)
                {
                    createWorktreeToolStripMenuItem.Enabled = false;
                }

                if (!GitVersion.Current.SupportWorktreeList)
                {
                    manageWorktreeToolStripMenuItem.Enabled = false;
                }
            }

            void InitToolStripStyles(Color toolForeColor, Color toolBackColor)
            {
                toolPanel.TopToolStripPanel.BackColor = toolBackColor;
                toolPanel.TopToolStripPanel.ForeColor = toolForeColor;

                ToolStripMain.BackColor = toolBackColor;
                ToolStripMain.ForeColor = toolForeColor;
                ToolStripFilters.BackColor = toolBackColor;
                ToolStripFilters.ForeColor = toolForeColor;
                toolStripRevisionFilterDropDownButton.BackColor = toolBackColor;
                toolStripRevisionFilterDropDownButton.ForeColor = toolForeColor;

                var toolTextBoxBackColor = SystemColors.Window;
                toolStripBranchFilterComboBox.BackColor = toolTextBoxBackColor;
                toolStripBranchFilterComboBox.ForeColor = toolForeColor;
                toolStripRevisionFilterTextBox.BackColor = toolTextBoxBackColor;
                toolStripRevisionFilterTextBox.ForeColor = toolForeColor;

                // Scale tool strip items according to DPI
                toolStripBranchFilterComboBox.Size = DpiUtil.Scale(toolStripBranchFilterComboBox.Size);
                toolStripRevisionFilterTextBox.Size = DpiUtil.Scale(toolStripRevisionFilterTextBox.Size);
            }

            Brush UpdateCommitButtonAndGetBrush(IReadOnlyList<GitItemStatus> status, bool showCount)
            {
                var repoStateVisualiser = new RepoStateVisualiser();
                var (image, brush) = repoStateVisualiser.Invoke(status);

                if (showCount)
                {
                    toolStripButtonCommit.Image = image;

                    if (status != null)
                    {
                        toolStripButtonCommit.Text = string.Format("{0} ({1})", _commitButtonText, status.Count);
                        toolStripButtonCommit.AutoSize = true;
                    }
                    else
                    {
                        int width = toolStripButtonCommit.Width;
                        toolStripButtonCommit.Text = _commitButtonText.Text;
                        if (width > toolStripButtonCommit.Width)
                        {
                            toolStripButtonCommit.AutoSize = false;
                            toolStripButtonCommit.Width = width;
                        }
                    }
                }
                else
                {
                    toolStripButtonCommit.Image = repoStateVisualiser.Invoke(new List<GitItemStatus>()).image;

                    toolStripButtonCommit.Text = _commitButtonText.Text;
                    toolStripButtonCommit.AutoSize = true;
                }

                return brush;
            }
        }

        private void FillNextPullActionAsDefaultToolStripMenuItems()
        {
            var setDefaultPullActionDropDown = (ToolStripDropDownMenu)setDefaultPullButtonActionToolStripMenuItem.DropDown;

            // Show both Check and Image margins in a menu
            setDefaultPullActionDropDown.ShowImageMargin = true;
            setDefaultPullActionDropDown.ShowCheckMargin = true;

            // Prevent submenu from closing while options are changed
            setDefaultPullActionDropDown.Closing += (sender, args) =>
            {
                if (args.CloseReason == ToolStripDropDownCloseReason.ItemClicked)
                {
                    args.Cancel = true;
                }
            };

            var setDefaultPullActionDropDownItems = toolStripButtonPull.DropDownItems
                .OfType<ToolStripMenuItem>()
                .Where(tsmi => tsmi.Tag is AppSettings.PullAction)
                .Select(tsmi =>
                {
                    ToolStripItem tsi = new ToolStripMenuItem
                    {
                        Name = $"{tsmi.Name}SetDefault",
                        Text = tsmi.Text,
                        CheckOnClick = true,
                        Image = tsmi.Image,
                        Tag = tsmi.Tag
                    };

                    tsi.Click += SetDefaultPullActionMenuItemClick;

                    return tsi;
                });

            setDefaultPullActionDropDown.Items.AddRange(setDefaultPullActionDropDownItems.ToArray());

            void SetDefaultPullActionMenuItemClick(object sender, EventArgs eventArgs)
            {
                var clickedMenuItem = (ToolStripMenuItem)sender;
                AppSettings.DefaultPullAction = (AppSettings.PullAction)clickedMenuItem.Tag;
                RefreshDefaultPullAction();
            }
        }

        private void FillUserShells(string defaultShell)
        {
            userShell.DropDownItems.Clear();

            bool userShellAccessible = false;
            ToolStripMenuItem selectedDefaultShell = null;
            foreach (IShellDescriptor shell in _shellProvider.GetShells())
            {
                if (!shell.HasExecutable)
                {
                    continue;
                }

                var toolStripMenuItem = new ToolStripMenuItem(shell.Name);
                userShell.DropDownItems.Add(toolStripMenuItem);
                toolStripMenuItem.Tag = shell;
                toolStripMenuItem.Image = shell.Icon;
                toolStripMenuItem.ToolTipText = shell.Name;
                toolStripMenuItem.Click += userShell_Click;

                if (selectedDefaultShell == null || string.Equals(shell.Name, defaultShell, StringComparison.InvariantCultureIgnoreCase))
                {
                    userShellAccessible = true;
                    selectedDefaultShell = toolStripMenuItem;
                }
            }

            if (selectedDefaultShell != null)
            {
                userShell.Image = selectedDefaultShell.Image;
                userShell.ToolTipText = selectedDefaultShell.ToolTipText;
                userShell.Tag = selectedDefaultShell.Tag;
            }

            userShell.Visible = userShell.DropDownItems.Count > 0;

            // a user may have a specific shell configured in settings, but the shell is no longer available
            // set the first available shell as default
            if (userShell.Visible && !userShellAccessible)
            {
                var shell = (IShellDescriptor)userShell.DropDownItems[0].Tag;
                userShell.Image = shell.Icon;
                userShell.ToolTipText = shell.Name;
                userShell.Tag = shell;
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // ReSharper disable ConstantConditionalAccessQualifier - these can be null if run from under the TranslatioApp

                _formBrowseMenus?.Dispose();
                components?.Dispose();
                _windowsJumpListManager?.Dispose();

                // ReSharper restore ConstantConditionalAccessQualifier
            }

            base.Dispose(disposing);
        }

        ////protected override void OnLoad(EventArgs e)
        ////{
        ////    HideVariableMainMenuItems();
        ////    RefreshSplitViewLayout();
        ////    LayoutRevisionInfo();
        ////    InternalInitialize(false);

        ////    if (!null.IsValidGitWorkingDir())
        ////    {
        ////        base.OnLoad(e);
        ////        return;
        ////    }

        ////    UpdateSubmodulesStructure();
        ////    UpdateStashCount();

        ////    toolStripButtonPush.DisplayAheadBehindInformation(null.GetSelectedBranch());

        ////    _formBrowseDiagnosticsReporter.Report();

        ////    base.OnLoad(e);

        ////    SetSplitterPositions();
        ////}

        ////protected override void OnActivated(EventArgs e)
        ////{
        ////    // wait for windows to really be displayed, which isn't necessarily the case in OnLoad()
        ////    if (_windowsJumpListManager.NeedsJumpListCreation)
        ////    {
        ////        _windowsJumpListManager.CreateJumpList(
        ////            Handle,
        ////            new WindowsThumbnailToolbarButtons(
        ////                new WindowsThumbnailToolbarButton(toolStripButtonCommit.Text, toolStripButtonCommit.Image, CommitToolStripMenuItemClick),
        ////                new WindowsThumbnailToolbarButton(toolStripButtonPush.Text, toolStripButtonPush.Image, PushToolStripMenuItemClick),
        ////                new WindowsThumbnailToolbarButton(toolStripButtonPull.Text, toolStripButtonPull.Image, PullToolStripMenuItemClick)));
        ////    }

        ////    this.InvokeAsync(OnActivate).FileAndForget();
        ////    base.OnActivated(e);
        ////}

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SaveApplicationSettings();

            foreach (var control in this.FindDescendants())
            {
                control.DragEnter -= FormBrowse_DragEnter;
                control.DragDrop -= FormBrowse_DragDrop;
            }

            base.OnFormClosing(e);
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            _splitterManager.SaveSplitters();
            base.OnClosing(e);
        }

        private bool NeedsGitStatusMonitor()
        {
            return AppSettings.ShowGitStatusInBrowseToolbar || (AppSettings.ShowGitStatusForArtificialCommits && AppSettings.RevisionGraphShowWorkingDirChanges);
        }

        private void UICommands_PostRepositoryChanged(object sender, GitUIEventArgs e)
        {
            ////this.InvokeAsync(RefreshRevisions).FileAndForget();
            ////UpdateSubmodulesStructure();
            ////UpdateStashCount();
        }

        private void RefreshRevisions()
        {
            ////if (RevisionGrid.IsDisposed || IsDisposed || Disposing)
            ////{
            ////    return;
            ////}

            ////_gitStatusMonitor.InvalidateGitWorkingDirectoryStatus();
            ////_gitStatusMonitor.RequestRefresh();

            ////if (_dashboard == null || !_dashboard.Visible)
            ////{
            ////    RevisionGrid.ForceRefreshRevisions();
            ////    InternalInitialize(true);
            ////}

            ////toolStripButtonPush.DisplayAheadBehindInformation(null.GetSelectedBranch());
        }

        private void RefreshSelection()
        {
            var selectedRevisions = RevisionGrid.GetSelectedRevisions();
            var selectedRevision = RevisionGrid.GetSelectedRevisions().FirstOrDefault();

            FillFileTree(selectedRevision);
            FillDiff(selectedRevisions);

            var oldBody = selectedRevision?.Body;
            FillCommitInfo(selectedRevision);

            // If the revision's body has been updated then the grid needs to be refreshed to display it
            if (selectedRevision != null && selectedRevision.HasMultiLineMessage && oldBody != selectedRevision.Body)
            {
                RevisionGrid.Refresh();
            }

            ThreadHelper.JoinableTaskFactory.RunAsync(() => FillGpgInfoAsync(selectedRevision));
            FillBuildReport(selectedRevision);
        }

        #region IBrowseRepo

        public void GoToRef(string refName, bool showNoRevisionMsg, bool toggleSelection = false)
        {
            using (WaitCursorScope.Enter())
            {
                RevisionGrid.GoToRef(refName, showNoRevisionMsg, toggleSelection);
            }
        }

        #endregion

        private void ShowDashboard()
        {
            toolPanel.SuspendLayout();
            toolPanel.TopToolStripPanelVisible = false;
            toolPanel.BottomToolStripPanelVisible = false;
            toolPanel.LeftToolStripPanelVisible = false;
            toolPanel.RightToolStripPanelVisible = false;
            toolPanel.ResumeLayout();

            MainSplitContainer.Visible = false;

            if (_dashboard == null)
            {
                _dashboard = new Dashboard { Dock = DockStyle.Fill };
                _dashboard.GitModuleChanged += SetGitModule;
                toolPanel.ContentPanel.Controls.Add(_dashboard);
            }

            Text = _appTitleGenerator.Generate(branchName: Strings.NoBranch);

            _dashboard.RefreshContent();
            _dashboard.Visible = true;
            _dashboard.BringToFront();

            DiagnosticsClient.TrackPageView("Dashboard");
        }

        private void HideDashboard()
        {
            MainSplitContainer.Visible = true;
            if (_dashboard == null || !_dashboard.Visible)
            {
                return;
            }

            _dashboard.Visible = false;
            toolPanel.SuspendLayout();
            toolPanel.TopToolStripPanelVisible = true;
            toolPanel.BottomToolStripPanelVisible = true;
            toolPanel.LeftToolStripPanelVisible = true;
            toolPanel.RightToolStripPanelVisible = true;
            toolPanel.ResumeLayout();

            DiagnosticsClient.TrackPageView("Revision graph");
        }

        private void UpdatePluginMenu(bool validWorkingDir)
        {
            foreach (ToolStripItem item in pluginsToolStripMenuItem.DropDownItems)
            {
                item.Enabled = !(item.Tag is IGitPluginForRepository) || validWorkingDir;
            }
        }

        private void RegisterPlugins()
        {
            const string PluginManagerName = "Plugin Manager";
            var existingPluginMenus = pluginsToolStripMenuItem.DropDownItems.OfType<ToolStripMenuItem>().ToLookup(c => c.Tag);

            lock (PluginRegistry.Plugins)
            {
                var pluginEntries = PluginRegistry.Plugins
                    .OrderByDescending(entry => entry.Name, StringComparer.CurrentCultureIgnoreCase);

                // pluginsToolStripMenuItem.DropDownItems menu already contains at least 2 items:
                //    [1] Separator
                //    [0] Plugin Settings
                // insert all plugins except 'Plugin Manager' above the separator
                foreach (var plugin in pluginEntries)
                {
                    // don't add the plugin to the Plugins menu, if already added
                    if (existingPluginMenus.Contains(plugin))
                    {
                        continue;
                    }

                    var item = new ToolStripMenuItem
                    {
                        Text = plugin.Description,
                        Image = plugin.Icon,
                        Tag = plugin
                    };
                    item.Click += delegate
                    {
                        if (plugin.Execute(new GitUIEventArgs(this, null)))
                        {
                            RefreshRevisions();
                        }
                    };

                    if (plugin.Name == PluginManagerName)
                    {
                        // insert Plugin Manager below the separator
                        pluginsToolStripMenuItem.DropDownItems.Insert(pluginsToolStripMenuItem.DropDownItems.Count - 1, item);
                    }
                    else
                    {
                        pluginsToolStripMenuItem.DropDownItems.Insert(0, item);
                    }
                }

                if (_dashboard?.Visible ?? false)
                {
                    // now that plugins are registered, populate Git-host-plugin actions on Dashboard, like "Clone GitHub repository"
                    _dashboard.RefreshContent();
                }

                mainMenuStrip?.Refresh();
            }

            // Show "Repository hosts" menu item when there is at least 1 repository host plugin loaded
            _repositoryHostsToolStripMenuItem.Visible = PluginRegistry.GitHosters.Count > 0;
            if (PluginRegistry.GitHosters.Count == 1)
            {
                _repositoryHostsToolStripMenuItem.Text = PluginRegistry.GitHosters[0].Description;
            }
        }

        /// <summary>
        /// to avoid showing menu items that should not be there during
        /// the transition from dashboard to repo browser and vice versa
        ///
        /// and reset hotkeys that are shared between mutual exclusive menu items
        /// </summary>
        private void HideVariableMainMenuItems()
        {
            dashboardToolStripMenuItem.Visible = false;
            repositoryToolStripMenuItem.Visible = false;
            commandsToolStripMenuItem.Visible = false;
            pluginsToolStripMenuItem.Visible = false;
            refreshToolStripMenuItem.ShortcutKeys = Keys.None;
            refreshDashboardToolStripMenuItem.ShortcutKeys = Keys.None;
            _repositoryHostsToolStripMenuItem.Visible = false;
            _formBrowseMenus.RemoveRevisionGridMainMenuItems();
            mainMenuStrip.Refresh();
        }

        private void InternalInitialize(bool hard)
        {
            toolPanel.SuspendLayout();
            toolPanel.TopToolStripPanel.SuspendLayout();

            using (WaitCursorScope.Enter())
            {
                // check for updates
                if (AppSettings.CheckForUpdates && AppSettings.LastUpdateCheck.AddDays(7) < DateTime.Now)
                {
                    AppSettings.LastUpdateCheck = DateTime.Now;
                    var updateForm = new FormUpdates(AppSettings.AppVersion);
                    updateForm.SearchForUpdatesAndShow(Owner, false);
                }

                bool hasWorkingDir = true;
                if (hasWorkingDir)
                {
                    HideDashboard();
                }
                else
                {
                    ShowDashboard();
                }

                bool bareRepository = false;
                bool isDashboard = _dashboard != null && _dashboard.Visible;
                bool validBrowseDir = !isDashboard;

                branchSelect.Text = validBrowseDir ? "safdsdfa" : "";
                toolStripButtonLevelUp.Enabled = hasWorkingDir && !bareRepository;
                fileExplorerToolStripMenuItem.Enabled = validBrowseDir;
                manageRemoteRepositoriesToolStripMenuItem1.Enabled = validBrowseDir;
                branchSelect.Enabled = validBrowseDir;
                toolStripButtonCommit.Enabled = validBrowseDir && !bareRepository;

                toolStripButtonPull.Enabled = validBrowseDir;
                toolStripButtonPush.Enabled = validBrowseDir;
                dashboardToolStripMenuItem.Visible = isDashboard;
                pluginsToolStripMenuItem.Visible = validBrowseDir;
                repositoryToolStripMenuItem.Visible = validBrowseDir;
                commandsToolStripMenuItem.Visible = validBrowseDir;
                toolStripFileExplorer.Enabled = validBrowseDir;
                if (!isDashboard)
                {
                    refreshToolStripMenuItem.ShortcutKeys = Keys.F5;
                }
                else
                {
                    refreshDashboardToolStripMenuItem.ShortcutKeys = Keys.F5;
                }

                UpdatePluginMenu(validBrowseDir);
                gitMaintenanceToolStripMenuItem.Enabled = validBrowseDir;
                editgitignoreToolStripMenuItem1.Enabled = validBrowseDir;
                editGitAttributesToolStripMenuItem.Enabled = validBrowseDir;
                editmailmapToolStripMenuItem.Enabled = validBrowseDir;
                toolStripSplitStash.Enabled = validBrowseDir && !bareRepository;
                _createPullRequestsToolStripMenuItem.Enabled = validBrowseDir;
                _viewPullRequestsToolStripMenuItem.Enabled = validBrowseDir;

                if (repositoryToolStripMenuItem.Visible)
                {
                    manageSubmodulesToolStripMenuItem.Enabled = !bareRepository;
                    updateAllSubmodulesToolStripMenuItem.Enabled = !bareRepository;
                    synchronizeAllSubmodulesToolStripMenuItem.Enabled = !bareRepository;
                    editgitignoreToolStripMenuItem1.Enabled = !bareRepository;
                    editGitAttributesToolStripMenuItem.Enabled = !bareRepository;
                    editmailmapToolStripMenuItem.Enabled = !bareRepository;
                }

                if (commandsToolStripMenuItem.Visible)
                {
                    commitToolStripMenuItem.Enabled = !bareRepository;
                    mergeToolStripMenuItem.Enabled = !bareRepository;
                    rebaseToolStripMenuItem1.Enabled = !bareRepository;
                    pullToolStripMenuItem1.Enabled = !bareRepository;
                    cleanupToolStripMenuItem.Enabled = !bareRepository;
                    stashToolStripMenuItem.Enabled = !bareRepository;
                    checkoutBranchToolStripMenuItem.Enabled = !bareRepository;
                    mergeBranchToolStripMenuItem.Enabled = !bareRepository;
                    rebaseToolStripMenuItem.Enabled = !bareRepository;
                    applyPatchToolStripMenuItem.Enabled = !bareRepository;
                }

                stashChangesToolStripMenuItem.Enabled = !bareRepository;
                gitGUIToolStripMenuItem.Enabled = !bareRepository;

                SetShortcutKeyDisplayStringsFromHotkeySettings();

                if (hard && hasWorkingDir)
                {
                    ShowRevisions();
                }

                RefreshWorkingDirComboText();
                var branchName = !string.IsNullOrEmpty(branchSelect.Text) ? branchSelect.Text : Strings.NoBranch;

                LoadUserMenu();

                if (validBrowseDir)
                {
                    // add Navigate and View menu
                    _formBrowseMenus.ResetMenuCommandSets();
                    _formBrowseMenus.AddMenuCommandSet(MainMenuItem.NavigateMenu, RevisionGrid.MenuCommands.NavigateMenuCommands);
                    _formBrowseMenus.AddMenuCommandSet(MainMenuItem.ViewMenu, RevisionGrid.MenuCommands.ViewMenuCommands);

                    _formBrowseMenus.InsertRevisionGridMainMenuItems(repositoryToolStripMenuItem);
                }
                else
                {
                    _windowsJumpListManager.DisableThumbnailToolbar();
                }
            }

            toolPanel.TopToolStripPanel.ResumeLayout();
            toolPanel.ResumeLayout();

            return;

            void SetShortcutKeyDisplayStringsFromHotkeySettings()
            {
                // TODO: add more
            }

            void LoadUserMenu()
            {
                var scripts = ScriptManager.GetScripts()
                    .Where(script => script.Enabled && script.OnEvent == ScriptEvent.ShowInUserMenuBar)
                    .ToList();

                for (int i = ToolStripMain.Items.Count - 1; i >= 0; i--)
                {
                    if (ToolStripMain.Items[i].Tag as string == "userscript")
                    {
                        ToolStripMain.Items.RemoveAt(i);
                    }
                }

                if (scripts.Count == 0)
                {
                    return;
                }

                ToolStripMain.Items.Add(new ToolStripSeparator { Tag = "userscript" });

                foreach (var script in scripts)
                {
                    var button = new ToolStripButton
                    {
                        // store scriptname
                        Text = script.Name,
                        Tag = "userscript",
                        Enabled = true,
                        Visible = true,
                        Image = script.GetIcon(),
                        DisplayStyle = ToolStripItemDisplayStyle.ImageAndText
                    };

                    button.Click += delegate
                    {
                        if (ScriptRunner.RunScript(this, null, script.Name, null, RevisionGrid).NeedsGridRefresh)
                        {
                            RevisionGrid.RefreshRevisions();
                        }
                    };

                    // add to toolstrip
                    ToolStripMain.Items.Add(button);
                }
            }

            void ShowRevisions()
            {
                if (RevisionGrid.IndexWatcher.IndexChanged)
                {
                    RefreshSelection();
                }

                RevisionGrid.IndexWatcher.Reset();
            }
        }

        private void UpdateStashCount()
        {
            if (AppSettings.ShowStashCount)
            {
                ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
                {
                    // Add a delay to not interfere with GUI updates when switching repository
                    await Task.Delay(500);
                    await TaskScheduler.Default;

                    await this.SwitchToMainThreadAsync();
                }).FileAndForget();
            }
            else
            {
                toolStripSplitStash.Text = string.Empty;
            }
        }

        #region Working directory combo box

        /// <summary>Updates the text shown on the combo button itself.</summary>
        private void RefreshWorkingDirComboText()
        {
            var path = "WorkingDir";

            // it appears at times null.WorkingDir path is an empty string, this caused issues like #4874
            if (string.IsNullOrWhiteSpace(path))
            {
                _NO_TRANSLATE_WorkingDir.Text = _noWorkingFolderText.Text;
                return;
            }

            var recentRepositoryHistory = ThreadHelper.JoinableTaskFactory.Run(
                () => RepositoryHistoryManager.Locals.AddAsMostRecentAsync(path));

            var mostRecentRepos = new List<RecentRepoInfo>();
            using (var graphics = CreateGraphics())
            {
                var splitter = new RecentRepoSplitter
                {
                    MeasureFont = _NO_TRANSLATE_WorkingDir.Font,
                    Graphics = graphics
                };

                splitter.SplitRecentRepos(recentRepositoryHistory, mostRecentRepos, mostRecentRepos);

                var ri = mostRecentRepos.Find(e => e.Repo.Path.Equals(path, StringComparison.InvariantCultureIgnoreCase));

                _NO_TRANSLATE_WorkingDir.Text = PathUtil.GetDisplayPath(ri?.Caption ?? path);

                if (AppSettings.RecentReposComboMinWidth > 0)
                {
                    _NO_TRANSLATE_WorkingDir.AutoSize = false;
                    var captionWidth = graphics.MeasureString(_NO_TRANSLATE_WorkingDir.Text, _NO_TRANSLATE_WorkingDir.Font).Width;
                    captionWidth = captionWidth + _NO_TRANSLATE_WorkingDir.DropDownButtonWidth + 5;
                    _NO_TRANSLATE_WorkingDir.Width = Math.Max(AppSettings.RecentReposComboMinWidth, (int)captionWidth);
                }
                else
                {
                    _NO_TRANSLATE_WorkingDir.AutoSize = true;
                }
            }
        }

        private void WorkingDirDropDownOpening(object sender, EventArgs e)
        {
            _NO_TRANSLATE_WorkingDir.DropDownItems.Clear();

            var tsmiCategorisedRepos = new ToolStripMenuItem(tsmiFavouriteRepositories.Text, tsmiFavouriteRepositories.Image);
            PopulateFavouriteRepositoriesMenu(tsmiCategorisedRepos);
            if (tsmiCategorisedRepos.DropDownItems.Count > 0)
            {
                _NO_TRANSLATE_WorkingDir.DropDownItems.Add(tsmiCategorisedRepos);
            }

            PopulateRecentRepositoriesMenu(_NO_TRANSLATE_WorkingDir);

            _NO_TRANSLATE_WorkingDir.DropDownItems.Add(new ToolStripSeparator());

            var mnuOpenLocalRepository = new ToolStripMenuItem(openToolStripMenuItem.Text, openToolStripMenuItem.Image) { ShortcutKeys = openToolStripMenuItem.ShortcutKeys };
            mnuOpenLocalRepository.Click += OpenToolStripMenuItemClick;
            _NO_TRANSLATE_WorkingDir.DropDownItems.Add(mnuOpenLocalRepository);

            var mnuRecentReposSettings = new ToolStripMenuItem(_configureWorkingDirMenu.Text);
            mnuRecentReposSettings.Click += (hs, he) =>
            {
                using (var frm = new FormRecentReposSettings())
                {
                    frm.ShowDialog(this);
                }

                RefreshWorkingDirComboText();
            };

            _NO_TRANSLATE_WorkingDir.DropDownItems.Add(mnuRecentReposSettings);

            PreventToolStripSplitButtonClosing((ToolStripSplitButton)sender);
        }

        private void WorkingDirClick(object sender, EventArgs e)
        {
            _NO_TRANSLATE_WorkingDir.ShowDropDown();
        }

        private void _NO_TRANSLATE_WorkingDir_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                OpenToolStripMenuItemClick(sender, e);
            }
        }

        #endregion

        private void FillFileTree(GitRevision revision)
        {
            // Don't show the "File Tree" tab for artificial commits
            var showFileTreeTab = revision?.IsArtificial != true;

            if (showFileTreeTab)
            {
            }
            else
            {
            }

            _selectedRevisionUpdatedTargets |= UpdateTargets.FileTree;
        }

        private void FillDiff(IReadOnlyList<GitRevision> revisions)
        {
            if (_selectedRevisionUpdatedTargets.HasFlag(UpdateTargets.DiffList))
            {
                return;
            }

            _selectedRevisionUpdatedTargets |= UpdateTargets.DiffList;
        }

        private void FillCommitInfo(GitRevision revision)
        {
        }

        private Task FillGpgInfoAsync(GitRevision revision)
        {
            return Task.CompletedTask;
        }

        private void FillBuildReport(GitRevision revision)
        {
        }

        private void OpenToolStripMenuItemClick(object sender, EventArgs e)
        {
            GitModule module = FormOpenDirectory.OpenModule(this, null);
            if (module != null)
            {
                SetGitModule(this, new GitModuleEventArgs(module));
            }
        }

        private void CheckoutToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CloneToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CommitToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void InitNewRepositoryToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void PushToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void RefreshToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Broadcast RepoChanged in case repo was changed outside of GE
        }

        private void RefreshDashboardToolStripMenuItemClick(object sender, EventArgs e)
        {
            _dashboard.RefreshContent();
        }

        private void AboutToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var frm = new FormAbout())
            {
                frm.ShowDialog(this);
            }
        }

        private void PatchToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void ApplyPatchToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void userShell_Click(object sender, EventArgs e)
        {
            if (userShell.DropDownButtonPressed)
            {
                return;
            }

            IShellDescriptor shell = (sender as ToolStripItem)?.Tag as IShellDescriptor;
            if (shell is null)
            {
                return;
            }

            try
            {
                var executable = new Executable(shell.ExecutablePath, null);
                executable.Start(createWindow: true);
            }
            catch (Exception exception)
            {
                MessageBoxes.FailedToRunShell(this, shell.Name, exception);
            }
        }

        private void GitGuiToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void FormatPatchToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void GitcommandLogToolStripMenuItemClick(object sender, EventArgs e)
        {
            FormGitCommandLog.ShowOrActivate(this);
        }

        private void CheckoutBranchToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void StashToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void ResetToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void RunMergetoolToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CurrentBranchClick(object sender, EventArgs e)
        {
            branchSelect.ShowDropDown();
        }

        private void DeleteBranchToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void DeleteTagToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CherryPickToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void MergeBranchToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void OnShowSettingsClick(object sender, EventArgs e)
        {
        }

        private void TagToolStripMenuItemClick(object sender, EventArgs e)
        {
            var revision = RevisionGrid.LatestSelectedRevision;
        }

        private void KGitToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void DonateToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var frm = new FormDonate())
            {
                frm.ShowDialog(this);
            }
        }

        private static void SaveApplicationSettings()
        {
            AppSettings.SaveSettings();
        }

        private void EditGitignoreToolStripMenuItem1Click(object sender, EventArgs e)
        {
        }

        private void EditGitInfoExcludeToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void ArchiveToolStripMenuItemClick(object sender, EventArgs e)
        {
            var revisions = RevisionGrid.GetSelectedRevisions();
            if (revisions.Count < 1 || revisions.Count > 2)
            {
                MessageBoxes.SelectOnlyOneOrTwoRevisions(this);
                return;
            }

            GitRevision mainRevision = revisions.First();
            GitRevision diffRevision = null;
            if (revisions.Count == 2)
            {
                diffRevision = revisions.Last();
            }
        }

        private void EditMailMapToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void EditLocalGitConfigToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CompressGitDatabaseToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void recoverLostObjectsToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void ManageRemoteRepositoriesToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void RebaseToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void StartAuthenticationAgentToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void GenerateOrImportKeyToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CommitInfoTabControl_SelectedIndexChanged(object sender, EventArgs e)
        {
            RefreshSelection();
            FillTerminalTab();
        }

        private void ChangelogToolStripMenuItemClick(object sender, EventArgs e)
        {
            using (var frm = new FormChangeLog())
            {
                frm.ShowDialog(this);
            }
        }

        private void ToolStripButtonPushClick(object sender, EventArgs e)
        {
            PushToolStripMenuItemClick(sender, e);
        }

        private void ManageSubmodulesToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void UpdateSubmoduleToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void UpdateAllSubmodulesToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void SynchronizeAllSubmodulesToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void ToolStripSplitStashButtonClick(object sender, EventArgs e)
        {
        }

        private void StashChangesToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void StashPopToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void ManageStashesToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CreateStashToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void ExitToolStripMenuItemClick(object sender, EventArgs e)
        {
            Close();
        }

        private void tsmiFavouriteRepositories_DropDownOpening(object sender, EventArgs e)
        {
            tsmiFavouriteRepositories.DropDownItems.Clear();
            PopulateFavouriteRepositoriesMenu(tsmiFavouriteRepositories);
        }

        private void tsmiRecentRepositories_DropDownOpening(object sender, EventArgs e)
        {
            tsmiRecentRepositories.DropDownItems.Clear();
            PopulateRecentRepositoriesMenu(tsmiRecentRepositories);
            if (tsmiRecentRepositories.DropDownItems.Count < 1)
            {
                return;
            }

            tsmiRecentRepositories.DropDownItems.Add(clearRecentRepositoriesListToolStripMenuItem);
            tsmiRecentRepositories.DropDownItems.Add(tsmiRecentRepositoriesClear);
        }

        private void tsmiRecentRepositoriesClear_Click(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.Run(async () =>
            {
                var repositoryHistory = Array.Empty<Repository>();
                await RepositoryHistoryManager.Locals.SaveRecentHistoryAsync(repositoryHistory);

                await this.SwitchToMainThreadAsync();
                _dashboard?.RefreshContent();
            });
        }

        private void PluginSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void RepoSettingsToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CloseToolStripMenuItemClick(object sender, EventArgs e)
        {
            SetWorkingDir("");
        }

        private void UserManualToolStripMenuItemClick(object sender, EventArgs e)
        {
            // Point to the default documentation, will work also if the old doc version is removed
            OsShellUtil.OpenUrlInDefaultBrowser(@"https://git-extensions-documentation.readthedocs.org");
        }

        private void CleanupToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void PopulateFavouriteRepositoriesMenu(ToolStripDropDownItem container)
        {
            var repositoryHistory = ThreadHelper.JoinableTaskFactory.Run(() => RepositoryHistoryManager.Locals.LoadFavouriteHistoryAsync());
            if (repositoryHistory.Count < 1)
            {
                return;
            }

            PopulateFavouriteRepositoriesMenu(container, repositoryHistory);
        }

        private void PopulateFavouriteRepositoriesMenu(ToolStripDropDownItem container, in IList<Repository> repositoryHistory)
        {
            var mostRecentRepos = new List<RecentRepoInfo>();
            var lessRecentRepos = new List<RecentRepoInfo>();

            using (var graphics = CreateGraphics())
            {
                var splitter = new RecentRepoSplitter
                {
                    MeasureFont = container.Font,
                    Graphics = graphics
                };

                splitter.SplitRecentRepos(repositoryHistory, mostRecentRepos, lessRecentRepos);
            }

            foreach (var repo in mostRecentRepos.Union(lessRecentRepos).GroupBy(k => k.Repo.Category).OrderBy(k => k.Key))
            {
                AddFavouriteRepositories(repo.Key, repo.ToList());
            }

            void AddFavouriteRepositories(string category, IList<RecentRepoInfo> repos)
            {
                ToolStripMenuItem menuItemCategory;
                if (!container.DropDownItems.ContainsKey(category))
                {
                    menuItemCategory = new ToolStripMenuItem(category);
                    container.DropDownItems.Add(menuItemCategory);
                }
                else
                {
                    menuItemCategory = (ToolStripMenuItem)container.DropDownItems[category];
                }
            }
        }

        private void PopulateRecentRepositoriesMenu(ToolStripDropDownItem container)
        {
            var mostRecentRepos = new List<RecentRepoInfo>();
            var lessRecentRepos = new List<RecentRepoInfo>();

            var repositoryHistory = ThreadHelper.JoinableTaskFactory.Run(() => RepositoryHistoryManager.Locals.LoadRecentHistoryAsync());
            if (repositoryHistory.Count < 1)
            {
                return;
            }

            using (var graphics = CreateGraphics())
            {
                var splitter = new RecentRepoSplitter
                {
                    MeasureFont = container.Font,
                    Graphics = graphics
                };

                splitter.SplitRecentRepos(repositoryHistory, mostRecentRepos, lessRecentRepos);
            }
        }

        public void SetWorkingDir(string path)
        {
            SetGitModule(this, new GitModuleEventArgs(new GitModule(path)));
        }

        private void SetGitModule(object sender, GitModuleEventArgs e)
        {
            var module = e.GitModule;
            HideVariableMainMenuItems();
            RevisionGrid.InvalidateCount();
            _submoduleStatusProvider.Init();
        }

        private void TranslateToolStripMenuItemClick(object sender, EventArgs e)
        {
            OsShellUtil.OpenUrlInDefaultBrowser(@"https://github.com/gitextensions/gitextensions/wiki/Translations");
        }

        private void FileExplorerToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void CreateBranchToolStripMenuItemClick(object sender, EventArgs e)
        {
        }

        private void editGitAttributesToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        public static void CopyFullPathToClipboard(FileStatusList diffFiles, GitModule module)
        {
            if (!diffFiles.SelectedItems.Any())
            {
                return;
            }

            var fileNames = new StringBuilder();
            foreach (var item in diffFiles.SelectedItems)
            {
                var path = PathUtil.Combine(module.WorkingDir, item.Item.Name);
                if (string.IsNullOrWhiteSpace(path))
                {
                    continue;
                }

                // Only use append line when multiple items are selected.
                // This to make it easier to use the text from clipboard when 1 file is selected.
                if (fileNames.Length > 0)
                {
                    fileNames.AppendLine();
                }

                fileNames.Append(path.ToNativePath());
            }

            ClipboardUtil.TrySetText(fileNames.ToString());
        }

        private void deleteIndexLockToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
            }
            catch (FileDeleteException ex)
            {
                MessageBox.Show(this, $@"{_indexLockCantDelete.Text}: {ex.FileName}{Environment.NewLine}{ex.Message}", Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void BisectClick(object sender, EventArgs e)
        {
            using (var frm = new FormBisect(RevisionGrid))
            {
                frm.ShowDialog(this);
            }
        }

        private void CurrentBranchDropDownOpening(object sender, EventArgs e)
        {
            branchSelect.DropDownItems.Clear();

            AddCheckoutBranchMenuItem();
            branchSelect.DropDownItems.Add(new ToolStripSeparator());
            AddBranchesMenuItems();

            PreventToolStripSplitButtonClosing(sender as ToolStripSplitButton);

            void AddCheckoutBranchMenuItem()
            {
                var checkoutBranchItem = new ToolStripMenuItem(checkoutBranchToolStripMenuItem.Text, Images.BranchCheckout)
                {
                    ShortcutKeys = checkoutBranchToolStripMenuItem.ShortcutKeys,
                    ShortcutKeyDisplayString = checkoutBranchToolStripMenuItem.ShortcutKeyDisplayString
                };

                branchSelect.DropDownItems.Add(checkoutBranchItem);
                checkoutBranchItem.Click += CheckoutBranchToolStripMenuItemClick;
            }

            void AddBranchesMenuItems()
            {
            }
        }

        private void _forkCloneMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void _viewPullRequestsToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void _createPullRequestToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void _addUpstreamRemoteToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private bool TryGetRepositoryHost(out IRepositoryHostPlugin repoHost)
        {
            repoHost = PluginRegistry.TryGetGitHosterForModule(null);
            if (repoHost == null)
            {
                MessageBox.Show(this, _noReposHostFound.Text, Strings.Error, MessageBoxButtons.OK, MessageBoxIcon.Error);
                return false;
            }

            return true;
        }

        #region Hotkey commands

        public static readonly string HotkeySettingsName = "Browse";

        internal enum Command
        {
            GitBash = 0,
            GitGui = 1,
            GitGitK = 2,
            FocusRevisionGrid = 3,
            FocusCommitInfo = 4,
            FocusDiff = 5,
            FocusFileTree = 6,
            Commit = 7,
            AddNotes = 8,
            FindFileInSelectedCommit = 9,
            CheckoutBranch = 10,
            QuickFetch = 11,
            QuickPull = 12,
            QuickPush = 13,

            /* deprecated: RotateApplicationIcon = 14, */
            CloseRepository = 15,
            Stash = 16,
            StashPop = 17,
            FocusFilter = 18,
            OpenWithDifftool = 19,
            OpenSettings = 20,
            ToggleBranchTreePanel = 21,
            EditFile = 22,
            OpenAsTempFile = 23,
            OpenAsTempFileWith = 24,
            FocusBranchTree = 25,
            FocusGpgInfo = 26,
            GoToSuperproject = 27,
            GoToSubmodule = 28,
            FocusGitConsole = 29,
            FocusBuildServerStatus = 30,
            FocusNextTab = 31,
            FocusPrevTab = 32,
            OpenWithDifftoolFirstToLocal = 33,
            OpenWithDifftoolSelectedToLocal = 34,
            OpenCommitsWithDifftool = 35,
            ToggleBetweenArtificialAndHeadCommits = 36,
            GoToChild = 37,
            GoToParent = 38
        }

        private void AddNotes()
        {
            var revision = RevisionGrid.GetSelectedRevisions().FirstOrDefault();
            var objectId = revision?.ObjectId;

            if (objectId == null || objectId.IsArtificial)
            {
                return;
            }

            FillCommitInfo(revision);
        }

        private void FocusFilter()
        {
            ToolStripControlHost filterToFocus = toolStripRevisionFilterTextBox.Focused ? (ToolStripControlHost)toolStripBranchFilterComboBox : (ToolStripControlHost)toolStripRevisionFilterTextBox;
            filterToFocus.Focus();
        }

        private void FindFileInSelectedCommit()
        {
        }

        private void QuickFetch()
        {
        }

        #endregion

        public static void OpenContainingFolder(FileStatusList diffFiles, GitModule module)
        {
            if (!diffFiles.SelectedItems.Any())
            {
                return;
            }

            foreach (var item in diffFiles.SelectedItems)
            {
                string filePath = PathUtil.Combine(module.WorkingDir, item.Item.Name.ToNativePath());
                if (string.IsNullOrWhiteSpace(filePath))
                {
                    continue;
                }

                FormBrowseUtil.ShowFileOrParentFolderInFileExplorer(filePath);
            }
        }

        private void SetSplitterPositions()
        {
            _splitterManager.AddSplitter(RevisionsSplitContainer, nameof(RevisionsSplitContainer));
            _splitterManager.AddSplitter(MainSplitContainer, nameof(MainSplitContainer));
            _splitterManager.AddSplitter(RightSplitContainer, nameof(RightSplitContainer));

            _splitterManager.RestoreSplitters();
            RefreshLayoutToggleButtonStates();
        }

        private void CommandsToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            // Most options do not make sense for artificial commits or no revision selected at all
            var selectedRevisions = RevisionGrid.GetSelectedRevisions();
            bool singleNormalCommit = selectedRevisions.Count == 1 && !selectedRevisions[0].IsArtificial;

            // Some commands like stash, undo commit etc has no relation to selections

            // Require that a single commit is selected
            // Some commands like delete branch could be available for artificial as no default is used,
            // but hide for consistency
            branchToolStripMenuItem.Enabled =
            deleteBranchToolStripMenuItem.Enabled =
            mergeBranchToolStripMenuItem.Enabled =
            rebaseToolStripMenuItem.Enabled =
            checkoutBranchToolStripMenuItem.Enabled =
            cherryPickToolStripMenuItem.Enabled =
            checkoutToolStripMenuItem.Enabled =
            bisectToolStripMenuItem.Enabled =
                singleNormalCommit;

            tagToolStripMenuItem.Enabled =
            deleteTagToolStripMenuItem.Enabled =
            archiveToolStripMenuItem.Enabled =
                singleNormalCommit;

            // Not operating on selected revision
            commitToolStripMenuItem.Enabled =
            undoLastCommitToolStripMenuItem.Enabled =
            runMergetoolToolStripMenuItem.Enabled =
            stashToolStripMenuItem.Enabled =
            resetToolStripMenuItem.Enabled =
            cleanupToolStripMenuItem.Enabled =
            toolStripMenuItemReflog.Enabled =
            applyPatchToolStripMenuItem.Enabled = true;
        }

        private void PullToolStripMenuItemClick(object sender, EventArgs e)
        {
            // "Pull/Fetch..." menu item always opens the dialog
            DoPull(pullAction: AppSettings.FormPullAction, isSilent: false);
        }

        private void ToolStripButtonPullClick(object sender, EventArgs e)
        {
            // Clicking on the Pull button toolbar button will perform the default selected action silently,
            // except if that action is to open the dialog (PullAction.None)
            bool isSilent = AppSettings.DefaultPullAction != AppSettings.PullAction.None;
            var pullAction = AppSettings.DefaultPullAction != AppSettings.PullAction.None ?
                AppSettings.DefaultPullAction : AppSettings.FormPullAction;
            DoPull(pullAction: pullAction, isSilent: isSilent);
        }

        private void pullToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            // "Open Pull Dialog..." toolbar menu item always open the dialog with the current default action
            DoPull(pullAction: AppSettings.FormPullAction, isSilent: false);
        }

        private void mergeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.Merge, isSilent: true);
        }

        private void rebaseToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.Rebase, isSilent: true);
        }

        private void fetchToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.Fetch, isSilent: true);
        }

        private void fetchAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.FetchAll, isSilent: true);
        }

        private void fetchPruneAllToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DoPull(pullAction: AppSettings.PullAction.FetchPruneAll, isSilent: true);
        }

        private void DoPull(AppSettings.PullAction pullAction, bool isSilent)
        {
        }

        private void RefreshDefaultPullAction()
        {
            var defaultPullAction = AppSettings.DefaultPullAction;

            foreach (ToolStripMenuItem menuItem in setDefaultPullButtonActionToolStripMenuItem.DropDown.Items)
            {
                menuItem.Checked = (AppSettings.PullAction)menuItem.Tag == defaultPullAction;
            }

            switch (defaultPullAction)
            {
                case AppSettings.PullAction.Fetch:
                    toolStripButtonPull.Image = Images.PullFetch.AdaptLightness();
                    toolStripButtonPull.ToolTipText = _pullFetch.Text;
                    break;

                case AppSettings.PullAction.FetchAll:
                    toolStripButtonPull.Image = Images.PullFetchAll.AdaptLightness();
                    toolStripButtonPull.ToolTipText = _pullFetchAll.Text;
                    break;

                case AppSettings.PullAction.FetchPruneAll:
                    toolStripButtonPull.Image = Images.PullFetchPruneAll.AdaptLightness();
                    toolStripButtonPull.ToolTipText = _pullFetchPruneAll.Text;
                    break;

                case AppSettings.PullAction.Merge:
                    toolStripButtonPull.Image = Images.PullMerge.AdaptLightness();
                    toolStripButtonPull.ToolTipText = _pullMerge.Text;
                    break;

                case AppSettings.PullAction.Rebase:
                    toolStripButtonPull.Image = Images.PullRebase.AdaptLightness();
                    toolStripButtonPull.ToolTipText = _pullRebase.Text;
                    break;

                default:
                    toolStripButtonPull.Image = Images.Pull.AdaptLightness();
                    toolStripButtonPull.ToolTipText = _pullOpenDialog.Text;
                    break;
            }
        }

        private void branchSelect_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CheckoutBranchToolStripMenuItemClick(sender, e);
            }
        }

        private void RevisionInfo_CommandClicked(object sender, ResourceManager.CommandEventArgs e)
        {
        }

        private void SubmoduleToolStripButtonClick(object sender, EventArgs e)
        {
            if (sender is ToolStripMenuItem menuSender)
            {
                SetWorkingDir(menuSender.Tag as string);
            }
        }

        private void PreventToolStripSplitButtonClosing(ToolStripSplitButton control)
        {
            if (control == null || toolStripBranchFilterComboBox.Focused || toolStripRevisionFilterTextBox.Focused)
            {
                return;
            }

            control.Tag = this.FindFocusedControl();
            control.DropDownClosed += ToolStripSplitButtonDropDownClosed;
            toolStripBranchFilterComboBox.Focus();
        }

        private static void ToolStripSplitButtonDropDownClosed(object sender, EventArgs e)
        {
            if (sender is ToolStripSplitButton control)
            {
                control.DropDownClosed -= ToolStripSplitButtonDropDownClosed;

                if (control.Tag is Control controlToFocus)
                {
                    controlToFocus.Focus();
                    control.Tag = null;
                }
            }
        }

        private void toolStripButtonLevelUp_DropDownOpening(object sender, EventArgs e)
        {
            PreventToolStripSplitButtonClosing(sender as ToolStripSplitButton);
        }

        #region Submodules

        private (ToolStripItem item, Func<Task<Action>> loadDetails)
            CreateSubmoduleMenuItem(CancellationToken cancelToken, SubmoduleInfo info, string textFormat = "{0}")
        {
            var item = new ToolStripMenuItem(string.Format(textFormat, info.Text))
            {
                Width = 200,
                Tag = info.Path,
                Image = Images.FolderSubmodule
            };

            if (info.Bold)
            {
                item.Font = new Font(item.Font, FontStyle.Bold);
            }

            item.Click += SubmoduleToolStripButtonClick;

            Func<Task<Action>> loadDetails = null;
            if (info.Detailed != null)
            {
                item.Image = GetSubmoduleItemImage(info.Detailed);
                item.Text = string.Format(textFormat, info.Text + info.Detailed.AddedAndRemovedText);
            }

            return (item, loadDetails);

            Image GetSubmoduleItemImage(DetailedSubmoduleInfo details)
            {
                if (details.Status == null)
                {
                    return Images.FolderSubmodule;
                }

                if (details.Status == SubmoduleStatus.FastForward)
                {
                    return details.IsDirty ? Images.SubmoduleRevisionUpDirty : Images.SubmoduleRevisionUp;
                }

                if (details.Status == SubmoduleStatus.Rewind)
                {
                    return details.IsDirty ? Images.SubmoduleRevisionDownDirty : Images.SubmoduleRevisionDown;
                }

                if (details.Status == SubmoduleStatus.NewerTime)
                {
                    return details.IsDirty ? Images.SubmoduleRevisionSemiUpDirty : Images.SubmoduleRevisionSemiUp;
                }

                if (details.Status == SubmoduleStatus.OlderTime)
                {
                    return details.IsDirty ? Images.SubmoduleRevisionSemiDownDirty : Images.SubmoduleRevisionSemiDown;
                }

                return details.IsDirty ? Images.SubmoduleDirty : Images.FileStatusModified;
            }
        }

        private void UpdateSubmodulesStructure()
        {
        }

        private void SubmoduleStatusProvider_StatusUpdating(object sender, EventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await this.SwitchToMainThreadAsync();
                RemoveSubmoduleButtons();
                toolStripButtonLevelUp.DropDownItems.Add(_loading.Text);
            }).FileAndForget();
        }

        private void SubmoduleStatusProvider_StatusUpdated(object sender, SubmoduleStatusEventArgs e)
        {
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                await PopulateToolbarAsync(e.Info, e.Token);
            }).FileAndForget();
        }

        private async Task PopulateToolbarAsync(SubmoduleInfoResult result, CancellationToken cancelToken)
        {
            // Second task: Populate toolbar menu on UI thread.  Note further tasks are created by
            // CreateSubmoduleMenuItem to update images with submodule status.
            await this.SwitchToMainThreadAsync(cancelToken);

            RemoveSubmoduleButtons();

            var newItems = result.OurSubmodules
                .Select(submodule => CreateSubmoduleMenuItem(cancelToken, submodule))
                .ToList();

            if (result.OurSubmodules.Count == 0)
            {
                newItems.Add((new ToolStripMenuItem(_noSubmodulesPresent.Text), null));
            }

            if (result.SuperProject != null)
            {
                newItems.Add((new ToolStripSeparator(), null));

                // Show top project only if it's not our super project
                if (result.TopProject != null && result.TopProject != result.SuperProject)
                {
                    newItems.Add(CreateSubmoduleMenuItem(cancelToken, result.TopProject, _topProjectModuleFormat.Text));
                }

                newItems.Add(CreateSubmoduleMenuItem(cancelToken, result.SuperProject, _superprojectModuleFormat.Text));
                newItems.AddRange(result.AllSubmodules.Select(submodule => CreateSubmoduleMenuItem(cancelToken, submodule)));
                toolStripButtonLevelUp.ToolTipText = _goToSuperProject.Text;
            }

            newItems.Add((new ToolStripSeparator(), null));

            var mi = new ToolStripMenuItem(updateAllSubmodulesToolStripMenuItem.Text, Images.SubmodulesUpdate);
            mi.Click += UpdateAllSubmodulesToolStripMenuItemClick;
            newItems.Add((mi, null));

            if (result.CurrentSubmoduleName != null)
            {
                var item = new ToolStripMenuItem(_updateCurrentSubmodule.Text) { Tag = result.CurrentSubmoduleName };
                item.Click += UpdateSubmoduleToolStripMenuItemClick;
                newItems.Add((item, null));
            }

            // Using AddRange is critical: if you used Add to add menu items one at a
            // time, performance would be extremely slow with many submodules (> 100).
            toolStripButtonLevelUp.DropDownItems.AddRange(newItems.Select(e => e.item).ToArray());

            // Load details sequentially to not spawn too many background threads
            // then refresh all items at once with a single switch to the main thread
            ThreadHelper.JoinableTaskFactory.RunAsync(async () =>
            {
                var loadDetails = newItems.Select(e => e.loadDetails).Where(e => e != null);
                var refreshActions = new List<Action>();
                foreach (var loadFunc in loadDetails)
                {
                    cancelToken.ThrowIfCancellationRequested();
                    var action = await loadFunc();
                    refreshActions.Add(action);
                }

                await this.SwitchToMainThreadAsync(cancelToken);
                foreach (var refreshAction in refreshActions)
                {
                    refreshAction();
                }
            }).FileAndForget();
        }

        private void RemoveSubmoduleButtons()
        {
            foreach (var item in toolStripButtonLevelUp.DropDownItems)
            {
                if (item is ToolStripMenuItem toolStripButton)
                {
                    toolStripButton.Click -= SubmoduleToolStripButtonClick;
                }
            }

            toolStripButtonLevelUp.DropDownItems.Clear();
        }

        #endregion

        private void toolStripButtonLevelUp_ButtonClick(object sender, EventArgs e)
        {
        }

        private void reportAnIssueToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UserEnvironmentInformation.CopyInformation();
            OsShellUtil.OpenUrlInDefaultBrowser(@"https://github.com/gitextensions/gitextensions/issues");
        }

        private void checkForUpdatesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var updateForm = new FormUpdates(AppSettings.AppVersion);
            updateForm.SearchForUpdatesAndShow(Owner, true);
        }

        private void toolStripButtonPull_DropDownOpened(object sender, EventArgs e)
        {
            PreventToolStripSplitButtonClosing(sender as ToolStripSplitButton);
        }

        /// <summary>
        /// Adds a tab with console interface to Git over the current working copy. Recreates the terminal on tab activation if user exits the shell.
        /// </summary>
        private void FillTerminalTab()
        {
        }

        public void ChangeTerminalActiveFolder(string path)
        {
            string shellType = AppSettings.ConEmuTerminal.Value;
            IShellDescriptor shell = _shellProvider.GetShell(shellType);
        }

        private void menuitemSparseWorkingCopy_Click(object sender, EventArgs e)
        {
        }

        private void toolStripBranches_DropDown_ResizeDropDownWidth(object sender, EventArgs e)
        {
            toolStripBranchFilterComboBox.ComboBox.ResizeDropDownWidth(AppSettings.BranchDropDownMinWidth, AppSettings.BranchDropDownMaxWidth);
        }

        private void toolStripMenuItemReflog_Click(object sender, EventArgs e)
        {
        }

        #region Layout management

        private void toggleSplitViewLayout_Click(object sender, EventArgs e)
        {
            AppSettings.ShowSplitViewLayout = !AppSettings.ShowSplitViewLayout;
            DiagnosticsClient.TrackEvent("Layout change",
                new Dictionary<string, string> { { nameof(AppSettings.ShowSplitViewLayout), AppSettings.ShowSplitViewLayout.ToString() } });

            RefreshSplitViewLayout();
        }

        private void toggleBranchTreePanel_Click(object sender, EventArgs e)
        {
            MainSplitContainer.Panel1Collapsed = !MainSplitContainer.Panel1Collapsed;
            DiagnosticsClient.TrackEvent("Layout change",
                new Dictionary<string, string> { { "ShowLeftPanel", MainSplitContainer.Panel1Collapsed.ToString() } });

            RefreshLayoutToggleButtonStates();
        }

        private void CommitInfoPositionClick(object sender, EventArgs e)
        {
            if (!menuCommitInfoPosition.DropDownButtonPressed)
            {
                SetCommitInfoPosition((CommitInfoPosition)(
                    ((int)AppSettings.CommitInfoPosition + 1) %
                    Enum.GetValues(typeof(CommitInfoPosition)).Length));
            }
        }

        private void CommitInfoBelowClick(object sender, EventArgs e) =>
            SetCommitInfoPosition(CommitInfoPosition.BelowList);

        private void CommitInfoLeftwardClick(object sender, EventArgs e) =>
            SetCommitInfoPosition(CommitInfoPosition.LeftwardFromList);

        private void CommitInfoRightwardClick(object sender, EventArgs e) =>
            SetCommitInfoPosition(CommitInfoPosition.RightwardFromList);

        private void SetCommitInfoPosition(CommitInfoPosition position)
        {
            AppSettings.CommitInfoPosition = position;
            DiagnosticsClient.TrackEvent("Layout change",
                new Dictionary<string, string> { { nameof(AppSettings.CommitInfoPosition), AppSettings.CommitInfoPosition.ToString() } });

            LayoutRevisionInfo();
            RefreshLayoutToggleButtonStates();
        }

        private void RefreshSplitViewLayout()
        {
            RightSplitContainer.Panel2Collapsed = !AppSettings.ShowSplitViewLayout;
            DiagnosticsClient.TrackEvent("Layout change",
                new Dictionary<string, string> { { nameof(AppSettings.ShowSplitViewLayout), AppSettings.ShowSplitViewLayout.ToString() } });

            RefreshLayoutToggleButtonStates();
        }

        private void RefreshLayoutToggleButtonStates()
        {
            toggleBranchTreePanel.Checked = !MainSplitContainer.Panel1Collapsed;
            toggleSplitViewLayout.Checked = AppSettings.ShowSplitViewLayout;

            int commitInfoPositionNumber = (int)AppSettings.CommitInfoPosition;
            var selectedMenuItem = menuCommitInfoPosition.DropDownItems[commitInfoPositionNumber];
            menuCommitInfoPosition.Image = selectedMenuItem.Image;
            menuCommitInfoPosition.ToolTipText = selectedMenuItem.Text;
        }

        private void LayoutRevisionInfo()
        {
        }

        #endregion

        private void manageWorktreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void createWorktreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
        }

        private void toolStripSplitStash_DropDownOpened(object sender, EventArgs e)
        {
            PreventToolStripSplitButtonClosing(sender as ToolStripSplitButton);
        }

        private void toolStripBranchFilterComboBox_Click(object sender, EventArgs e)
        {
            if (toolStripBranchFilterComboBox.Items.Count == 0)
            {
                return;
            }

            toolStripBranchFilterComboBox.DroppedDown = true;
        }

        private void undoLastCommitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (AppSettings.DontConfirmUndoLastCommit || MessageBox.Show(this, _undoLastCommitText.Text, _undoLastCommitCaption.Text, MessageBoxButtons.YesNo, MessageBoxIcon.Warning) == DialogResult.Yes)
            {
                var args = GitCommandHelpers.ResetCmd(ResetMode.Soft, "HEAD~1");
                refreshToolStripMenuItem.PerformClick();
            }
        }

        internal TestAccessor GetTestAccessor()
            => new TestAccessor(this);

        internal readonly struct TestAccessor
        {
            private readonly FormBrowse _form;

            public TestAccessor(FormBrowse form)
            {
                _form = form;
            }

            public void PopulateFavouriteRepositoriesMenu(ToolStripDropDownItem container, IList<Repository> repositoryHistory)
            {
                _form.PopulateFavouriteRepositoriesMenu(container, repositoryHistory);
            }
        }

        private void FormBrowse_DragDrop(object sender, DragEventArgs e)
        {
        }

        private void FormBrowse_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop)
                || e.Data.GetDataPresent(DataFormats.Text)
                || e.Data.GetDataPresent(DataFormats.UnicodeText))
            {
                e.Effect = DragDropEffects.Move;
            }
        }

        private void TsmiTelemetryEnabled_Click(object sender, EventArgs e)
        {
        }

        private void HelpToolStripMenuItem_DropDownOpening(object sender, EventArgs e)
        {
            tsmiTelemetryEnabled.Checked = AppSettings.TelemetryEnabled ?? false;
        }
    }
}
