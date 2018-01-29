using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using DryIocAttributes;
using FTWPlayer.Views;
using FuwaTea.Playback;
using GalaSoft.MvvmLight.CommandWpf;

namespace FTWPlayer.ViewModels
{
    //[UIPart("Equalizer Tab")]
    [Reuse(ReuseType.Singleton)]
    public class EqualizerViewModel : ITab
    {
        
        public EqualizerViewModel([Import] IPlaybackManager playbackManager)
        {
            PlaybackManager = playbackManager;
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
