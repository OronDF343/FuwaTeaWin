using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using JetBrains.Annotations;
using Newtonsoft.Json;
using Sage.Extensibility.Config;

namespace Sage.Lib.Models
{
    public abstract class ImplementationSelectorBase<TInterface> : IImplementationSelector<TInterface>, IConfigPage where TInterface : class
    {
        protected ImplementationSelectorBase() { }

        // ReSharper disable once VirtualMemberCallInConstructor
        protected ImplementationSelectorBase([NotNull] IList<TInterface> implementations) => Init(implementations, implementations.FirstOrDefault());

        protected virtual void Init([NotNull] IList<TInterface> implementations, TInterface defaultImplementation)
        {
            Implementations = new ReadOnlyCollection<TInterface>(implementations);
            DefaultImplementation = defaultImplementation;
            SelectImplementation(DefaultImplementation);
        }

        [JsonIgnore]
        public IReadOnlyCollection<TInterface> Implementations { get; private set; }

        [JsonIgnore]
        public TInterface DefaultImplementation { get; private set; }
        
        [JsonIgnore]
        public TInterface SelectedImplementation { get; private set; }

        public virtual void SelectImplementation(TInterface ti)
        {
            SelectedImplementation = ti ?? Implementations.First();
        }
        
        [UsedImplicitly]
        public string SerializedClassName
        {
            get => SelectedImplementation.GetType().AssemblyQualifiedName;
            set => SelectImplementation(Implementations.FirstOrDefault(i => i.GetType().AssemblyQualifiedName?
                                                                                .Equals(value, StringComparison.Ordinal)
                                                                            ?? false));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
