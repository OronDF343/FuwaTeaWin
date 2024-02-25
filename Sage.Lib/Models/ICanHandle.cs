namespace Sage.Lib.Models
{
    /// <summary>
    /// Interface for determining whether an input can be handled, and if so, providing a realized output.
    /// </summary>
    public interface ICanHandle<in TInput, out TOutput, TFormat> : ISupportFormats<TFormat>
        where TInput : notnull
    {
        bool CanHandle(TInput ti);
        TOutput? Handle(TInput ti);
    }

    /// <inheritdoc/>
    public interface ICanHandle<in TInput, out TOutput> : ICanHandle<TInput, TOutput, string> { }
}