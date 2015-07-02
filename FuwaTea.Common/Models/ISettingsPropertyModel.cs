using System;
using System.ComponentModel;

namespace FuwaTea.Common.Models
{
    public interface ISettingsPropertyModel : INotifyPropertyChanged
    {
        string Name { get; }

        object DefaultValue { get; set; }

        object PropertyValue { get; set; }

        bool IsDirty { get; set; }

        bool IsDefaultValue { get; set; }

        Type PropertyType { get; set; }

        bool IsReadOnly { get; set; }
    }
}
