﻿using GitCommands;
using GitUIPluginInterfaces;

namespace GitUI.CommandsDialogs.SettingsDialog.Pages
{
    public partial class PluginRootIntroductionPage : SettingsPageBase
    {
        public PluginRootIntroductionPage()
        {
            InitializeComponent();
            Text = "Plugins Settings";
            Translate();
        }

        protected override void SettingsToPage()
        {            
        }

        protected override void PageToSettings()
        {
        }

        protected override bool AreEffectiveSettings => true;
        protected override SettingsSource GetCurrentSettings()
        {
            return AppSettings.Instance.SettingsContainer;
        }
    }
}
