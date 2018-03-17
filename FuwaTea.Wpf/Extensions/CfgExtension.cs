using System.ComponentModel;
using System.Configuration;
using System.Windows;
using FuwaTea.Wpf.Helpers;
using JetBrains.Annotations;
using XAMLMarkupExtensions.Base;

namespace FuwaTea.Wpf.Extensions
{
    public class CfgExtension : NestedMarkupExtension
    {
        public CfgExtension() { }

        public CfgExtension(string key)
        {
            Key = key;
        }

        private string _key;
        public string Key { get => _key; set { _key = value; UpdateNewValue(); } }

        private ApplicationSettingsBase _repo;

        public ApplicationSettingsBase Repository
        {
            get => _repo;
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

        [CanBeNull]
        public override object FormatOutput(TargetInfo endPoint, TargetInfo info)
        {
            if (_repo == null) return UpdateRepo(endPoint.TargetObject) ? Repository[Key] : null;
            return Repository[Key];
        }

        private bool UpdateRepo(object endPoint)
        {
            if (!(endPoint is DependencyObject)) return false;
            var ep = (DependencyObject)endPoint;
            while (_repo == null && ep != null)
            {
                _repo = ConfigSource.GetRepository(ep);
                ep = (ep as FrameworkElement)?.Parent;
            }
            if (_repo != null) _repo.PropertyChanged += RepoOnPropertyChanged;
            return _repo != null;
        }

        protected override bool UpdateOnEndpoint(TargetInfo endpoint)
        {
            return true;
        }

        public override string ToString()
        {
            return "Cfg:" + Key;
        }
    }
}
