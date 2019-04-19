using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sage.Audio.Files;
using Sage.Lib.Models;

namespace Sage.Audio.Decoders.Impl
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
