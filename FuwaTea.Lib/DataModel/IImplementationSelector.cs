using System.Collections.Generic;
using System.ComponentModel;

namespace FuwaTea.Lib.DataModel
{
    public interface IImplementationSelector<TInterface> : INotifyPropertyChanged where TInterface : class
    {
        IReadOnlyCollection<TInterface> Implementations { get; }
        TInterface DefaultImplementation { get; }
        TInterface SelectedImplementation { get; }
        void SelectImplementation(TInterface ti);
    }
}
