using System;
using System.Collections.Generic;
using System.Text;
using TagLib;

namespace FuwaTea.Audio.Metadata
{
    public class Metadata : Dictionary<object, IMetadataField>, IMetadata
    {
        //private const string PicturePrefix = "א.picture:";
    }
}
