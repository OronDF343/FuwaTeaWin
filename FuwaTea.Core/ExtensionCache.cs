using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
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

        /// <summary>
        /// Create or find an extension instance in the extension cache.
        /// </summary>
        /// <param name="path">The relative path to the DLL file.</param>
        /// <returns>An extension object, not initialized.</returns>
        [NotNull]
        public Extension CreateExtension(string path)
        {
            var ext = FindExtension(path);
            if (ext == null)
            {
                ext = new Extension(path, true);
                Extensions.Add(ext);
            }
            return ext;
        }
        
        /// <summary>
        /// Find an extension instance in the extension cache.
        /// </summary>
        /// <param name="path">The relative path to the DLL file.</param>
        /// <returns>An extension object, or null if the extension was not found.</returns>
        [CanBeNull]
        public Extension FindExtension(string path)
        {
            return Extensions.FirstOrDefault(e => e.RelativeFilePath == path);
        }

        /// <inheritdoc />
        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
