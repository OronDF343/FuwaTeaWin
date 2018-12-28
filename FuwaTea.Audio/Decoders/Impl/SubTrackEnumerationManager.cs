using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using DryIocAttributes;
using FuwaTea.Audio.Files;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Decoders.Impl
{
    public class SubTrackEnumerationManager : ImplementationPriorityManagerBase<ISubTrackEnumerator, IFileHandle, IEnumerable<ISubTrackInfo>>, ISubTrackEnumerationManager
    {
        public SubTrackEnumerationManager([ImportMany] IList<ISubTrackEnumerator> implementations)
            : base(implementations) { }

        public override string FormatOf(IFileHandle ti)
        {
            return ti.Extension;
        }
    }
}
