using System;
using System.ComponentModel;
using System.Configuration;
using System.Globalization;
using System.Runtime.CompilerServices;
using System.Windows.Data;
using ModularFramework;

namespace FuwaTea.Wpf.Extensions
{
    public class CfgDynBinding : MultiBinding, INotifyPropertyChanged
    {
        public CfgDynBinding(BindingBase b)
        {
            Bindings.Add(b);
            Bindings.Add(new Binding("Value") { Source = this });
            Converter = new IndexerConverterProxy(this);
        }

        public object Value { get; set; }

        private ApplicationSettingsBase _repo;

        public ApplicationSettingsBase Repository
        {
            get { return _repo; }
            set
            {
                if (_repo != null) _repo.PropertyChanged -= RepoOnPropertyChanged;
                _repo = value;
                if (_repo != null) _repo.PropertyChanged += RepoOnPropertyChanged;
                UpdateNewValue();
            }
        }

        private void RepoOnPropertyChanged(object sender, PropertyChangedEventArgs args)
        {
            if (((IndexerConverterProxy)Converter).Key == args.PropertyName) UpdateNewValue();
        }

        private void UpdateNewValue()
        {
            OnPropertyChanged(nameof(Value));
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    internal class IndexerConverterProxy : IMultiValueConverter
    {
        internal IndexerConverterProxy(CfgDynBinding cb)
        {
            Binding = cb;
        }

        internal CfgDynBinding Binding { get; }

        internal string Key { get; private set; }

        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            return Binding.Repository[Key = (string)values[0]];
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            Binding.Repository[Key] = value;
            return new[] { Key, Binding.Value };
        }
    }
}
