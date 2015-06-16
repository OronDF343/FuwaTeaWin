#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using FuwaTea.Common.Models;
using NAudio.Dsp;
using NAudio.Wave;

namespace FuwaTea.Logic.Playback.NAudio
{
    // Modified code from NAudioWpfDemo
    public class Equalizer : ISampleProvider
    {
        private readonly ISampleProvider _sourceProvider;
        private readonly ObservableCollection<EqualizerBand> _bands;
        private readonly List<BiQuadFilter[]> _filters;
        private readonly int _channels;
        private bool _updated;
        private readonly object _lockObj;

        public bool Enabled { get; set; }

        public Equalizer(ISampleProvider sourceProvider, ObservableCollection<EqualizerBand> bands)
        {
            _sourceProvider = sourceProvider;
            _bands = bands;
            _channels = sourceProvider.WaveFormat.Channels;
            _lockObj = new object();
            foreach (var band in _bands) band.PropertyChanged += EqualizerBandPropertyChanged;
            _bands.CollectionChanged += BandsOnCollectionChanged;
            _filters = new List<BiQuadFilter[]>();
            CreateFilters();
        }

        private void BandsOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Remove:
                    foreach (EqualizerBand item in e.OldItems)
                    {
                        //Removed items
                        item.PropertyChanged -= EqualizerBandPropertyChanged;
                    }
                    break;
                case NotifyCollectionChangedAction.Add:
                    foreach (EqualizerBand item in e.NewItems)
                    {
                        //Added items
                        item.PropertyChanged += EqualizerBandPropertyChanged;
                    }
                    break;
            }
            Update();
        }

        private void EqualizerBandPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Update();
        }

        private void CreateFilters()
        {
            lock (_lockObj)
            {
                _filters.Clear();
                foreach (var band in _bands)
                {
                    var filter = new BiQuadFilter[_channels];
                    for (var n = 0; n < _channels; n++)
                    {
                        filter[n] = BiQuadFilter.PeakingEQ(_sourceProvider.WaveFormat.SampleRate, band.Frequency,
                                                           band.Bandwidth, band.Gain);
                    }
                    _filters.Add(filter);
                }
            }
        }

        public void Update()
        {
            _updated = true;
            CreateFilters();
        }

        public WaveFormat WaveFormat
        {
            get { return _sourceProvider.WaveFormat; }
        }

        public int Read(float[] buffer, int offset, int count)
        {
            var samplesRead = _sourceProvider.Read(buffer, offset, count);
            if (!Enabled) return samplesRead;

            if (_updated)
            {
                CreateFilters();
                _updated = false;
            }

            lock (_lockObj)
            {
                for (var n = 0; n < samplesRead; n++)
                {
                    var ch = n % _channels;

                    for (var band = 0; band < _bands.Count; band++)
                    {
                        buffer[offset + n] = _filters[band][ch].Transform(buffer[offset + n]);
                    }
                }
            }
            return samplesRead;
        }
    }
}
