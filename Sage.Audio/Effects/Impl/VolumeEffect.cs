using System;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Sage.Extensibility.Config;

namespace Sage.Audio.Effects.Impl
{
    [ConfigPage(nameof(VolumeEffect)), Export]
    public class VolumeEffect : EffectBase, IConfigPage
    {
        private float _volume = 1f;

        public virtual float Volume
        {
            get => _volume;
            set
            {
                if (value < 0 || value > 1)
                    throw new ArgumentOutOfRangeException(nameof(value));
                _volume = value;
            }
        }

        public virtual float Epsilon { get; set; } = 0.0001f;

        public override int Read(float[] buffer, int offset, int count)
        {
            if (BaseSource == null) return 0;

            var read = BaseSource.Read(buffer, offset, count);
            
            var volume = Volume;
            if (Math.Abs(volume) < Epsilon)
                Array.Clear(buffer, offset, read);
            else if (Math.Abs(1f - volume) > Epsilon)
                for (var i = offset; i < read + offset; ++i)
                    buffer[i] *= volume;

            return read;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
