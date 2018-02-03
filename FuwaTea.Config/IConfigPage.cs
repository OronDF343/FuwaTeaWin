using System.Collections.Generic;
using System.ComponentModel.Composition;

namespace FuwaTea.Config
{
    [InheritedExport]
    public interface IConfigPage
    {
        string Key { get; }
        IDictionary<string, object> Values { get; }
    }
}
