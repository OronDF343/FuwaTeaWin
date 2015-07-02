using System;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using FuwaTea.Annotations;

namespace FTWPlayer.Tabs
{
    [UIPart("Settings Tab")]
    public class SettingsTab : ITab
    {
        public SettingsTab()
        {
            TabObject = new SettingsView(this);
        }
        public TabItem TabObject { get; private set; }
        public decimal Index { get { return 3; } }
    }
}
