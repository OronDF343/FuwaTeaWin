using System.ComponentModel.Composition;

namespace FuwaTea.Core
{
    [InheritedExport]
    public interface IAppInstance
    {
        void Initialize(string[] args);
    }
}
