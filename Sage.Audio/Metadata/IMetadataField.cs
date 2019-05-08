using System;
using System.Collections.Generic;

namespace Sage.Audio.Metadata
{
    public interface ITextField : IMetadataField<string>
    {
        uint MaxLength { get; }
    }

    public interface IListField : IMetadataField<IList<string>>
    {
        uint MaxCount { get; }
        uint MaxLength { get; }
    }

    public interface IDateTimeField : IMetadataField<DateTime?>
    {
        ushort? Year { get; set; }
        byte? Month { get; set; }
        byte? Day { get; set; }
        byte? Hour { get; set; }
        byte? Minute { get; set; }
        byte? Second { get; set; }
        
        /// <summary>
        /// Specifies how many of the above fields are supported (1~6).
        /// The order of precedence is always Year, Month, Day, Hour, Minute, Second.
        /// </summary>
        byte MaxResolution { get; }
    }

    public interface INumericField : IMetadataField<uint?>
    {
        uint MaxValue { get; }
    }

    public interface IEnumField<T> : IMetadataField<T?> where T : struct, Enum, IConvertible
    {
        string StringValue { get; set; }
    }

    public interface IMetadataField<T> : IMetadataField
    {
        T Value { get; set; }
    }

    public interface IMetadataField
    {
        void ParseFrom(string s);
    }
}
