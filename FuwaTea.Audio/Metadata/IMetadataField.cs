using System;
using System.Collections;
using System.Collections.Generic;

namespace FuwaTea.Audio.Metadata
{
    public interface IMetadataField
    {
        Type ValueType { get; }
        IEnumerable Value { get; set; }
        uint MaxCount { get; }
    }

    public interface IMetadataField<T> : IMetadataField
    {
        new ICollection<T> Value { get; set; }
    }

    public abstract class MetadataFieldBase<T> : IMetadataField<T>
    {
        public MetadataFieldBase(uint maxCount = 0)
        {
            Value = new List<T>();
            MaxCount = maxCount;
        }

        public MetadataFieldBase(ICollection<T> value, uint maxCount = 0)
        {
            Value = value;
            MaxCount = maxCount;
        }

        public MetadataFieldBase(T value, uint maxCount = 1) : this(maxCount)
        {
            Value.Add(value);
        }

        public virtual Type ValueType => typeof(T);
        public virtual ICollection<T> Value { get; set; }
        public virtual uint MaxCount { get; }

        IEnumerable IMetadataField.Value
        {
            get => Value;
            set => Value = (ICollection<T>)value;
        }
    }
}
