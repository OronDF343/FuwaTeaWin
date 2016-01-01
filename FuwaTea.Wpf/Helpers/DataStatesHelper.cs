using System;
using System.Linq;
using System.Windows;
using System.Windows.Markup;

namespace FuwaTea.Wpf.Helpers
{
    [ContentProperty(nameof(States))]
    public class DataStatesHelper : DependencyObject
    {
        public static readonly DependencyProperty HelperObjectProperty = DependencyProperty.RegisterAttached(
                                                                "HelperObject", typeof(DataStatesHelper), typeof(DataStatesHelper),
                                                                new PropertyMetadata(new DataStatesHelper()));

        public static void SetHelperObject(DependencyObject element, DataStatesHelper value)
        {
            element.SetValue(HelperObjectProperty, value);
        }

        public static DataStatesHelper GetHelperObject(DependencyObject element)
        {
            return (DataStatesHelper)element.GetValue(HelperObjectProperty);
        }

        public static readonly DependencyProperty BindingProperty = DependencyProperty.Register(
                                                        "Binding", typeof(object), typeof(DataStatesHelper), new PropertyMetadata(null, OnBindingUpdated));

        protected static void OnBindingUpdated(DependencyObject obj, DependencyPropertyChangedEventArgs args)
        {
            var dsb = (DataStatesHelper)obj;
            dsb.CurrentState = dsb.States.FirstOrDefault(s => s.DataType.IsInstanceOfType(args.NewValue) && s.Value.Equals(args.NewValue));
        }

        public object Binding { get { return GetValue(BindingProperty); } set { SetValue(BindingProperty, value); } }

        private static readonly DependencyPropertyKey CurrentStatePropertyKey = DependencyProperty.RegisterReadOnly(
                                                        "CurrentState", typeof(State), typeof(DataStatesHelper), new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentStateProperty = CurrentStatePropertyKey.DependencyProperty;

        public State CurrentState
        {
            get
            {
                return (State)GetValue(CurrentStateProperty);
            }
            protected set
            {
                SetValue(CurrentStatePropertyKey, value);
                SetValue(CurrentStateNamePropertyKey, value.StateName);
            }
        }

        private static readonly DependencyPropertyKey CurrentStateNamePropertyKey = DependencyProperty.RegisterReadOnly(
                                                        "CurrentStateName", typeof(string), typeof(DataStatesHelper), new PropertyMetadata(null));

        public static readonly DependencyProperty CurrentStateNameProperty =
            CurrentStateNamePropertyKey.DependencyProperty;

        public string CurrentStateName => (string)GetValue(CurrentStateNameProperty);

        private static readonly DependencyPropertyKey StatesPropertyKey = DependencyProperty.RegisterReadOnly(
                                                        "States", typeof(FreezableCollection<State>), typeof(DataStatesHelper), new PropertyMetadata(new FreezableCollection<State>()));

        public static readonly DependencyProperty StatesProperty = StatesPropertyKey.DependencyProperty;

        public FreezableCollection<State> States => (FreezableCollection<State>)GetValue(StatesProperty);
    }

    [ContentProperty(nameof(Value))]
    public class State : Freezable
    {
        public static readonly DependencyProperty StateNameProperty = DependencyProperty.Register(
                                                        "StateName", typeof(string), typeof(State), new PropertyMetadata(default(string)));

        public string StateName { get { return (string)GetValue(StateNameProperty); } set { SetValue(StateNameProperty, value); } }

        public static readonly DependencyProperty DataTypeProperty = DependencyProperty.Register(
                                                        "DataType", typeof(Type), typeof(State), new PropertyMetadata(default(Type)));

        public Type DataType { get { return (Type)GetValue(DataTypeProperty); } set { SetValue(DataTypeProperty, value); } }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(
                                                        "Value", typeof(object), typeof(State), new PropertyMetadata(default(object)));

        public object Value { get { return GetValue(ValueProperty); } set { SetValue(ValueProperty, value); } }

        protected override Freezable CreateInstanceCore()
        {
            return new State();
        }
    }
}