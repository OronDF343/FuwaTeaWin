using System.Collections.Generic;
using System.ComponentModel.Composition;
using CSCore;
using DryIocAttributes;
using FuwaTea.Audio.Files;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Decoders.Impl
{
    public class DecoderManager : ImplementationPriorityManagerBase<ITrackDecoder, IFileHandle, ISampleSource>, IDecoderManager
    {
        public DecoderManager([ImportMany] IList<ITrackDecoder> implementations)
            : base(implementations) { }

        public override string FormatOf(IFileHandle ti)
        {
            return ti.Extension;
        }
    }
}
