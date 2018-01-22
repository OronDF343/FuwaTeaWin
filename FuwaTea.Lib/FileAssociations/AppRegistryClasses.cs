using System.Collections;
using System.Collections.Generic;
using System.Linq;
using JetBrains.Annotations;
using Microsoft.Win32;

namespace FuwaTea.Lib.FileAssociations
{
    public class AppRegistryClasses : ICollection<RegistryClass>
    {
        private readonly List<RegistryClass> _registryClasses = new List<RegistryClass>();

        public string AppId { get; }
        public AppRegistryClasses(string appId)
        {
            AppId = appId;
            _registryClasses.AddRange(Registry.ClassesRoot.GetSubKeyNames().Where(s => s.StartsWith(appId + ".")).Select(s => new RegistryClass(s)));
        }

        public void UpdateAll()
        {
            foreach (var c in _registryClasses)
                c.WriteValues();
        }

        [CanBeNull]
        public RegistryClass GetKey(string suffix)
        {
            return _registryClasses.FirstOrDefault(rc => rc.ClassName == AppId + "." + suffix);
        }

        public bool RemoveKey(string suffix)
        {
            return _registryClasses.Remove(GetKey(suffix));
        }

        public IEnumerable<string> Keys => _registryClasses.Select(RegistryUtils.GetSuffix);

        public IEnumerator<RegistryClass> GetEnumerator()
        {
            return _registryClasses.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }

        public void Add(RegistryClass item)
        {
            _registryClasses.Add(item);
        }

        public void Clear()
        {
            foreach (var c in _registryClasses)
                c.Delete();
            _registryClasses.Clear();
        }

        public bool Contains(RegistryClass item)
        {
            return _registryClasses.Contains(item);
        }

        public void CopyTo(RegistryClass[] array, int arrayIndex)
        {
            _registryClasses.CopyTo(array, arrayIndex);
        }

        public bool Remove(RegistryClass item)
        {
            if (!_registryClasses.Remove(item)) return false;
            item.Delete();
            return true;
        }

        public int Count => _registryClasses.Count;
        public bool IsReadOnly => false;
    }
}
