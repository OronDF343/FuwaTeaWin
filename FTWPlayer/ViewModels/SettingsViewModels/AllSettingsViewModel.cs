﻿using System.Collections.ObjectModel;
using System.Configuration;
using System.Linq;
using System.Windows.Controls;
using FTWPlayer.Views.SettingsViews;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    [UIPart("All Settings Tab")]
    public class AllSettingsViewModel : ISettingsTab
    {
        public TabItem GetTabItem(ApplicationSettingsBase settings)
        {
            SettingsPropertyValues = new ObservableCollection<SettingsPropertyValue>(settings.PropertyValues.OfType<SettingsPropertyValue>());
            return new AllSettingsView(this);

        }

        public decimal Index => 1;
        public ObservableCollection<SettingsPropertyValue> SettingsPropertyValues { get; private set; }
    }
}