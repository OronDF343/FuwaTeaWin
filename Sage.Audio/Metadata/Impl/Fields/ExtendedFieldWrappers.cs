using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Sage.Lib;

namespace Sage.Audio.Metadata.Impl.Fields
{
    public abstract class ExtendedMetadataField<T> : IMetadataField<T>
    {
        protected IMetadata Metadata { get; }

        public ExtendedMetadataField(IMetadata metadata, string fieldName)
        {
            Metadata = metadata;
            FieldName = fieldName;
        }

        public string FieldName { get; }

        void IMetadataField.ParseFrom(string s)
        {
            Metadata.ExtendedFields[FieldName].Add(s);
        }

        public virtual T Value
        {
            get => Parse(Metadata.ExtendedFields[FieldName].FirstOrDefault());
            set
            {
                var f = Metadata.ExtendedFields[FieldName];
                if (value == null) f.Clear();
                else if (f.Count < 1) f.Add(Format(value));
                else f[0] = Format(value);
            }
        }

        protected abstract T Parse(string str);

        protected virtual string Format(T value)
        {
            return value.ToString();
        }

        public override string ToString()
        {
            return Metadata.ExtendedFields[FieldName].FirstOrDefault() ?? "null";
        }
    }

    public class ExtendedNumericField : ExtendedMetadataField<uint?>, INumericField
    {
        public ExtendedNumericField(IMetadata metadata, string fieldName)
            : base(metadata, fieldName) { }
        
        protected override uint? Parse(string str) => str != null ? uint.Parse(str) : (uint?)null;

        public uint MaxValue => 0;
    }

    public class ExtendedTextField : ExtendedMetadataField<string>, ITextField
    {
        public ExtendedTextField(IMetadata metadata, string fieldName)
            : base(metadata, fieldName) { }

        public override string Value
        {
            get => Metadata.ExtendedFields[FieldName].FirstOrDefault();
            set => base.Value = value;
        }

        protected override string Parse(string str) => str;

        public uint MaxLength => 0;
    }

    public class ExtendedListField : ExtendedMetadataField<IList<string>>, IListField
    {
        public ExtendedListField(IMetadata metadata, string fieldName)
            : base(metadata, fieldName) { }

        public override IList<string> Value
        {
            get => Metadata.ExtendedFields[FieldName];
            set => Metadata.ExtendedFields[FieldName] = value;
        }

        protected override IList<string> Parse(string str) => new List<string> { str };

        public uint MaxCount => 0;
        public uint MaxLength => 0;
    }

    public class ExtendedDateTimeField : ExtendedMetadataField<DateTime?>, IDateTimeField
    {
        public ExtendedDateTimeField(IMetadata metadata, string fieldName)
            : base(metadata, fieldName) { }

        public ushort? Year
        {
            get => (Value?.Year).NullableCast<int, ushort>();
            set
            {
                if (value != null)
                    Value = Value?.AddYears(value.Value - Value.Value.Year) ?? new DateTime(value.Value, 1, 1);
                // ReSharper disable once UseNullPropagation
                else if (Value != null)
                    Value = Value.Value.AddYears(Math.Min(-Value.Value.Year, 1));
            }
        }

        public byte? Month
        {
            get => (Value?.Month).NullableCast<int, byte>();
            set
            {
                if (value != null)
                    Value = Value?.AddMonths(value.Value - Value.Value.Month) ?? new DateTime(1, value.Value, 1);
                // ReSharper disable once UseNullPropagation
                else if (Value != null)
                    Value = Value.Value.AddMonths(Math.Min(-Value.Value.Month, 1));
            }
        }

        public byte? Day
        {
            get => (Value?.Day).NullableCast<int, byte>();
            set
            {
                if (value != null)
                    Value = Value?.AddDays(value.Value - Value.Value.Day) ?? new DateTime(1, 1, value.Value);
                // ReSharper disable once UseNullPropagation
                else if (Value != null)
                    Value = Value.Value.AddDays(Math.Min(-Value.Value.Day, 1));
            }
        }

        public byte? Hour
        {
            get => (Value?.Hour).NullableCast<int, byte>();
            set
            {
                if (value != null)
                    Value = Value?.AddHours(value.Value - Value.Value.Hour) ?? new DateTime(1, 1, 1, value.Value, 0, 0);
                // ReSharper disable once UseNullPropagation
                else if (Value != null)
                    Value = Value.Value.AddHours(-Value.Value.Hour);
            }
        }

        public byte? Minute
        {
            get => (Value?.Minute).NullableCast<int, byte>();
            set
            {
                if (value != null)
                    Value = Value?.AddMinutes(value.Value - Value.Value.Minute) ?? new DateTime(1, 1, 1, 0, value.Value, 0);
                // ReSharper disable once UseNullPropagation
                else if (Value != null)
                    Value = Value.Value.AddMinutes(-Value.Value.Minute);
            }
        }

        public byte? Second
        {
            get => (Value?.Second).NullableCast<int, byte>();
            set
            {
                if (value != null)
                    Value = Value?.AddSeconds(value.Value - Value.Value.Second) ?? new DateTime(1, 1, 1, 0, 0, value.Value);
                // ReSharper disable once UseNullPropagation
                else if (Value != null)
                    Value = Value.Value.AddSeconds(-Value.Value.Second);
            }
        }

        public byte MaxResolution => 6;

        protected override DateTime? Parse(string str) => str != null ? DateTime.Parse(str) : (DateTime?)null;

        public override string ToString()
        {
            return Value.ToString();
        }
    }

    public class ExtendedEnumField<T> : ExtendedMetadataField<T?>, IEnumField<T> where T : struct, Enum
    {
        public ExtendedEnumField(IMetadata metadata, string fieldName)
            : base(metadata, fieldName) { }
        
        protected override T? Parse(string str) => str != null ? (T)Enum.Parse(typeof(T), str, true) : (T?)null;

        public string StringValue { get => Format(Value); set => Value = Parse(value); }
    }

    public class ExtendedMusicalKeyField : ExtendedEnumField<MusicalKey>
    {
        public ExtendedMusicalKeyField(IMetadata metadata, string fieldName)
            : base(metadata, fieldName) { }

        protected override MusicalKey? Parse(string str) => MusicalKeyField.ParseMusicalKey(str);

        protected override string Format(MusicalKey? value) => MusicalKeyField.FormatMusicalKey(value);
    }
}
