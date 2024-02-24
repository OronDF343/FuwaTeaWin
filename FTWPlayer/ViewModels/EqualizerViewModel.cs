using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FTWPlayer.Views;
using FuwaTea.Playback;
using CommunityToolkit.Mvvm.Input;
using ModularFramework;

namespace FTWPlayer.ViewModels
{
    [UIPart("Equalizer Tab")]
    public class EqualizerViewModel : ITab
    {
        public EqualizerViewModel()
        {
            PlaybackManager = ModuleFactory.GetElement<IPlaybackManager>();
            ResetEqCommand = new RelayCommand<RoutedEventArgs>(ResetEq);
            TabObject = new EqualizerControl(this);
        }

        public TabItem TabObject { get; }
        public decimal Index => 1;

        public IPlaybackManager PlaybackManager { get; }

        public ICommand ResetEqCommand { get; set; }

        private void ResetEq(RoutedEventArgs e)
        {
            // TODO: EqualizerManager
            foreach (var band in PlaybackManager.EqualizerBands)
            {
                band.Gain = 0;
            }
        }
    }
}
