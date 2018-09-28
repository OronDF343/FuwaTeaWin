using System;
using System.Collections;
using System.Collections.Generic;
using TagLib;

namespace FuwaTea.Audio.Metadata
{
    public interface IPictureField
    {
        string MimeType { get; set; }
        PictureType PictureType { get; set; }
        string Description { get; set; }
        byte[] Data { get; set; }
    }

    public interface IMetadataField
    {
        Type ActualValueType { get; }
        object ActualValue { get; set; }
    }

    public interface ITextField : IMetadataField
    {
        string TextualValue { get; set; }
    }

    public interface IListField : IMetadataField
    {
        string[] StringValues { get; set; }
    }

    public interface IDateTimeField : IMetadataField
    {
        DateTime DateTimeValue { get; set; }
    }

    public interface INumericField : IMetadataField
    {
        uint NumericalValue { get; set; }
    }

    public interface IEnumField<T> : ITextField where T : struct, Enum
    {
        T EnumValue { get; set; }
    }

    public interface IMetadataField<T> : IMetadataField
    {
        new T ActualValue { get; set; }
    }
}
