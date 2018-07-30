using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Text;
using FuwaTea.Lib.DataModel;

namespace FuwaTea.Audio.Files
{
    [InheritedExport]
    public interface IProtocolManager : IImplementationPriorityManager<IProtocolHandler, IFileLocationInfo, IFileHandle>
    {
    }
}
