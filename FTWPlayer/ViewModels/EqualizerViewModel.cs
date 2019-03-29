﻿using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using FTWPlayer.Views;
using GalaSoft.MvvmLight.CommandWpf;
using Sage.Audio.Playback;

namespace FTWPlayer.ViewModels
{
    //[UIPart("Equalizer Tab")]
    public class EqualizerViewModel : ITab
    {
        
        public EqualizerViewModel([Import] IPlaybackManager playbackManager, [Import] UISettings settings)
        {
            PlaybackManager = playbackManager;
            ResetEqCommand = new RelayCommand<RoutedEventArgs>(ResetEq);
            TabObject = new EqualizerControl(this);
            Settings = settings;
        }

        public UISettings Settings { get; }

        public TabItem TabObject { get; }
        public decimal Index => 1;

        public IPlaybackManager PlaybackManager { get; }

        public ICommand ResetEqCommand { get; set; }

        private void ResetEq(RoutedEventArgs e)
        {
            // TODO: Reset EQ
            /*foreach (var band in PlaybackManager.EqualizerBands)
            {
                band.Gain = 0;
            }*/
        }
    }
}
