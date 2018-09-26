using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using CSCore;
using FuwaTea.Audio.Files;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Decoders
{
    [InheritedExport]
    public interface IDecoderManager : IImplementationPriorityManager<ITrackDecoder, IFileHandle, ISampleSource>
    {
    }
}
