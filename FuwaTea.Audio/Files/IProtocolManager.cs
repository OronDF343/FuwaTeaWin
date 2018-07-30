﻿using System.ComponentModel.Composition;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Files
{
    [InheritedExport]
    public interface IProtocolManager : IImplementationPriorityManager<IProtocolHandler, IFileLocationInfo, IFileHandle>
    {
    }
}
