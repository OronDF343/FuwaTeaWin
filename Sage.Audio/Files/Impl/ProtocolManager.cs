using System.Collections.Generic;
using System.ComponentModel.Composition;
using Sage.Extensibility.Config;
using Sage.Lib.Models;

namespace Sage.Audio.Files.Impl
{
    
    [ConfigPage(nameof(ProtocolManager))]
    public class ProtocolManager : ImplementationPriorityManagerBase<IProtocolHandler, IFileLocationInfo, IFileHandle>, IProtocolManager
    {
        public ProtocolManager([ImportMany] IList<IProtocolHandler> implementations)
            : base(implementations) { }

        public override string FormatOf(IFileLocationInfo ti) => ti.Protocol;
    }
}
