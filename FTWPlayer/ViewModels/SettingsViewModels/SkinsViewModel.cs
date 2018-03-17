using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using System.Windows.Controls;
using System.Windows.Input;
using DryIocAttributes;
using FTWPlayer.Skins;
using FTWPlayer.Views.SettingsViews;
using FuwaTea.Wpf.Behaviors;
using GalaSoft.MvvmLight.CommandWpf;
using JetBrains.Annotations;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    //[UIPart("Skin Selection")]
    [Reuse(ReuseType.Singleton)]
    public class SkinsViewModel : ISettingsTab, INotifyPropertyChanged
    {
        
        public SkinsViewModel([Import] ISkinManager skinManager)
        {
            SkinManager = skinManager;
            LoadSkin = new RelayCommand(OnLoadSkin);
        }

        public TabItem GetTabItem()
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
