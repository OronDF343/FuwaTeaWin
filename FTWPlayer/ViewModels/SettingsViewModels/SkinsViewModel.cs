using System.Collections.Generic;
using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using FTWPlayer.Skins;
using FTWPlayer.Views.SettingsViews;
using FuwaTea.Wpf.Behaviors;
using CommunityToolkit.Mvvm.Input;
using ModularFramework;
using ModularFramework.Configuration;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    [UIPart("Skin Selection")]
    public class SkinsViewModel : ISettingsTab, INotifyPropertyChanged
    {
        public SkinsViewModel()
        {
            SkinManager = ModuleFactory.GetElement<ISkinManager>();
            LoadSkin = new RelayCommand(OnLoadSkin);
        }

        public TabItem GetTabItem(ApplicationSettingsBase settings, List<IConfigurablePropertyInfo> dynSettings)
        {
            return new SkinsView(this);
        }

        public decimal Index => 2;

        public ISkinManager SkinManager { get; }
        public ICommand LoadSkin { get; }
        public SkinPackage SelectedSkin { get; set; }
        public ResourceDictionaryIdentifier LoadedSkinIdentifier => SkinLoadingBehavior.LoadedSkin.GetIdentifier();

        public void OnLoadSkin()
        {
            SkinLoadingBehavior.UpdateSkin(SelectedSkin.SkinParts);
            OnPropertyChanged(nameof(LoadedSkinIdentifier));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
