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

using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace FuwaTea.Common.Models
{
    public class EqualizerBand : INotifyPropertyChanged
    {
        private float _frequency;
        public float Frequency { get { return _frequency; } set { _frequency = value; OnPropertyChanged(); } }

        private float _gain;
        public float Gain { get { return _gain; } set { _gain = value; OnPropertyChanged(); } }

        private float _bandwidth;
        public float Bandwidth { get { return _bandwidth; } set { _bandwidth = value; OnPropertyChanged(); } }

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
