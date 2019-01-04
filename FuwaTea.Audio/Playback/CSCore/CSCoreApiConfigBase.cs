using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using FuwaTea.Extensibility.Config;
using JetBrains.Annotations;

namespace FuwaTea.Audio.Playback.CSCore
{
    [SuppressMessage("ReSharper", "InconsistentNaming", Justification = "Library name is CSCore")]
    public abstract class CSCoreApiConfigBase : IConfigPage
    {
        public float MasterVolume { get; set; } = 1.0f;

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
