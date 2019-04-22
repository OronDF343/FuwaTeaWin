using System.Collections.Generic;
using System.ComponentModel;

namespace Sage.Lib.Models
{
    public interface IImplementationMultiSelector<TInterface> : INotifyPropertyChanged where TInterface : class
    {
        IReadOnlyCollection<TInterface> Implementations { get; }
        IReadOnlyList<TInterface> SelectedImplementations { get; }
        bool InsertImplementation(int index, TInterface ti);
        bool AppendImplementation(TInterface ti);
        void RemoveImplementation(int index);
        bool RemoveImplementation(TInterface ti);
    }
}
