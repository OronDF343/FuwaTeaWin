using System.Collections;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Media;
using JetBrains.Annotations;

namespace FuwaTea.Wpf.Behaviors
{
    public class DragDropRowBehavior : Behavior<ListView>
    {
        private object _draggedItem;
        private bool _isEditing;
        private bool _isDragging;

        #region DragEnded
        public static readonly RoutedEvent DragEndedEvent =
            EventManager.RegisterRoutedEvent("DragEnded", RoutingStrategy.Bubble, typeof(RoutedEventHandler), typeof(DragDropRowBehavior));
        public static void AddDragEndedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            var uie = d as UIElement;
            uie?.AddHandler(DragEndedEvent, handler);
        }
        public static void RemoveDragEndedHandler(DependencyObject d, RoutedEventHandler handler)
        {
            var uie = d as UIElement;
            uie?.RemoveHandler(DragEndedEvent, handler);
        }

        private void RaiseDragEndedEvent()
        {
            var args = new RoutedEventArgs(DragEndedEvent);
            AssociatedObject.RaiseEvent(args);
        }
        #endregion

        #region Popup
        public static readonly DependencyProperty PopupProperty =
            DependencyProperty.Register("Popup", typeof(Popup), typeof(DragDropRowBehavior));
        public Popup Popup
        {
            get => (Popup)GetValue(PopupProperty);
            set => SetValue(PopupProperty, value);
        }
        #endregion

        protected override void OnAttached()
        {
            base.OnAttached();
            
            AssociatedObject.MouseLeftButtonUp += OnMouseLeftButtonUp;
            AssociatedObject.PreviewMouseLeftButtonDown += OnMouseLeftButtonDown;
            AssociatedObject.MouseMove += OnMouseMove;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            
            AssociatedObject.MouseLeftButtonUp -= OnMouseLeftButtonUp;
            AssociatedObject.MouseLeftButtonDown -= OnMouseLeftButtonDown;
            AssociatedObject.MouseMove -= OnMouseMove;

            Popup = null;
            _draggedItem = null;
            _isEditing = false;
            _isDragging = false;
        }

        private void OnMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (_isEditing) return;

            var row = UIHelpers.TryFindFromPoint<ListViewItem>((UIElement)sender, e.GetPosition(AssociatedObject));
            if (row == null) return;

            //set flag that indicates we're capturing mouse movements
            _isDragging = true;
            _draggedItem = row.DataContext;
        }

        private void OnMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            if (!_isDragging || _isEditing)
                return;

            //get the target item
            var targetItem = AssociatedObject.SelectedItem;

            if (targetItem == null || !ReferenceEquals(_draggedItem, targetItem))
            {
                //get target index
                var targetIndex = (AssociatedObject.ItemsSource as IList).IndexOf(targetItem);

                //remove the source from the list
                (AssociatedObject.ItemsSource as IList).Remove(_draggedItem);

                //move source at the target's location
                (AssociatedObject.ItemsSource as IList).Insert(targetIndex, _draggedItem);

                //select the dropped item
                AssociatedObject.SelectedItem = _draggedItem;
                RaiseDragEndedEvent();
            }

            //reset
            ResetDragDrop();
        }

        private void ResetDragDrop()
        {
            _isDragging = false;
            Popup.IsOpen = false;
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (!_isDragging || e.LeftButton != MouseButtonState.Pressed)
                return;

            Popup.DataContext = _draggedItem;
            //display the popup if it hasn't been opened yet
            if (!Popup.IsOpen)
            {
                //make sure the popup is visible
                Popup.IsOpen = true;
            }

            var popupSize = new Size(Popup.ActualWidth, Popup.ActualHeight);
            Popup.PlacementRectangle = new Rect(e.GetPosition(AssociatedObject), popupSize);

            //make sure the row under the grid is being selected
            var position = e.GetPosition(AssociatedObject);
            var row = UIHelpers.TryFindFromPoint<ListViewItem>(AssociatedObject, position);
            if (row != null) AssociatedObject.SelectedItem = row.DataContext;
        }
    }

    public static class UIHelpers
    {

        #region find parent

        /// <summary>
        /// Finds a parent of a given item on the visual tree.
        /// </summary>
        /// <typeparam name="T">The type of the queried item.</typeparam>
        /// <param name="child">A direct or indirect child of the
        /// queried item.</param>
        /// <returns>The first parent item that matches the submitted
        /// type parameter. If not matching item can be found, a null
        /// reference is being returned.</returns>
        public static T TryFindParent<T>(DependencyObject child)
          where T : DependencyObject
        {
            //get parent item
            var parentObject = GetParentObject(child);

            //we've reached the end of the tree
            if (parentObject == null) return null;

            //check if the parent matches the type we're looking for
            var parent = parentObject as T;
            return parent ?? TryFindParent<T>(parentObject);
        }


        /// <summary>
        /// This method is an alternative to WPF's
        /// <see cref="VisualTreeHelper.GetParent"/> method, which also
        /// supports content elements. Do note, that for content element,
        /// this method falls back to the logical tree of the element.
        /// </summary>
        /// <param name="child">The item to be processed.</param>
        /// <returns>The submitted item's parent, if available. Otherwise
        /// null.</returns>
        [CanBeNull]
        public static DependencyObject GetParentObject([CanBeNull] DependencyObject child)
        {
            if (child == null) return null;
            var contentElement = child as ContentElement;

            if (contentElement == null) return VisualTreeHelper.GetParent(child);
            var parent = ContentOperations.GetParent(contentElement);
            if (parent != null) return parent;

            var fce = contentElement as FrameworkContentElement;
            return fce?.Parent;

            //if it's not a ContentElement, rely on VisualTreeHelper
        }

        #endregion

        /// <summary>
        /// Tries to locate a given item within the visual tree,
        /// starting with the dependency object at a given position. 
        /// </summary>
        /// <typeparam name="T">The type of the element to be found
        /// on the visual tree of the element at the given location.</typeparam>
        /// <param name="reference">The main element which is used to perform
        /// hit testing.</param>
        /// <param name="point">The position to be evaluated on the origin.</param>
        [CanBeNull]
        public static T TryFindFromPoint<T>(UIElement reference, Point point)
          where T : DependencyObject
        {
            var element = reference.InputHitTest(point)
                                         as DependencyObject;
            if (element == null) return null;
            var fromPoint = element as T;
            return fromPoint ?? TryFindParent<T>(element);
        }
    }
}
