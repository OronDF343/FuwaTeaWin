using JetBrains.Annotations;

namespace FuwaTea.Lib.DataModel
{
    /// <summary>
    /// Interface for determining whether an input can be handled, and if so, providing a realized output.
    /// </summary>
    public interface ICanHandle<in TInput, out TOutput> : ISupportFormats
    {
        bool CanHandle([NotNull] TInput ti);
        [CanBeNull]
        TOutput Handle([NotNull] TInput ti);
    }
}