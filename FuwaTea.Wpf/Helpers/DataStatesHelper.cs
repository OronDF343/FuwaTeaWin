using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Markup;
using JetBrains.Annotations;
using log4net;

namespace FuwaTea.Wpf.Helpers
{
    public class DataStatesHelper
    {
        public static readonly DependencyProperty BindingProperty = DependencyProperty.RegisterAttached(
                                                        "Binding", typeof(object), typeof(DataStatesHelper), new PropertyMetadata(null, OnBindingUpdated));

        protected static void OnBindingUpdated(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var states = GetStates(obj);
            var current = states?.Where(s => Equals(s.Value, args.NewValue) || BoundsCheck(args.NewValue, s.MinValue, s.MaxValue)).ToList();
            SetCurrentState(obj, current?.FirstOrDefault());
            SetStateChecker(obj, current == null ? new StateChecker() : new StateChecker(current));
        }

        private static bool BoundsCheck(object v, object min, object max)
        {
            try
            {
                var cv = v as IComparable;
                var cmin = min as IComparable;
                var cmax = max as IComparable;
                if (cmin == null && cmax == null) return false;
                if (cv == null) return false;
                var res = false;
                if (cmin != null) res = cmin.CompareTo(cv) <= 0;
                if (cmax != null) res = res && cmax.CompareTo(cv) > 0;
                return res;
            }
            catch (Exception e)
            {
                LogManager.GetLogger(typeof(DataStatesHelper)).Warn("Min/max value comparison error: ", e);
                return false;
            }
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

        public static readonly DependencyProperty StateCheckerProperty = DependencyProperty.RegisterAttached(
                                                                "StateChecker", typeof(StateChecker), typeof(DataStatesHelper),
                                                                new PropertyMetadata(default(StateChecker)));

        public static void SetStateChecker(DependencyObject element, StateChecker value)
        {
            element.SetValue(StateCheckerProperty, value);
        }

        public static StateChecker GetStateChecker(DependencyObject element)
        {
            return (StateChecker)element.GetValue(StateCheckerProperty);
        }
    }

    public class StateChecker
    {
        private readonly HashSet<State> _states;

        public StateChecker()
        {
            _states = new HashSet<State>();
        }

        public StateChecker(IEnumerable<State> states)
        {
            _states = new HashSet<State>(states);
        }

        public bool this[string s]
        {
            get { return _states.Any(x => x.StateName.Equals(s, StringComparison.OrdinalIgnoreCase)); }
        }
    }

    public class StateCollection : FreezableCollection<State> { }
    
    [ContentProperty(nameof(Value))]
    // This attribute doesn't work. Hasn't been fixed in 8 years... (Microsoft :disappointed:)
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

        public static readonly DependencyProperty MinValueProperty = DependencyProperty.Register(
                                                        "MinValue", typeof(object), typeof(State), new PropertyMetadata(null));

        public object MinValue { get { return GetValue(MinValueProperty); } set { SetValue(MinValueProperty, value); } }

        public static readonly DependencyProperty MaxValueProperty = DependencyProperty.Register(
                                                        "MaxValue", typeof(object), typeof(State), new PropertyMetadata(null));

        public object MaxValue { get { return GetValue(MaxValueProperty); } set { SetValue(MaxValueProperty, value); } }

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