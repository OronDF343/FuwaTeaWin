using System;
using System.Collections.Generic;
using JetBrains.Annotations;

namespace FuwaTea.Lib.DataModel
{
    /// <summary>
    /// Interface for providing a method of customizable implementation priority and fallback.
    /// </summary>
    /// <typeparam name="TInterface">The interface</typeparam>
    /// <typeparam name="TInput">The input object type</typeparam>
    /// <typeparam name="TOutput">The realized output object type</typeparam>
    public interface IImplementationPriorityManager<out TInterface, in TInput, out TOutput> : ICanHandle<TInput, TOutput>
        where TInterface : ICanHandle<TInput, TOutput>
    {
        IEnumerable<TInterface> Implementations { get; }
        IEnumerable<TInterface> GetImplementation([CanBeNull] string format);
        IEnumerable<TInterface> this[[CanBeNull] string format] { get; }
        IEnumerable<TInterface> GetImplementation([NotNull] TInput ti);
        IEnumerable<TInterface> this[[NotNull] TInput ti] { get; }

        void ConfigurePriority([CanBeNull] string format, Func<TInterface, int> priorityFunc);
        void ConfigurePriority([CanBeNull] string format, Comparison<TInterface> compareFunc);
    }

    /// <summary>
    /// Shorthand version that handles basic ICanHandle&lt;TInput, TOutput&gt; interfaces.
    /// </summary>
    /// <typeparam name="TInput"></typeparam>
    /// <typeparam name="TOutput"></typeparam>
    public interface IImplementationPriorityManager<in TInput, out TOutput> : IImplementationPriorityManager<ICanHandle<TInput, TOutput>, TInput, TOutput> { }

    /// <summary>
    /// Interface for determining whether an input can be handled, and if so, providing a realized output.
    /// </summary>
    public interface ICanHandle<in TInput, out TOutput>
    {
        string[] SupportedFormats { get; }
        bool CanHandle(TInput ti);
        TOutput Handle(TInput ti);
    }
}