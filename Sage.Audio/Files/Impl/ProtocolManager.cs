﻿using System.Collections.Generic;
using System.ComponentModel.Composition;
using FuwaTea.Lib.DataModel;

namespace Sage.Audio.Files.Impl
{
    public class ProtocolManager : ImplementationPriorityManagerBase<IProtocolHandler, IFileLocationInfo, IFileHandle>, IProtocolManager
    {
        public ProtocolManager([ImportMany] IList<IProtocolHandler> implementations)
            : base(implementations) { }

        public override string FormatOf(IFileLocationInfo ti) => ti.Protocol;
    }
}