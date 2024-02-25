/* The MIT License (MIT)
   
   Copyright (c) 2019 Wiesław Šoltés
   
   Permission is hereby granted, free of charge, to any person obtaining a copy
   of this software and associated documentation files (the "Software"), to deal
   in the Software without restriction, including without limitation the rights
   to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
   copies of the Software, and to permit persons to whom the Software is
   furnished to do so, subject to the following conditions:
   
   The above copyright notice and this permission notice shall be included in all
   copies or substantial portions of the Software.
   
   THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
   IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
   FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
   AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
   LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
   OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
   SOFTWARE.
 */

using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Xaml.Interactivity;
using PropertyChanged;

namespace Sage.Helpers
{
    [DoNotNotify]
    public class DropBehavior : Behavior<Control>
    {
        public static readonly StyledProperty<IDropHandler> HandlerProperty =
            AvaloniaProperty.Register<DropBehavior, IDropHandler>(nameof(Handler));

        public static readonly StyledProperty<bool> IsEnabledProperty =
            AvaloniaProperty.RegisterAttached<Control, bool>("IsEnabled", typeof(DropBehavior), true, true, BindingMode.TwoWay);

        public IDropHandler Handler
        {
            get => GetValue(HandlerProperty);
            set => SetValue(HandlerProperty, value);
        }

        public static bool GetIsEnabled(Control control)
        {
            return control.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(Control control, bool value)
        {
            control.SetValue(IsEnabledProperty, value);
        }

        /// <inheritdoc/>
        protected override void OnAttached()
        {
            base.OnAttached();
            DragDrop.SetAllowDrop(AssociatedObject, true);
            AssociatedObject.AddHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.AddHandler(DragDrop.DragLeaveEvent, DragLeave);
            AssociatedObject.AddHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.AddHandler(DragDrop.DropEvent, Drop);
        }

        /// <inheritdoc/>
        protected override void OnDetaching()
        {
            base.OnDetaching();
            DragDrop.SetAllowDrop(AssociatedObject, false);
            AssociatedObject.RemoveHandler(DragDrop.DragEnterEvent, DragEnter);
            AssociatedObject.RemoveHandler(DragDrop.DragLeaveEvent, DragLeave);
            AssociatedObject.RemoveHandler(DragDrop.DragOverEvent, DragOver);
            AssociatedObject.RemoveHandler(DragDrop.DropEvent, Drop);
        }

        private void DragEnter(object sender, DragEventArgs e)
        {
            if (GetIsEnabled(AssociatedObject)) Handler?.Enter(sender, e);
        }

        private void DragLeave(object sender, RoutedEventArgs e)
        {
            if (GetIsEnabled(AssociatedObject)) Handler?.Leave(sender, e);
        }

        private void DragOver(object sender, DragEventArgs e)
        {
            if (GetIsEnabled(AssociatedObject)) Handler?.Over(sender, e);
        }

        private void Drop(object sender, DragEventArgs e)
        {
            if (GetIsEnabled(AssociatedObject)) Handler?.Drop(sender, e);
        }
    }

    public interface IDropHandler
    {
        void Enter(object sender, DragEventArgs e);
        void Leave(object sender, RoutedEventArgs e);
        void Over(object sender, DragEventArgs e);
        void Drop(object sender, DragEventArgs e);
    }
}
