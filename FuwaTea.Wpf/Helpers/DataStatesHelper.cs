using System;
using System.Linq;
using System.Windows;
using System.Windows.Data;
using System.Windows.Markup;
using ModularFramework;

namespace FuwaTea.Wpf.Helpers
{
    public class DataStatesHelper
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.RegisterAttached(
                                                        "Binding", typeof(object), typeof(DataStatesHelper), new PropertyMetadata(null, OnBindingUpdated));

        protected static void OnBindingUpdated(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var states = GetStates(obj);
            SetCurrentState(obj, states?.FirstOrDefault(s => Equals(s.Value, args.NewValue)));
        }

        [CanBeNull]
        public static object GetBinding(DependencyObject d)
        {
            return d.GetValue(BindingProperty);
        }

        public static void SetBinding(DependencyObject d, [CanBeNull] object value)
        {
            d.SetValue(BindingProperty, value);
        }

        private static readonly DependencyPropertyKey CurrentStatePropertyKey = DependencyProperty.RegisterAttachedReadOnly(
                                                        "CurrentState", typeof(State), typeof(DataStatesHelper), new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentStateProperty = CurrentStatePropertyKey.DependencyProperty;

        [CanBeNull]
        public static State GetCurrentState(DependencyObject d)
        {
            return (State)d.GetValue(CurrentStateProperty);
        }

        protected static void SetCurrentState(DependencyObject d, [CanBeNull] State value)
        {
            d.SetValue(CurrentStatePropertyKey, value);
            d.SetValue(CurrentStateNamePropertyKey, value?.StateName);
        }

        private static readonly DependencyPropertyKey CurrentStateNamePropertyKey = DependencyProperty.RegisterAttachedReadOnly(
                                                        "CurrentStateName", typeof(string), typeof(DataStatesHelper), new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentStateNameProperty =
            CurrentStateNamePropertyKey.DependencyProperty;

        [CanBeNull]
        public static string GetCurrentStateName(DependencyObject d)
        {
            return (string)d.GetValue(CurrentStateNameProperty);
        }

        public static readonly DependencyProperty StatesProperty = DependencyProperty.RegisterAttached(
                                                        "States", typeof(StateCollection), typeof(DataStatesHelper), new PropertyMetadata(null));

        [CanBeNull]
        public static StateCollection GetStates(DependencyObject d)
        {
            return (StateCollection)d.GetValue(StatesProperty);
        }

        public static void SetStates(DependencyObject d, StateCollection value)
        {
            d.SetValue(StatesProperty, value);
        }
    }

    public class StateCollection : FreezableCollection<State> { }
    
    [ContentProperty(nameof(Value))]
    [DictionaryKeyProperty(nameof(StateName))]
    public class State : Freezable
    {
        public static readonly DependencyProperty StateNameProperty = DependencyProperty.Register(
                                                        "StateName", typeof(string), typeof(State), new PropertyMetadata(""));

        public string StateName { get { return (string)GetValue(StateNameProperty); } set { SetValue(StateNameProperty, value); } }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                                                        "Value", typeof(object), typeof(State), new PropertyMetadata(null));

        [CanBeNull]
        public object Value { get { return GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        public static readonly DependencyProperty TagProperty = DependencyProperty.Register(
                                                        "Tag", typeof(object), typeof(State), new PropertyMetadata(null));

        [CanBeNull]
        public object Tag { get { return GetValue(TagProperty); } set { SetValue(TagProperty, value); } }

        protected override Freezable CreateInstanceCore()
        {
            return new State();
        }
    }
}