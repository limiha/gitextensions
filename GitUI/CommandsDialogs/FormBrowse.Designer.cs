using System.Windows.Forms;

namespace GitUI.CommandsDialogs
{
    partial class FormBrowse
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem2;
            System.Windows.Forms.ToolStripMenuItem toolStripMenuItem4;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator14;
            System.Windows.Forms.ToolStripSeparator toolStripSeparator11;
            this.ToolStripMain = new GitUI.ToolStripEx();
            this.RefreshButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator0 = new System.Windows.Forms.ToolStripSeparator();
            this.toggleBranchTreePanel = new System.Windows.Forms.ToolStripButton();
            this.toggleSplitViewLayout = new System.Windows.Forms.ToolStripButton();
            this.menuCommitInfoPosition = new System.Windows.Forms.ToolStripSplitButton();
            this.commitInfoBelowMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commitInfoLeftwardMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.commitInfoRightwardMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator17 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripButtonLevelUp = new System.Windows.Forms.ToolStripSplitButton();
            this._NO_TRANSLATE_WorkingDir = new System.Windows.Forms.ToolStripSplitButton();
            this.branchSelect = new System.Windows.Forms.ToolStripSplitButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSplitStash = new System.Windows.Forms.ToolStripSplitButton();
            this.stashChangesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.stashPopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator9 = new System.Windows.Forms.ToolStripSeparator();
            this.manageStashesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.createAStashToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonCommit = new System.Windows.Forms.ToolStripButton();
            this.toolStripButtonPull = new System.Windows.Forms.ToolStripSplitButton();
            this.mergeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.rebaseToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fetchToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pullToolStripMenuItem1 = new System.Windows.Forms.ToolStripMenuItem();
            this.fetchAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.fetchPruneAllToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.setDefaultPullButtonActionToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripButtonPush = new GitUI.CommandsDialogs.ToolStripPushButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripFileExplorer = new System.Windows.Forms.ToolStripButton();
            this.userShell = new System.Windows.Forms.ToolStripSplitButton();
            this.EditSettings = new System.Windows.Forms.ToolStripButton();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.toolStripBranchFilterComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.toolStripBranchFilterDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.toolStripSeparator19 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripRevisionFilterLabel = new System.Windows.Forms.ToolStripLabel();
            this.toolStripRevisionFilterTextBox = new System.Windows.Forms.ToolStripTextBox();
            this.toolStripRevisionFilterDropDownButton = new System.Windows.Forms.ToolStripDropDownButton();
            this.ShowFirstParent = new System.Windows.Forms.ToolStripButton();
            this.MainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.RightSplitContainer = new System.Windows.Forms.SplitContainer();
            this.RevisionsSplitContainer = new System.Windows.Forms.SplitContainer();
            this.FilterToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.fileToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initNewRepositoryToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.openToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiFavouriteRepositories = new System.Windows.Forms.ToolStripMenuItem();
            this.tsmiRecentRepositories = new System.Windows.Forms.ToolStripMenuItem();
            this.clearRecentRepositoriesListToolStripMenuItem = new System.Windows.Forms.ToolStripSeparator();
            this.tsmiRecentRepositoriesClear = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator12 = new System.Windows.Forms.ToolStripSeparator();
            this.cloneToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripMenuItem1 = new System.Windows.Forms.ToolStripSeparator();
            this.exitToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.refreshDashboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator46 = new System.Windows.Forms.ToolStripSeparator();
            this.toolStripSeparator42 = new System.Windows.Forms.ToolStripSeparator();
            this.dashboardToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainMenuStrip = new GitUI.MenuStripEx();
            this.gitItemBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.gitRevisionBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.toolPanel = new System.Windows.Forms.ToolStripContainer();
            this.ToolStripFilters = new GitUI.ToolStripEx();
            toolStripMenuItem2 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripMenuItem4 = new System.Windows.Forms.ToolStripMenuItem();
            toolStripSeparator14 = new System.Windows.Forms.ToolStripSeparator();
            toolStripSeparator11 = new System.Windows.Forms.ToolStripSeparator();
            this.ToolStripMain.SuspendLayout();
            this.ToolStripFilters.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).BeginInit();
            this.MainSplitContainer.Panel1.SuspendLayout();
            this.MainSplitContainer.Panel2.SuspendLayout();
            this.MainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RightSplitContainer)).BeginInit();
            this.RightSplitContainer.Panel1.SuspendLayout();
            this.RightSplitContainer.Panel2.SuspendLayout();
            this.RightSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.RevisionsSplitContainer)).BeginInit();
            this.RevisionsSplitContainer.Panel1.SuspendLayout();
            this.RevisionsSplitContainer.SuspendLayout();
            this.mainMenuStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gitItemBindingSource)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.gitRevisionBindingSource)).BeginInit();
            this.toolPanel.ContentPanel.SuspendLayout();
            this.toolPanel.TopToolStripPanel.SuspendLayout();
            this.toolPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // toolStripMenuItem2
            // 
            toolStripMenuItem2.Name = "toolStripMenuItem2";
            toolStripMenuItem2.Size = new System.Drawing.Size(122, 22);
            toolStripMenuItem2.Text = "...";
            // 
            // toolStripMenuItem4
            // 
            toolStripMenuItem4.Name = "toolStripMenuItem4";
            toolStripMenuItem4.Size = new System.Drawing.Size(83, 22);
            toolStripMenuItem4.Text = "...";
            // 
            // toolStripSeparator14
            // 
            toolStripSeparator14.Name = "toolStripSeparator14";
            toolStripSeparator14.Size = new System.Drawing.Size(225, 6);
            // 
            // ToolStripMain
            // 
            this.ToolStripMain.ClickThrough = true;
            this.ToolStripMain.Dock = System.Windows.Forms.DockStyle.None;
            this.ToolStripMain.GripMargin = new System.Windows.Forms.Padding(0);
            this.ToolStripMain.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStripMain.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ToolStripMain.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.RefreshButton,
            this.toolStripSeparator0,
            this.toggleBranchTreePanel,
            this.toggleSplitViewLayout,
            this.menuCommitInfoPosition,
            this.toolStripSeparator17,
            this.toolStripButtonLevelUp,
            this._NO_TRANSLATE_WorkingDir,
            this.branchSelect,
            this.toolStripSeparator1,
            this.toolStripButtonPull,
            this.toolStripButtonPush,
            this.toolStripButtonCommit,
            this.toolStripSplitStash,
            this.toolStripSeparator2,
            this.toolStripFileExplorer,
            this.userShell,
            this.EditSettings});
            this.ToolStripMain.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ToolStripMain.Location = new System.Drawing.Point(3, 0);
            this.ToolStripMain.Name = "ToolStripMain";
            this.ToolStripMain.Padding = new System.Windows.Forms.Padding(0);
            this.ToolStripMain.Size = new System.Drawing.Size(601, 25);
            this.ToolStripMain.TabIndex = 0;
            this.ToolStripMain.Text = "Standard";
            // 
            // ToolStripFilters
            // 
            this.ToolStripFilters.ClickThrough = true;
            this.ToolStripFilters.Dock = System.Windows.Forms.DockStyle.None;
            this.ToolStripFilters.GripMargin = new System.Windows.Forms.Padding(0);
            this.ToolStripFilters.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.ToolStripFilters.ImeMode = System.Windows.Forms.ImeMode.NoControl;
            this.ToolStripFilters.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.toolStripBranchFilterComboBox,
            this.toolStripBranchFilterDropDownButton,
            this.toolStripSeparator19,
            this.toolStripRevisionFilterLabel,
            this.toolStripRevisionFilterTextBox,
            this.toolStripRevisionFilterDropDownButton,
            this.ShowFirstParent});
            this.ToolStripFilters.LayoutStyle = System.Windows.Forms.ToolStripLayoutStyle.HorizontalStackWithOverflow;
            this.ToolStripFilters.Location = new System.Drawing.Point(604, 0);
            this.ToolStripFilters.Name = "ToolStripFilters";
            this.ToolStripFilters.Padding = new System.Windows.Forms.Padding(0);
            this.ToolStripFilters.Size = new System.Drawing.Size(319, 25);
            this.ToolStripFilters.TabIndex = 1;
            this.ToolStripFilters.Text = "Filters";
            // 
            // RefreshButton
            // 
            this.RefreshButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.RefreshButton.Image = global::GitUI.Properties.Images.ReloadRevisions;
            this.RefreshButton.ImageTransparentColor = System.Drawing.Color.White;
            this.RefreshButton.Name = "RefreshButton";
            this.RefreshButton.Size = new System.Drawing.Size(23, 22);
            this.RefreshButton.ToolTipText = "Refresh";
            this.RefreshButton.Click += new System.EventHandler(this.RefreshToolStripMenuItemClick);
            // 
            // toolStripSeparator0
            // 
            this.toolStripSeparator0.Name = "toolStripSeparator0";
            this.toolStripSeparator0.Size = new System.Drawing.Size(6, 25);
            // 
            // toggleBranchTreePanel
            // 
            this.toggleBranchTreePanel.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleBranchTreePanel.Image = global::GitUI.Properties.Images.LayoutSidebarLeft;
            this.toggleBranchTreePanel.Name = "toggleBranchTreePanel";
            this.toggleBranchTreePanel.Size = new System.Drawing.Size(23, 22);
            this.toggleBranchTreePanel.ToolTipText = "Toggle left panel";
            this.toggleBranchTreePanel.Click += new System.EventHandler(this.toggleBranchTreePanel_Click);
            // 
            // toggleSplitViewLayout
            // 
            this.toggleSplitViewLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toggleSplitViewLayout.Image = global::GitUI.Properties.Images.LayoutFooter;
            this.toggleSplitViewLayout.Name = "toggleSplitViewLayout";
            this.toggleSplitViewLayout.Size = new System.Drawing.Size(23, 22);
            this.toggleSplitViewLayout.ToolTipText = "Toggle split view layout";
            this.toggleSplitViewLayout.Click += new System.EventHandler(this.toggleSplitViewLayout_Click);
            // 
            // menuCommitInfoPosition
            // 
            this.menuCommitInfoPosition.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.menuCommitInfoPosition.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.commitInfoBelowMenuItem,
            this.commitInfoLeftwardMenuItem,
            this.commitInfoRightwardMenuItem});
            this.menuCommitInfoPosition.Image = global::GitUI.Properties.Images.LayoutFooterTab;
            this.menuCommitInfoPosition.Name = "menuCommitInfoPosition";
            this.menuCommitInfoPosition.Size = new System.Drawing.Size(32, 22);
            this.menuCommitInfoPosition.ToolTipText = "Commit info position";
            this.menuCommitInfoPosition.Click += new System.EventHandler(this.CommitInfoPositionClick);
            // 
            // commitInfoBelowMenuItem
            // 
            this.commitInfoBelowMenuItem.Image = global::GitUI.Properties.Images.LayoutFooterTab;
            this.commitInfoBelowMenuItem.Name = "commitInfoBelowMenuItem";
            this.commitInfoBelowMenuItem.Size = new System.Drawing.Size(218, 22);
            this.commitInfoBelowMenuItem.Text = "Commit info below graph";
            this.commitInfoBelowMenuItem.Click += new System.EventHandler(this.CommitInfoBelowClick);
            // 
            // commitInfoLeftwardMenuItem
            // 
            this.commitInfoLeftwardMenuItem.Image = global::GitUI.Properties.Images.LayoutSidebarTopLeft;
            this.commitInfoLeftwardMenuItem.Name = "commitInfoLeftwardMenuItem";
            this.commitInfoLeftwardMenuItem.Size = new System.Drawing.Size(218, 22);
            this.commitInfoLeftwardMenuItem.Text = "Commit info left of graph";
            this.commitInfoLeftwardMenuItem.Click += new System.EventHandler(this.CommitInfoLeftwardClick);
            // 
            // commitInfoRightwardMenuItem
            // 
            this.commitInfoRightwardMenuItem.Image = global::GitUI.Properties.Images.LayoutSidebarTopRight;
            this.commitInfoRightwardMenuItem.Name = "commitInfoRightwardMenuItem";
            this.commitInfoRightwardMenuItem.Size = new System.Drawing.Size(218, 22);
            this.commitInfoRightwardMenuItem.Text = "Commit info right of graph";
            this.commitInfoRightwardMenuItem.Click += new System.EventHandler(this.CommitInfoRightwardClick);
            // 
            // toolStripSeparator17
            // 
            this.toolStripSeparator17.Name = "toolStripSeparator17";
            this.toolStripSeparator17.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripButtonLevelUp
            // 
            this.toolStripButtonLevelUp.AutoToolTip = false;
            this.toolStripButtonLevelUp.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonLevelUp.Image = global::GitUI.Properties.Images.SubmodulesManage;
            this.toolStripButtonLevelUp.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonLevelUp.Name = "toolStripButtonLevelUp";
            this.toolStripButtonLevelUp.Size = new System.Drawing.Size(32, 22);
            this.toolStripButtonLevelUp.ButtonClick += new System.EventHandler(this.toolStripButtonLevelUp_ButtonClick);
            this.toolStripButtonLevelUp.DropDownOpening += new System.EventHandler(this.toolStripButtonLevelUp_DropDownOpening);
            // 
            // _NO_TRANSLATE_WorkingDir
            // 
            this._NO_TRANSLATE_WorkingDir.Image = global::GitUI.Properties.Resources.RepoOpen;
            this._NO_TRANSLATE_WorkingDir.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._NO_TRANSLATE_WorkingDir.ImageTransparentColor = System.Drawing.Color.Magenta;
            this._NO_TRANSLATE_WorkingDir.Name = "_NO_TRANSLATE_WorkingDir";
            this._NO_TRANSLATE_WorkingDir.Size = new System.Drawing.Size(99, 22);
            this._NO_TRANSLATE_WorkingDir.Text = "WorkingDir";
            this._NO_TRANSLATE_WorkingDir.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this._NO_TRANSLATE_WorkingDir.ToolTipText = "Change working directory";
            this._NO_TRANSLATE_WorkingDir.ButtonClick += new System.EventHandler(this.WorkingDirClick);
            this._NO_TRANSLATE_WorkingDir.DropDownOpening += new System.EventHandler(this.WorkingDirDropDownOpening);
            this._NO_TRANSLATE_WorkingDir.MouseUp += new System.Windows.Forms.MouseEventHandler(this._NO_TRANSLATE_WorkingDir_MouseUp);
            // 
            // branchSelect
            // 
            this.branchSelect.Image = global::GitUI.Properties.Resources.branch;
            this.branchSelect.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.branchSelect.Name = "branchSelect";
            this.branchSelect.Size = new System.Drawing.Size(76, 22);
            this.branchSelect.Text = "Branch";
            this.branchSelect.ToolTipText = "Change current branch";
            this.branchSelect.ButtonClick += new System.EventHandler(this.CurrentBranchClick);
            this.branchSelect.DropDownOpening += new System.EventHandler(this.CurrentBranchDropDownOpening);
            this.branchSelect.MouseUp += new System.Windows.Forms.MouseEventHandler(this.branchSelect_MouseUp);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripSplitStash
            // 
            this.toolStripSplitStash.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.stashChangesToolStripMenuItem,
            this.stashPopToolStripMenuItem,
            this.toolStripSeparator9,
            this.manageStashesToolStripMenuItem,
            this.createAStashToolStripMenuItem});
            this.toolStripSplitStash.Image = global::GitUI.Properties.Images.Stash;
            this.toolStripSplitStash.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripSplitStash.Name = "toolStripSplitStash";
            this.toolStripSplitStash.Size = new System.Drawing.Size(32, 22);
            this.toolStripSplitStash.ToolTipText = "Manage stashes";
            this.toolStripSplitStash.ButtonClick += new System.EventHandler(this.ToolStripSplitStashButtonClick);
            this.toolStripSplitStash.DropDownOpened += new System.EventHandler(this.toolStripSplitStash_DropDownOpened);
            // 
            // stashChangesToolStripMenuItem
            // 
            this.stashChangesToolStripMenuItem.Name = "stashChangesToolStripMenuItem";
            this.stashChangesToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.stashChangesToolStripMenuItem.Text = "Stash";
            this.stashChangesToolStripMenuItem.ToolTipText = "Stash changes";
            this.stashChangesToolStripMenuItem.Click += new System.EventHandler(this.StashChangesToolStripMenuItemClick);
            // 
            // stashPopToolStripMenuItem
            // 
            this.stashPopToolStripMenuItem.Name = "stashPopToolStripMenuItem";
            this.stashPopToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.stashPopToolStripMenuItem.Text = "Stash pop";
            this.stashPopToolStripMenuItem.ToolTipText = "Apply and drop single stash";
            this.stashPopToolStripMenuItem.Click += new System.EventHandler(this.StashPopToolStripMenuItemClick);
            // 
            // toolStripSeparator9
            // 
            this.toolStripSeparator9.Name = "toolStripSeparator9";
            this.toolStripSeparator9.Size = new System.Drawing.Size(164, 6);
            // 
            // manageStashesToolStripMenuItem
            // 
            this.manageStashesToolStripMenuItem.Name = "manageStashesToolStripMenuItem";
            this.manageStashesToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.manageStashesToolStripMenuItem.Text = "Manage stashes...";
            this.manageStashesToolStripMenuItem.ToolTipText = "Manage stashes";
            this.manageStashesToolStripMenuItem.Click += new System.EventHandler(this.ManageStashesToolStripMenuItemClick);
            // 
            // createAStashToolStripMenuItem
            // 
            this.createAStashToolStripMenuItem.Name = "createAStashToolStripMenuItem";
            this.createAStashToolStripMenuItem.Size = new System.Drawing.Size(167, 22);
            this.createAStashToolStripMenuItem.Text = "Create a stash...";
            this.createAStashToolStripMenuItem.Click += new System.EventHandler(this.CreateStashToolStripMenuItemClick);
            // 
            // toolStripButtonCommit
            // 
            this.toolStripButtonCommit.Image = global::GitUI.Properties.Images.RepoStateClean;
            this.toolStripButtonCommit.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonCommit.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonCommit.Name = "toolStripButtonCommit";
            this.toolStripButtonCommit.Size = new System.Drawing.Size(71, 22);
            this.toolStripButtonCommit.Text = "Commit";
            this.toolStripButtonCommit.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.toolStripButtonCommit.Click += new System.EventHandler(this.CommitToolStripMenuItemClick);
            // 
            // toolStripButtonPull
            // 
            this.toolStripButtonPull.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPull.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.pullToolStripMenuItem1,
            toolStripSeparator11,
            this.mergeToolStripMenuItem,
            this.rebaseToolStripMenuItem1,
            this.fetchToolStripMenuItem,
            this.fetchAllToolStripMenuItem,
            this.fetchPruneAllToolStripMenuItem,
            toolStripSeparator14,
            this.setDefaultPullButtonActionToolStripMenuItem});
            this.toolStripButtonPull.Image = global::GitUI.Properties.Images.Pull;
            this.toolStripButtonPull.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPull.Name = "toolStripButtonPull";
            this.toolStripButtonPull.Size = new System.Drawing.Size(32, 22);
            this.toolStripButtonPull.Text = "Pull";
            this.toolStripButtonPull.ButtonClick += new System.EventHandler(this.ToolStripButtonPullClick);
            this.toolStripButtonPull.DropDownOpened += new System.EventHandler(this.toolStripButtonPull_DropDownOpened);
            // 
            // toolStripSeparator11
            // 
            toolStripSeparator11.Name = "toolStripSeparator11";
            toolStripSeparator11.Size = new System.Drawing.Size(225, 6);
            // 
            // mergeToolStripMenuItem
            // 
            this.mergeToolStripMenuItem.Image = global::GitUI.Properties.Images.PullMerge;
            this.mergeToolStripMenuItem.Name = "mergeToolStripMenuItem";
            this.mergeToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.mergeToolStripMenuItem.Text = "Pull - merge";
            this.mergeToolStripMenuItem.Click += new System.EventHandler(this.mergeToolStripMenuItem_Click);
            // 
            // rebaseToolStripMenuItem1
            // 
            this.rebaseToolStripMenuItem1.Image = global::GitUI.Properties.Images.PullRebase;
            this.rebaseToolStripMenuItem1.Name = "rebaseToolStripMenuItem1";
            this.rebaseToolStripMenuItem1.Size = new System.Drawing.Size(228, 22);
            this.rebaseToolStripMenuItem1.Text = "Pull - rebase";
            this.rebaseToolStripMenuItem1.Click += new System.EventHandler(this.rebaseToolStripMenuItem1_Click);
            // 
            // fetchToolStripMenuItem
            // 
            this.fetchToolStripMenuItem.Image = global::GitUI.Properties.Images.PullFetch;
            this.fetchToolStripMenuItem.Name = "fetchToolStripMenuItem";
            this.fetchToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.fetchToolStripMenuItem.Text = "Fetch";
            this.fetchToolStripMenuItem.ToolTipText = "Fetch branches and tags";
            this.fetchToolStripMenuItem.Click += new System.EventHandler(this.fetchToolStripMenuItem_Click);
            // 
            // pullToolStripMenuItem1
            // 
            this.pullToolStripMenuItem1.Image = global::GitUI.Properties.Images.Pull;
            this.pullToolStripMenuItem1.Name = "pullToolStripMenuItem1";
            this.pullToolStripMenuItem1.Size = new System.Drawing.Size(228, 22);
            this.pullToolStripMenuItem1.Text = "Open pull dialog...";
            this.pullToolStripMenuItem1.Click += new System.EventHandler(this.pullToolStripMenuItem1_Click);
            // 
            // fetchAllToolStripMenuItem
            // 
            this.fetchAllToolStripMenuItem.Image = global::GitUI.Properties.Images.PullFetchAll;
            this.fetchAllToolStripMenuItem.Name = "fetchAllToolStripMenuItem";
            this.fetchAllToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.fetchAllToolStripMenuItem.Text = "Fetch all";
            this.fetchAllToolStripMenuItem.ToolTipText = "Fetch branches and tags from all remote repositories";
            this.fetchAllToolStripMenuItem.Click += new System.EventHandler(this.fetchAllToolStripMenuItem_Click);
            // 
            // fetchPruneAllToolStripMenuItem
            // 
            this.fetchPruneAllToolStripMenuItem.Image = global::GitUI.Properties.Images.PullFetchPruneAll;
            this.fetchPruneAllToolStripMenuItem.Name = "fetchPruneAllToolStripMenuItem";
            this.fetchPruneAllToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.fetchPruneAllToolStripMenuItem.Text = "Fetch and prune all";
            this.fetchPruneAllToolStripMenuItem.ToolTipText = "Fetch branches and tags from all remote repositories also prune deleted refs";
            this.fetchPruneAllToolStripMenuItem.Click += new System.EventHandler(this.fetchPruneAllToolStripMenuItem_Click);
            // 
            // setDefaultPullButtonActionToolStripMenuItem
            // 
            this.setDefaultPullButtonActionToolStripMenuItem.Name = "setDefaultPullButtonActionToolStripMenuItem";
            this.setDefaultPullButtonActionToolStripMenuItem.Size = new System.Drawing.Size(228, 22);
            this.setDefaultPullButtonActionToolStripMenuItem.Text = "Set default Pull button action";
            // 
            // toolStripButtonPush
            // 
            this.toolStripButtonPush.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripButtonPush.Image = global::GitUI.Properties.Images.Push;
            this.toolStripButtonPush.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripButtonPush.Name = "toolStripButtonPush";
            this.toolStripButtonPush.Size = new System.Drawing.Size(23, 22);
            this.toolStripButtonPush.Text = "Push";
            this.toolStripButtonPush.Click += new System.EventHandler(this.ToolStripButtonPushClick);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripFileExplorer
            // 
            this.toolStripFileExplorer.Enabled = false;
            this.toolStripFileExplorer.Image = global::GitUI.Properties.Images.BrowseFileExplorer;
            this.toolStripFileExplorer.ImageTransparentColor = System.Drawing.Color.Gray;
            this.toolStripFileExplorer.Name = "toolStripFileExplorer";
            this.toolStripFileExplorer.Size = new System.Drawing.Size(23, 22);
            this.toolStripFileExplorer.ToolTipText = "File Explorer";
            this.toolStripFileExplorer.Click += new System.EventHandler(this.FileExplorerToolStripMenuItemClick);
            // 
            // userShell
            // 
            this.userShell.Image = global::GitUI.Properties.Images.GitForWindows;
            this.userShell.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.userShell.Name = "userShell";
            this.userShell.Size = new System.Drawing.Size(32, 22);
            this.userShell.ToolTipText = "Git bash";
            this.userShell.Click += new System.EventHandler(this.userShell_Click);
            // 
            // EditSettings
            // 
            this.EditSettings.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.EditSettings.Image = global::GitUI.Properties.Images.Settings;
            this.EditSettings.Name = "EditSettings";
            this.EditSettings.Size = new System.Drawing.Size(23, 22);
            this.EditSettings.ToolTipText = "Settings";
            this.EditSettings.Click += new System.EventHandler(this.OnShowSettingsClick);
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(58, 22);
            this.toolStripLabel1.Text = "Branches:";
            // 
            // toolStripBranchFilterComboBox
            // 
            this.toolStripBranchFilterComboBox.AutoSize = false;
            this.toolStripBranchFilterComboBox.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripBranchFilterComboBox.DropDownWidth = 300;
            this.toolStripBranchFilterComboBox.FlatStyle = System.Windows.Forms.FlatStyle.System;
            this.toolStripBranchFilterComboBox.Name = "toolStripBranchFilterComboBox";
            this.toolStripBranchFilterComboBox.Size = new System.Drawing.Size(100, 23);
            this.toolStripBranchFilterComboBox.Click += new System.EventHandler(this.toolStripBranchFilterComboBox_Click);
            // 
            // toolStripBranchFilterDropDownButton
            // 
            this.toolStripBranchFilterDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripBranchFilterDropDownButton.Image = global::GitUI.Properties.Images.EditFilter;
            this.toolStripBranchFilterDropDownButton.Name = "toolStripBranchFilterDropDownButton";
            this.toolStripBranchFilterDropDownButton.Size = new System.Drawing.Size(29, 22);
            // 
            // toolStripSeparator19
            // 
            this.toolStripSeparator19.Name = "toolStripSeparator19";
            this.toolStripSeparator19.Size = new System.Drawing.Size(6, 25);
            // 
            // toolStripRevisionFilterLabel
            // 
            this.toolStripRevisionFilterLabel.Name = "toolStripRevisionFilterLabel";
            this.toolStripRevisionFilterLabel.Size = new System.Drawing.Size(36, 22);
            this.toolStripRevisionFilterLabel.Text = "Filter:";
            // 
            // toolStripRevisionFilterTextBox
            // 
            this.toolStripRevisionFilterTextBox.BackColor = System.Drawing.SystemColors.Control;
            this.toolStripRevisionFilterTextBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.toolStripRevisionFilterTextBox.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.toolStripRevisionFilterTextBox.ForeColor = System.Drawing.SystemColors.ControlText;
            this.toolStripRevisionFilterTextBox.Name = "toolStripRevisionFilterTextBox";
            this.toolStripRevisionFilterTextBox.Size = new System.Drawing.Size(100, 23);
            // 
            // toolStripRevisionFilterDropDownButton
            // 
            this.toolStripRevisionFilterDropDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.toolStripRevisionFilterDropDownButton.Image = global::GitUI.Properties.Images.EditFilter;
            this.toolStripRevisionFilterDropDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.toolStripRevisionFilterDropDownButton.Name = "toolStripRevisionFilterDropDownButton";
            this.toolStripRevisionFilterDropDownButton.Size = new System.Drawing.Size(29, 20);
            // 
            // ShowFirstParent
            // 
            this.ShowFirstParent.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.ShowFirstParent.Image = global::GitUI.Properties.Images.ShowFirstParent;
            this.ShowFirstParent.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.ShowFirstParent.Name = "ShowFirstParent";
            this.ShowFirstParent.Size = new System.Drawing.Size(23, 20);
            this.ShowFirstParent.ToolTipText = "Show first parents";
            // 
            // MainSplitContainer
            // 
            this.MainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.MainSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            this.MainSplitContainer.Location = new System.Drawing.Point(8, 8);
            this.MainSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.MainSplitContainer.Name = "MainSplitContainer";
            // 
            // MainSplitContainer.Panel1
            // 
            this.MainSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(1);
            this.MainSplitContainer.Panel1MinSize = 192;
            // 
            // MainSplitContainer.Panel2
            // 
            this.MainSplitContainer.Panel2.Controls.Add(this.RightSplitContainer);
            this.MainSplitContainer.Size = new System.Drawing.Size(907, 505);
            this.MainSplitContainer.SplitterDistance = 192;
            this.MainSplitContainer.SplitterWidth = 6;
            this.MainSplitContainer.TabIndex = 1;
            // 
            // RightSplitContainer
            // 
            this.RightSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RightSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.RightSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.RightSplitContainer.Name = "RightSplitContainer";
            this.RightSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // RightSplitContainer.Panel1
            // 
            this.RightSplitContainer.Panel1.Controls.Add(this.RevisionsSplitContainer);
            // 
            // RightSplitContainer.Panel2
            // 
            this.RightSplitContainer.Panel2MinSize = 0;
            this.RightSplitContainer.Size = new System.Drawing.Size(650, 502);
            this.RightSplitContainer.SplitterDistance = 209;
            this.RightSplitContainer.SplitterWidth = 6;
            this.RightSplitContainer.TabIndex = 1;
            this.RightSplitContainer.TabStop = false;
            // 
            // RevisionsSplitContainer
            // 
            this.RevisionsSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.RevisionsSplitContainer.FixedPanel = System.Windows.Forms.FixedPanel.Panel2;
            this.RevisionsSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.RevisionsSplitContainer.Margin = new System.Windows.Forms.Padding(0);
            this.RevisionsSplitContainer.Name = "RevisionsSplitContainer";
            this.RevisionsSplitContainer.Panel1.Padding = new System.Windows.Forms.Padding(1);
            this.RevisionsSplitContainer.Panel2.Padding = new System.Windows.Forms.Padding(1);
            // 
            // RevisionsSplitContainer.Panel1
            // 
            this.RevisionsSplitContainer.Size = new System.Drawing.Size(650, 209);
            this.RevisionsSplitContainer.SplitterDistance = 350;
            this.RevisionsSplitContainer.SplitterWidth = 6;
            this.RevisionsSplitContainer.TabIndex = 0;
            // 
            // FilterToolTip
            // 
            this.FilterToolTip.AutomaticDelay = 100;
            this.FilterToolTip.ShowAlways = true;
            this.FilterToolTip.ToolTipIcon = System.Windows.Forms.ToolTipIcon.Error;
            this.FilterToolTip.ToolTipTitle = "RegEx";
            this.FilterToolTip.UseAnimation = false;
            this.FilterToolTip.UseFading = false;
            // 
            // fileToolStripMenuItem
            // 
            this.fileToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.initNewRepositoryToolStripMenuItem,
            this.openToolStripMenuItem,
            this.tsmiFavouriteRepositories,
            this.tsmiRecentRepositories,
            this.toolStripSeparator12,
            this.cloneToolStripMenuItem,
            this.toolStripMenuItem1,
            this.exitToolStripMenuItem});
            this.fileToolStripMenuItem.Name = "fileToolStripMenuItem";
            this.fileToolStripMenuItem.Size = new System.Drawing.Size(43, 19);
            this.fileToolStripMenuItem.Text = "&Start";
            // 
            // initNewRepositoryToolStripMenuItem
            // 
            this.initNewRepositoryToolStripMenuItem.Image = global::GitUI.Properties.Images.RepoCreate;
            this.initNewRepositoryToolStripMenuItem.Name = "initNewRepositoryToolStripMenuItem";
            this.initNewRepositoryToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.initNewRepositoryToolStripMenuItem.Text = "Create new repository...";
            this.initNewRepositoryToolStripMenuItem.Click += new System.EventHandler(this.InitNewRepositoryToolStripMenuItemClick);
            // 
            // openToolStripMenuItem
            // 
            this.openToolStripMenuItem.Image = global::GitUI.Properties.Images.RepoOpen;
            this.openToolStripMenuItem.Name = "openToolStripMenuItem";
            this.openToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
            this.openToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.openToolStripMenuItem.Text = "Open...";
            this.openToolStripMenuItem.Click += new System.EventHandler(this.OpenToolStripMenuItemClick);
            // 
            // tsmiFavouriteRepositories
            // 
            this.tsmiFavouriteRepositories.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem4});
            this.tsmiFavouriteRepositories.Image = global::GitUI.Properties.Images.Star;
            this.tsmiFavouriteRepositories.Name = "tsmiFavouriteRepositories";
            this.tsmiFavouriteRepositories.Size = new System.Drawing.Size(198, 22);
            this.tsmiFavouriteRepositories.Text = "Favourite Repositories";
            this.tsmiFavouriteRepositories.DropDownOpening += new System.EventHandler(this.tsmiFavouriteRepositories_DropDownOpening);
            // 
            // tsmiRecentRepositories
            // 
            this.tsmiRecentRepositories.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            toolStripMenuItem2,
            this.clearRecentRepositoriesListToolStripMenuItem,
            this.tsmiRecentRepositoriesClear});
            this.tsmiRecentRepositories.Image = global::GitUI.Properties.Images.RecentRepositories;
            this.tsmiRecentRepositories.Name = "tsmiRecentRepositories";
            this.tsmiRecentRepositories.Size = new System.Drawing.Size(198, 22);
            this.tsmiRecentRepositories.Text = "Recent Repositories";
            this.tsmiRecentRepositories.DropDownOpening += new System.EventHandler(this.tsmiRecentRepositories_DropDownOpening);
            // 
            // clearRecentRepositoriesListToolStripMenuItem
            // 
            this.clearRecentRepositoriesListToolStripMenuItem.Name = "clearRecentRepositoriesListToolStripMenuItem";
            this.clearRecentRepositoriesListToolStripMenuItem.Size = new System.Drawing.Size(119, 6);
            // 
            // tsmiRecentRepositoriesClear
            // 
            this.tsmiRecentRepositoriesClear.Name = "tsmiRecentRepositoriesClear";
            this.tsmiRecentRepositoriesClear.Size = new System.Drawing.Size(122, 22);
            this.tsmiRecentRepositoriesClear.Text = "Clear List";
            this.tsmiRecentRepositoriesClear.Click += new System.EventHandler(this.tsmiRecentRepositoriesClear_Click);
            // 
            // toolStripSeparator12
            // 
            this.toolStripSeparator12.Name = "toolStripSeparator12";
            this.toolStripSeparator12.Size = new System.Drawing.Size(195, 6);
            // 
            // cloneToolStripMenuItem
            // 
            this.cloneToolStripMenuItem.Image = global::GitUI.Properties.Images.CloneRepoGit;
            this.cloneToolStripMenuItem.Name = "cloneToolStripMenuItem";
            this.cloneToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.cloneToolStripMenuItem.Text = "Clone repository...";
            this.cloneToolStripMenuItem.Click += new System.EventHandler(this.CloneToolStripMenuItemClick);
            // 
            // toolStripMenuItem1
            // 
            this.toolStripMenuItem1.Name = "toolStripMenuItem1";
            this.toolStripMenuItem1.Size = new System.Drawing.Size(195, 6);
            // 
            // exitToolStripMenuItem
            // 
            this.exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            this.exitToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Q)));
            this.exitToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.exitToolStripMenuItem.Text = "Exit";
            this.exitToolStripMenuItem.Click += new System.EventHandler(this.ExitToolStripMenuItemClick);
            // 
            // refreshDashboardToolStripMenuItem
            // 
            this.refreshDashboardToolStripMenuItem.Image = global::GitUI.Properties.Images.ReloadRevisions;
            this.refreshDashboardToolStripMenuItem.ImageTransparentColor = System.Drawing.Color.Transparent;
            this.refreshDashboardToolStripMenuItem.Name = "refreshDashboardToolStripMenuItem";
            this.refreshDashboardToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.refreshDashboardToolStripMenuItem.Text = "&Refresh";
            this.refreshDashboardToolStripMenuItem.Click += new System.EventHandler(this.RefreshDashboardToolStripMenuItemClick);
            // 
            // toolStripSeparator46
            // 
            this.toolStripSeparator46.Name = "toolStripSeparator46";
            this.toolStripSeparator46.Size = new System.Drawing.Size(268, 6);
            // 
            // toolStripSeparator42
            // 
            this.toolStripSeparator42.Name = "toolStripSeparator42";
            this.toolStripSeparator42.Size = new System.Drawing.Size(177, 6);
            // 
            // dashboardToolStripMenuItem
            // 
            this.dashboardToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.refreshDashboardToolStripMenuItem,
            this.toolStripSeparator42});
            this.dashboardToolStripMenuItem.Name = "dashboardToolStripMenuItem";
            this.dashboardToolStripMenuItem.Size = new System.Drawing.Size(76, 19);
            this.dashboardToolStripMenuItem.Text = "&Dashboard";
            // 
            // mainMenuStrip
            // 
            this.mainMenuStrip.ClickThrough = true;
            this.mainMenuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.fileToolStripMenuItem,
            this.dashboardToolStripMenuItem});
            this.mainMenuStrip.Location = new System.Drawing.Point(0, 0);
            this.mainMenuStrip.Name = "mainMenuStrip";
            this.mainMenuStrip.Padding = new System.Windows.Forms.Padding(4);
            this.mainMenuStrip.Size = new System.Drawing.Size(923, 27);
            this.mainMenuStrip.TabIndex = 0;
            // 
            // toolPanel
            // 
            this.toolPanel.BottomToolStripPanelVisible = false;
            // 
            // toolPanel.ContentPanel
            // 
            this.toolPanel.ContentPanel.Controls.Add(this.MainSplitContainer);
            this.toolPanel.ContentPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolPanel.ContentPanel.Padding = new System.Windows.Forms.Padding(8);
            this.toolPanel.ContentPanel.Size = new System.Drawing.Size(923, 521);
            this.toolPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.toolPanel.LeftToolStripPanelVisible = false;
            this.toolPanel.Location = new System.Drawing.Point(0, 27);
            this.toolPanel.Margin = new System.Windows.Forms.Padding(0);
            this.toolPanel.Name = "toolPanel";
            this.toolPanel.Padding = new System.Windows.Forms.Padding(0);
            this.toolPanel.TopToolStripPanel.Padding = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.toolPanel.RightToolStripPanelVisible = false;
            this.toolPanel.Size = new System.Drawing.Size(923, 546);
            this.toolPanel.TabIndex = 1;
            // 
            // toolPanel.TopToolStripPanel
            // 
            this.toolPanel.TopToolStripPanel.Controls.Add(this.ToolStripMain);
            this.toolPanel.TopToolStripPanel.Controls.Add(this.ToolStripFilters);
            // 
            // FormBrowse
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.AutoValidate = System.Windows.Forms.AutoValidate.EnablePreventFocusChange;
            this.ClientSize = new System.Drawing.Size(923, 573);
            this.Controls.Add(this.toolPanel);
            this.Controls.Add(this.mainMenuStrip);
            this.Name = "FormBrowse";
            this.Text = "Git Extensions";
            this.ToolStripMain.ResumeLayout(false);
            this.ToolStripMain.PerformLayout();
            this.MainSplitContainer.Panel1.ResumeLayout(false);
            this.MainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.MainSplitContainer)).EndInit();
            this.MainSplitContainer.ResumeLayout(false);
            this.RightSplitContainer.Panel1.ResumeLayout(false);
            this.RightSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RightSplitContainer)).EndInit();
            this.RightSplitContainer.ResumeLayout(false);
            this.RevisionsSplitContainer.Panel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.RevisionsSplitContainer)).EndInit();
            this.RevisionsSplitContainer.ResumeLayout(false);
            this.mainMenuStrip.ResumeLayout(false);
            this.mainMenuStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.gitItemBindingSource)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.gitRevisionBindingSource)).EndInit();
            this.toolPanel.ContentPanel.ResumeLayout(false);
            this.toolPanel.TopToolStripPanel.ResumeLayout(false);
            this.toolPanel.TopToolStripPanel.PerformLayout();
            this.toolPanel.ResumeLayout(false);
            this.toolPanel.PerformLayout();
            this.ToolStripFilters.ResumeLayout(false);
            this.ToolStripFilters.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        #endregion

        internal SplitContainer MainSplitContainer;
        private SplitContainer RightSplitContainer;
        private SplitContainer RevisionsSplitContainer;

        private BindingSource gitRevisionBindingSource;
        private BindingSource gitItemBindingSource;
        private MenuStripEx mainMenuStrip;
        private ToolStripEx ToolStripMain;
        private ToolStripEx ToolStripFilters;
        private ToolTip FilterToolTip;
        private ToolStripContainer toolPanel;

        private ToolStripButton toolStripButtonCommit;
        private ToolStripSplitButton _NO_TRANSLATE_WorkingDir;
        private ToolStripSeparator toolStripSeparator0;
        private ToolStripSeparator toolStripSeparator1;
        private ToolStripSplitButton userShell;
        private ToolStripSeparator toolStripSeparator2;
        private ToolStripButton EditSettings;
        private ToolStripButton RefreshButton;
        private ToolStripTextBox toolStripRevisionFilterTextBox;
        private ToolStripPushButton toolStripButtonPush;
        private ToolStripLabel toolStripRevisionFilterLabel;
        private ToolStripSplitButton toolStripSplitStash;
        private ToolStripMenuItem stashChangesToolStripMenuItem;
        private ToolStripMenuItem stashPopToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator9;
        private ToolStripMenuItem manageStashesToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator17;
        private ToolStripSeparator toolStripSeparator19;
        private ToolStripLabel toolStripLabel1;
        private ToolStripComboBox toolStripBranchFilterComboBox;
        private ToolStripDropDownButton toolStripBranchFilterDropDownButton;
        private ToolStripDropDownButton toolStripRevisionFilterDropDownButton;
        private ToolStripSplitButton branchSelect;
        private ToolStripButton toggleBranchTreePanel;
        private ToolStripButton toggleSplitViewLayout;
        private ToolStripMenuItem fileToolStripMenuItem;
        private ToolStripMenuItem openToolStripMenuItem;
        private ToolStripMenuItem refreshDashboardToolStripMenuItem;
        private ToolStripMenuItem tsmiRecentRepositories;
        private ToolStripSeparator toolStripSeparator12;
        private ToolStripSeparator toolStripMenuItem1;
        private ToolStripMenuItem exitToolStripMenuItem;
        private ToolStripMenuItem cloneToolStripMenuItem;
        private ToolStripMenuItem initNewRepositoryToolStripMenuItem;
        private ToolStripMenuItem dashboardToolStripMenuItem;
        private ToolStripSplitButton toolStripButtonLevelUp;
        private ToolStripSplitButton toolStripButtonPull;
        private ToolStripMenuItem mergeToolStripMenuItem;
        private ToolStripMenuItem rebaseToolStripMenuItem1;
        private ToolStripMenuItem fetchToolStripMenuItem;
        private ToolStripMenuItem pullToolStripMenuItem1;
        private ToolStripMenuItem setDefaultPullButtonActionToolStripMenuItem;
        private ToolStripMenuItem fetchAllToolStripMenuItem;
        private ToolStripMenuItem fetchPruneAllToolStripMenuItem;
        private ToolStripSeparator toolStripSeparator42;
        private ToolStripSeparator toolStripSeparator46;
        private ToolStripButton ShowFirstParent;
        private ToolStripMenuItem tsmiRecentRepositoriesClear;
        private ToolStripSeparator clearRecentRepositoriesListToolStripMenuItem;
        private ToolStripButton toolStripFileExplorer;
        private ToolStripMenuItem createAStashToolStripMenuItem;
        private ToolStripMenuItem tsmiFavouriteRepositories;
        private ToolStripSplitButton menuCommitInfoPosition;
        private ToolStripMenuItem commitInfoBelowMenuItem;
        private ToolStripMenuItem commitInfoLeftwardMenuItem;
        private ToolStripMenuItem commitInfoRightwardMenuItem;
    }
}
