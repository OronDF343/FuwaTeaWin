﻿using System.ComponentModel;
using System.Configuration;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using ModularFramework;

namespace FuwaTea.Wpf.Extensions
{
    [ContentProperty(nameof(Key))]
    public class CfgBinding : Binding, INotifyPropertyChanged
    {
        public CfgBinding()
        {
            Path = new PropertyPath(nameof(Value));
            Source = this;
        }

        public CfgBinding(string key) : this()
        {
            Key = key;
        }

        private string _key;
        public string Key
        {
            get { return _key; }
            set
            {
                if (_key == value) return;
                _key = value;
                UpdateNewValue();
            }
        }

        private object _value;
        public object Value
        {
            get { return _value; }
            set
            {
                if (_value == value) return;
                _value = value;
                OnPropertyChanged();
            }
        }

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
            if (args.PropertyName == Key) UpdateNewValue();
        }

        private void UpdateNewValue()
        {
            Value = _repo == null || _key == null ? null : Repository[Key];
        }

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            var handler = PropertyChanged;
            handler?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
