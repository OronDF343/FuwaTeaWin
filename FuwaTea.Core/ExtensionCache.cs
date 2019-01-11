using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using FuwaTea.Extensibility;
using FuwaTea.Extensibility.Config;
using JetBrains.Annotations;

namespace FuwaTea.Core
{
    /// <summary>
    /// The extension cache.
    /// </summary>
    public class ExtensionCache : IConfigPage
    {
        /// <summary>
        /// The list of known extensions.
        /// </summary>
        public List<Extension> Extensions { get; } = new List<Extension>();

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
