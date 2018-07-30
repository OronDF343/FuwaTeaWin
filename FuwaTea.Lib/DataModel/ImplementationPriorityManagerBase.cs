using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using JetBrains.Annotations;

namespace FuwaTea.Lib.DataModel
{
    public abstract class ImplementationPriorityManagerBase<TInterface, TInput, TOutput> : IImplementationPriorityManager<TInterface, TInput, TOutput>
        where TInterface : ICanHandle<TInput, TOutput>
    {
        // ReSharper disable once VirtualMemberCallInConstructor
        protected ImplementationPriorityManagerBase() => Init();

        protected virtual void Init()
        {
            Implementations = new ReadOnlyCollection<TInterface>(ProtectedImplementations);
            ImplementationDictionary.Add("", new List<TInterface>());
            foreach (var format in SupportedFormats)
                ImplementationDictionary.Add(format, new List<TInterface>());
            foreach (var impl in Implementations)
            {
                ImplementationDictionary[""].Add(impl);
                foreach (var format in impl.SupportedFormats)
                    ImplementationDictionary[format].Add(impl);
            }
        }

        public IEnumerable<string> SupportedFormats => Implementations.SelectMany(i => i.SupportedFormats).Distinct();

        public bool CanHandle(TInput ti) => !Equals(GetImplementations(ti).FirstOrDefault(it => it.CanHandle(ti)), default(TInterface));

        public virtual TOutput Handle(TInput ti)
        {
            foreach (var impl in GetImplementations(ti).Where(it => it.CanHandle(ti)))
            {
                TOutput to;
                try
                {
                    to = impl.Handle(ti);
                }
                catch (Exception e)
                {
                    // TODO: Log e
                    continue;
                }

                if (!Equals(to, default(TOutput))) return to;
            }

            return default(TOutput);
        }

        protected Dictionary<string, List<TInterface>> ImplementationDictionary { get; } = new Dictionary<string, List<TInterface>>();

        /// <summary>
        /// Implementations must populate this list immediately, using something like [ImportMany], or in the beginning of the <see cref="Init"/> method.
        /// </summary>
        protected abstract IList<TInterface> ProtectedImplementations { get; }

        /// <summary>
        /// Returns the format of an input, if known. Otherwise, returns an empty string.
        /// REQUIRED!
        /// </summary>
        /// <param name="ti"></param>
        /// <returns></returns>
        [NotNull]
        public abstract string FormatOf(TInput ti);

        public IReadOnlyCollection<TInterface> Implementations { get; private set; }

        public virtual IReadOnlyCollection<TInterface> GetImplementations(string format) => new ReadOnlyCollection<TInterface>(ImplementationDictionary[format]);

        public IReadOnlyCollection<TInterface> this[string format] => GetImplementations(format);

        public virtual IReadOnlyCollection<TInterface> GetImplementations(TInput ti) => GetImplementations(FormatOf(ti));

        public IReadOnlyCollection<TInterface> this[TInput ti] => GetImplementations(ti);

        public void ConfigurePriority(string format, Func<TInterface, int> priorityFunc)
        {
            ConfigurePriority(format, (ti1, ti2) => priorityFunc(ti1).CompareTo(priorityFunc(ti2)));
        }

        public void ConfigurePriority(string format, Comparison<TInterface> compareFunc)
        {
            ImplementationDictionary[format].Sort(compareFunc);
        }
    }
}