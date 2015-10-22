using System.Windows;

namespace FTWPlayer
{
    public class WindowSizeParameters
    {
        public static readonly DependencyProperty CompactHeightProperty = DependencyProperty.RegisterAttached("CompactHeight",
                                                                                                              typeof(double),
                                                                                                              typeof(WindowSizeParameters),
                                                                                                              new PropertyMetadata(default(double)));

        public static void SetCompactHeight(DependencyObject element, double value)
        {
            element.SetValue(CompactHeightProperty, value);
        }

        public static double GetCompactHeight(DependencyObject element)
        {
            return (double)element.GetValue(CompactHeightProperty);
        }

        public static readonly DependencyProperty TotalCompactHeightProperty = DependencyProperty.RegisterAttached("TotalCompactHeight",
                                                                                                                   typeof(double),
                                                                                                                   typeof(WindowSizeParameters),
                                                                                                                   new PropertyMetadata(default(double)));

        public static void SetTotalCompactHeight(DependencyObject element, double value)
        {
            element.SetValue(TotalCompactHeightProperty, value);
        }

        public static double GetTotalCompactHeight(DependencyObject element)
        {
            return (double)element.GetValue(TotalCompactHeightProperty);
        }

        public static readonly DependencyProperty BottomHeightProperty = DependencyProperty.RegisterAttached("BottomHeight",
                                                                                                             typeof(double),
                                                                                                             typeof(WindowSizeParameters),
                                                                                                             new PropertyMetadata(default(double)));

        public static void SetBottomHeight(DependencyObject element, double value)
        {
            element.SetValue(BottomHeightProperty, value);
        }

        public static double GetBottomHeight(DependencyObject element)
        {
            return (double)element.GetValue(BottomHeightProperty);
        }
    }
}
