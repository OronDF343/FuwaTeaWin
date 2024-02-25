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

    public interface IListWithDescriptorsField : IMetadataField<IList<EntryWithDescriptors>>
    {
        uint MaxCount { get; }
        uint MaxLength { get; }
        
        bool DescriptionSupported { get; }
        bool DescriptionRequired { get; }
        bool LanguageSupported { get; }
        bool LanguageRequired { get; }
    }

    public class EntryWithDescriptors
    {
        public EntryWithDescriptors(string value)
        {
            Value = value;
        }
        public EntryWithDescriptors(string lang, string desc, string value = null)
        {
            Language = lang;
            Description = desc;
            Value = value;
        }

        public string Language { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }

        // TODO: Text encoding
    }

    public interface IMetadataField<T> : IMetadataField
    {
        T? Value { get; set; }
    }

    public interface IMetadataField
    {
        void SetFrom(string s);
        void SetFrom(uint s);
        void SetFrom(IEnumerable<string> s);
        IEnumerable<string> ToStringEnumerable();
    }

    public abstract class MetadataField : IMetadataField
    {
        public abstract void SetFrom(string s);
        public abstract void SetFrom(uint s);
        public abstract void SetFrom(IEnumerable<string> s);
        public virtual IEnumerable<string> ToStringEnumerable()
        {
            yield return ToString();
        }
        public abstract override string ToString();
    }
}
