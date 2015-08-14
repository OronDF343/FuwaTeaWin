using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FTWPlayer.Views;
using FuwaTea.Presentation.Playback;
using GalaSoft.MvvmLight.CommandWpf;
using LayerFramework;

namespace FTWPlayer.ViewModels
{
    [UIPart("Equalizer Tab")]
    public class EqualizerViewModel : ITab
    {
        public EqualizerViewModel()
        {
            PlaybackManager = LayerFactory.GetElement<IPlaybackManager>();
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
