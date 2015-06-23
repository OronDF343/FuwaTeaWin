using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FuwaTea.Presentation.Playback;
using GalaSoft.MvvmLight.CommandWpf;
using LayerFramework;

namespace FTWPlayer.Tabs
{
    [UIPart("Equalizer Tab")]
    public class EqualizerTab : ITab
    {
        public EqualizerTab()
        {
            PlaybackManager = LayerFactory.GetElement<IPlaybackManager>();
            ResetEqCommand = new RelayCommand<RoutedEventArgs>(ResetEq);
            TabObject = new EqualizerControl(this);
        }

        public TabItem TabObject { get; private set; }
        public decimal Index { get { return 1; } }

        public IPlaybackManager PlaybackManager { get; private set; }

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
