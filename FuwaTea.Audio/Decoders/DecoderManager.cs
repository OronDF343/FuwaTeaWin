using CSCore;
using DryIocAttributes;
using FuwaTea.Audio.Files;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Decoders
{
    [Reuse(ReuseType.Singleton)]
    public class DecoderManager : ImplementationPriorityManagerBase<ITrackDecoder, IFileHandle, ISampleSource>, IDecoderManager
    {
        public override string FormatOf(IFileHandle ti)
        {
            return ti.Extension;
        }
    }
}
