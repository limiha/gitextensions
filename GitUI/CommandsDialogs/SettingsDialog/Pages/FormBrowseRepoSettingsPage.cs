﻿using GitCommands;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace GitUI.CommandsDialogs.SettingsDialog.Pages
{
    public partial class FormBrowseRepoSettingsPage : SettingsPageWithHeader
    {
        public FormBrowseRepoSettingsPage()
        {
            InitializeComponent();
            Text = "Browse repository window";
            Translate();
        }

        protected override void Init(ISettingsPageHost aPageHost)
        {
            base.Init(aPageHost);
            BindSettingsWithControls();
        }

        private void BindSettingsWithControls()
        {
            AddSettingBinding(AppSettings.Instance.ShowConEmuTab, chkChowConsoleTab);
            AddSettingBinding(AppSettings.Instance.ConEmuStyle, _NO_TRANSLATE_cboStyle);
            AddSettingBinding(AppSettings.Instance.ConEmuTerminal, cboTerminal);
            AddSettingBinding(AppSettings.Instance.ConEmuFontSize, cboFontSize);
            AddSettingBinding(AppSettings.Instance.ShowRevisionInfoNextToRevisionGrid, chkShowRevisionInfoNextToRevisionGrid);
        }

        private void chkChowConsoleTab_CheckedChanged(object sender, System.EventArgs e)
        {
            groupBoxConsoleSettings.Enabled = chkChowConsoleTab.Checked;
        }

        public static SettingsPageReference GetPageReference()
        {
            return new SettingsPageReferenceByType(typeof(FormBrowseRepoSettingsPage));
        }
    }
}
