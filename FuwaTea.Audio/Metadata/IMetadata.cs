using System.Collections.Generic;
using TagLib;

namespace FuwaTea.Audio.Metadata
{
    public interface IMetadata : IDictionary<object, IMetadataField>
    {
    }
}
