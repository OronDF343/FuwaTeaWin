using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Sage.Extensibility.Config;

namespace Sage.Audio.Playback.CSCore
{
    public abstract class CSCoreApiConfigBase : IConfigPage
    {
        public float MasterVolume { get; set; } = 1.0f;

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
