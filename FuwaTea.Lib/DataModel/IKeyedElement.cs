namespace FuwaTea.Lib.DataModel
{
    /// <summary>
    /// An element identified by an <see cref="int"/>
    /// </summary>
    public interface IKeyedElement : IKeyedElement<int> { }

    /// <summary>
    /// An element with a unique identifier
    /// </summary>
    /// <typeparam name="T">The type of the identifier</typeparam>
    public interface IKeyedElement<T>
    {
        /// <summary>
        /// Gets the unique identifier for this object.
        /// WARNING: The setter only exists for deserialization! Changing the key could lead to unexpected results!
        /// </summary>
        T Key { get; set; }
    }
}
