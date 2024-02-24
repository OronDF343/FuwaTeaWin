using System;
using System.Collections;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace ModularFramework.Configuration
{
    public class ConfigurablePropertyInfo<TElement> : IConfigurablePropertyInfo
    {
        public ConfigurablePropertyInfo(PropertyInfo pi, ConfigurablePropertyAttribute cpa)
        {
            Name = cpa.Name ?? pi.DeclaringType?.FullName + "." + pi.Name;
            PropertyInfo = pi;
            DefaultValue = cpa.DefaultValue;
            _options = ElementType.GetProperties().FirstOrDefault(p => p.GetAttribute<PropertyOptionsEnumeratorAttribute>()?.PropertyName == Name && typeof(IEnumerable).IsAssignableFrom(p.PropertyType));
        }

        public string Name { get; }
        public object DefaultValue { get; set; }
        private readonly PropertyInfo _options;
        public IEnumerable ValidValues => (IEnumerable)_options.GetValue(BoundObject);
        public PropertyInfo PropertyInfo { get; }
        public Type ElementType => typeof(TElement);

        private TElement _boundObject;

        public object BoundObject
        {
            get { return _boundObject; }
            set { if (value is TElement) _boundObject = (TElement)value; }
        }

        private object _value;
        public object Value
        {
            get { return BoundObject == null ? _value : PropertyInfo.GetValue(BoundObject); }
            set
            {
                if (BoundObject != null) PropertyInfo.SetValue(BoundObject, value);
                else _value = value;
                OnPropertyChanged();
            }
        }

        public void SetWaitForInstance(IElementFactory source)
        {
            _source = source;
            _source.InstanceCreated += SourceOnInstanceCreated; 
        }

        private IElementFactory _source;
        private void SourceOnInstanceCreated(object sender, InstanceCreatedEventArgs args)
        {
            if (args.InstanceType != ElementType) return;
            BoundObject = args.NewInstance;
            Value = _value;
            _source.InstanceCreated -= SourceOnInstanceCreated;
        }

        public override bool Equals(object obj)
        {
            var info = obj as IConfigurablePropertyInfo;
            if (info != null)
                return Name.Equals(info.Name) && ElementType == info.ElementType;
            return false;
        }

        public override int GetHashCode()
        {
            return Name.GetHashCode();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
