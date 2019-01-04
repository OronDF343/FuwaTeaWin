﻿using System.Collections;
using System.Collections.ObjectModel;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows.Controls;
using System.Windows.Input;
using FTWPlayer.Views.SettingsViews;
using FuwaTea.Lib.Collections;
using GalaSoft.MvvmLight.CommandWpf;

namespace FTWPlayer.ViewModels.SettingsViewModels
{
    //[UIPart("Key Bindings settings tab")]
    public class KeyBindingsViewModel : ISettingsTab
    {
        public KeyBindingsViewModel([Import] UISettings settings)
        {
            Settings = settings;
        }

        public TabItem GetTabItem()
        {
            RemoveItemCommand = new RelayCommand<object[]>(RemoveItem);
            AddItemCommand = new RelayCommand<object[]>(AddItem);
            return new KeyBindingsView(this);
        }

        public decimal Index => 3;
        public UISettings Settings { get; }

        public ICommand AddItemCommand { get; private set; }

        public void AddItem(object[] context)
        {
            if (context[0] == null || context[1] == null) return;
            var set = context[0] as ObservableHashSet<Key>;
            set?.Add((Key)context[1]);
            var set2 = context[0] as ObservableCollection<string>;
            if (string.IsNullOrWhiteSpace(context[1] as string)) return;
            set2?.Add((string)context[1]);
        }

        public ICommand RemoveItemCommand { get; private set; }

        public void RemoveItem(object[] context)
        {
            if (context[0] == null || context[1] == null) return;
            var set = context[0] as ObservableHashSet<Key>;
            var set2 = context[0] as ObservableCollection<string>;
            foreach (var item in ((IList)context[1]).OfType<Key>().ToList()) set?.RemoveSlow(item);
            foreach (var item in ((IList)context[1]).OfType<string>().ToList()) set2?.Remove(item);
        }
    }
}
