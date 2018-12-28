using System.Collections.Generic;
using System.ComponentModel.Composition;
using DryIocAttributes;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Files.Impl
{
    public class ProtocolManager : ImplementationPriorityManagerBase<IProtocolHandler, IFileLocationInfo, IFileHandle>, IProtocolManager
    {
        public ProtocolManager([ImportMany] IList<IProtocolHandler> implementations)
            : base(implementations) { }

        public override string FormatOf(IFileLocationInfo ti) => ti.Protocol;
    }
}
