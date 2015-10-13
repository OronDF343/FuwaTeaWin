using System;
using ModularFramework;
using ModularFramework.Attributes;

namespace FuwaTea.Playlist
{
    [MeansImplicitUse]
    [BaseTypeRequired(typeof(IPlaylistHandler))]
    [AttributeUsage(AttributeTargets.Class)]
    public class PlaylistHandlerAttribute : ElementAttribute
    {
        public PlaylistHandlerAttribute(string elemName) : base(elemName) { }
    }
}
