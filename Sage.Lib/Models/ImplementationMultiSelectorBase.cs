using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json.Serialization;
using Sage.Extensibility.Config;

namespace Sage.Lib.Models
{
    public abstract class ImplementationMultiSelectorBase<TInterface> : IImplementationMultiSelector<TInterface>, IConfigPage where TInterface : class
    {
        protected ImplementationMultiSelectorBase() { }

        // ReSharper disable once VirtualMemberCallInConstructor
        protected ImplementationMultiSelectorBase(IList<TInterface> implementations) => Init(implementations);

        protected virtual void Init(IList<TInterface> implementations)
        {
            Implementations = new ReadOnlyCollection<TInterface>(implementations);
        }

        protected List<TInterface> SelectedImplementationsList { get; } = new List<TInterface>();

        [JsonIgnore]
        public IReadOnlyCollection<TInterface> Implementations { get; private set; }

        [JsonIgnore]
        public IReadOnlyList<TInterface> SelectedImplementations => new ReadOnlyCollection<TInterface>(SelectedImplementationsList);

        public virtual bool InsertImplementation(int index, TInterface ti)
        {
            SelectedImplementationsList.Insert(index, ti);
            return true;
        }

        public virtual bool AppendImplementation(TInterface ti)
        {
            SelectedImplementationsList.Add(ti);
            return true;
        }

        public virtual void RemoveImplementation(int index)
        {
            SelectedImplementationsList.RemoveAt(index);
        }

        public virtual bool RemoveImplementation(TInterface ti)
        {
            return SelectedImplementationsList.Remove(ti);
        }

        public List<string> SerializedClassNameList
        {
            get => SelectedImplementations.Select(i => i.GetType().AssemblyQualifiedName).ToList();
            set
            {
                SelectedImplementationsList.Clear();
                var cn = Implementations.ToDictionary(i => i.GetType().AssemblyQualifiedName, i => i);
                SelectedImplementationsList.AddRange(value.Select(v => cn[v]));
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
