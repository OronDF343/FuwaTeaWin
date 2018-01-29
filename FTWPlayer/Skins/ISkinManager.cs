using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using JetBrains.Annotations;

namespace FTWPlayer.Skins
{
    [InheritedExport]
    public interface ISkinManager : IUIPart
    {
        ObservableCollection<SkinPackage> LoadedSkins { get; }

        [CanBeNull]
        SkinPackage GetLoadedSkin(string source);

        /// <exception cref="System.IO.DirectoryNotFoundException">Skins directory is invalid, such as referring to an unmapped drive. </exception>
        /// <exception cref="System.IO.IOException">Skins directory is a file name.</exception>
        /// <exception cref="System.Security.SecurityException">The caller does not have the required permission. </exception>
        /// <exception cref="System.UnauthorizedAccessException">The caller does not have the required permission. </exception>
        /// <exception cref="SkinLoadException">An error occured while loading a skin</exception>
        void LoadAllSkins(Action<Exception> ec);

        /// <summary>
        /// Loads a skin
        /// </summary>
        /// <remarks>Only this function takes shortened paths! It also adds missing ResourceDictionaries from the default skin</remarks>
        /// <param name="source">The location of the skin as a shortened path</param>
        /// <param name="children">(used internally in the recursion) all the children of the current skin</param>
        /// <exception cref="System.ArgumentNullException"><paramref name="source"/> is <see langword="null" />.</exception>
        /// <exception cref="System.InvalidOperationException">There is a cyclic dependency between skins</exception>
        /// <exception cref="SkinLoadException">An error occured while loading the skin</exception>
        SkinPackage LoadSkin(string source, HashSet<string> children = null);

        /// <exception cref="SkinLoadException">An error occured while loading the skin</exception>
        SkinPackage LoadFallbackSkin();
        
        string ExpandPath(string path);
        string ShortenPath(string path);
    }
}
