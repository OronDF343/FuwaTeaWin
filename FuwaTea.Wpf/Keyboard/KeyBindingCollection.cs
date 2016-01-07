using System;
using System.Collections.ObjectModel;
using System.Xml.Serialization;

namespace FuwaTea.Wpf.Keyboard
{
    [Serializable]
    public class KeyBindingCollection
    {
        public KeyBindingCollection()
        {
            Items = new ObservableCollection<KeyBinding>();
        }

        public KeyBindingCollection(ObservableCollection<KeyBinding> items)
        {
            Items = items;
        }

        [XmlArray(nameof(Items))]
        [XmlArrayItem(nameof(KeyBinding))]
        public ObservableCollection<KeyBinding> Items { get; }
    }
}
