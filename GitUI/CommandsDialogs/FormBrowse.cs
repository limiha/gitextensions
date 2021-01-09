using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using GitCommands;
using GitCommands.Git;
using GitCommands.Git.Commands;
using GitCommands.UserRepositoryHistory;
using GitExtUtils;
using GitUI.CommandsDialogs.BrowseDialog;
using GitUI.Infrastructure.Telemetry;
using GitUI.Properties;
using GitUI.Shells;
using GitUIPluginInterfaces;
using JetBrains.Annotations;
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
        private readonly ShellProvider _shellProvider = new ShellProvider();

        [Flags]
        private enum UpdateTargets
        {
            None = 1,
            DiffList = 2,
            FileTree = 4,
            CommitInfo = 8
        }

        [Obsolete("For VS designer and translation test only. Do not remove.")]
        private FormBrowse()
        {
            InitializeComponent();
        }

        public FormBrowse([NotNull] GitUICommands commands, string filter, ObjectId selectCommit = null)
        {
            InitializeComponent();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // ReSharper disable ConstantConditionalAccessQualifier - these can be null if run from under the TranslatioApp

                components?.Dispose();

                // ReSharper restore ConstantConditionalAccessQualifier
            }

            base.Dispose(disposing);
        }

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

        #region IBrowseRepo

        #endregion

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
            mainMenuStrip.Refresh();
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

        private void branchSelect_MouseUp(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                CheckoutBranchToolStripMenuItemClick(sender, e);
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

        private void menuitemSparseWorkingCopy_Click(object sender, EventArgs e)
        {
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
