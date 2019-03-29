using System.Collections.Generic;
using System.ComponentModel.Composition;
using CSCore;
using FuwaTea.Lib.DataModel;
using Sage.Audio.Files;

namespace Sage.Audio.Decoders.Impl
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
