﻿using System;
using System.Collections.Generic;

namespace Sage.Lib.Models
{
    /// <summary>
    /// Interface for providing a method of customizable implementation priority and fallback.
    /// </summary>
    /// <typeparam name="TInterface">The interface</typeparam>
    /// <typeparam name="TInput">The input object type</typeparam>
    /// <typeparam name="TOutput">The realized output object type</typeparam>
    public interface IImplementationPriorityManager<TInterface, in TInput, out TOutput> : ICanHandle<TInput, TOutput>
        where TInterface : ICanHandle<TInput, TOutput>
    {
        IReadOnlyCollection<TInterface> Implementations { get; }
        IReadOnlyCollection<TInterface> GetImplementations(string format);
        IReadOnlyCollection<TInterface> this[string format] { get; }
        IReadOnlyCollection<TInterface> GetImplementations(TInput ti);
        IReadOnlyCollection<TInterface> this[TInput ti] { get; }
        
        // Ascending sort!
        void ConfigurePriority(string format, Func<TInterface, int> priorityFunc);
        void ConfigurePriority(string format, Comparison<TInterface> compareFunc);

        string FormatOf(TInput ti);

        TOutput Handle(TInput ti, out TInterface implementation);
    }

    /// <inheritdoc />
    /// <summary>
    /// Shorthand version that handles basic ICanHandle&lt;TInput, TOutput&gt; interfaces.
    /// </summary>
    public interface IImplementationPriorityManager<TInput, TOutput> : IImplementationPriorityManager<ICanHandle<TInput, TOutput>, TInput, TOutput> { }
}