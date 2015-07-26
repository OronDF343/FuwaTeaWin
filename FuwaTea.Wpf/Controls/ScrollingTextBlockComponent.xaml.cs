#region License
//     This file is part of FuwaTeaWin.
// 
//     FuwaTeaWin is free software: you can redistribute it and/or modify
//     it under the terms of the GNU General Public License as published by
//     the Free Software Foundation, either version 3 of the License, or
//     (at your option) any later version.
// 
//     FuwaTeaWin is distributed in the hope that it will be useful,
//     but WITHOUT ANY WARRANTY; without even the implied warranty of
//     MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//     GNU General Public License for more details.
// 
//     You should have received a copy of the GNU General Public License
//     along with FuwaTeaWin.  If not, see <http://www.gnu.org/licenses/>.
#endregion

using System;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace FuwaTea.Wpf.Controls
{
    /// <summary>
    /// Put this in a <see cref="Canvas"/> with ClipToBounds="True"
    /// </summary>
    public partial class ScrollingTextBlockComponent : TextBlock
    {
        public ScrollingTextBlockComponent()
        {
            InitializeComponent();
            var dpd = DependencyPropertyDescriptor.FromProperty(FlowDirectionProperty, typeof(ScrollingTextBlockComponent));
            if (dpd != null)
            {
                dpd.AddValueChanged(this, delegate
                {
                    var tx = GetValue(ScrollingTextProperty);
                    ScrollingTextChanged(this, new DependencyPropertyChangedEventArgs(ScrollingTextProperty, tx, tx));
                });
            }
        }

        public static readonly DependencyProperty ScrollingTextProperty = DependencyProperty.Register("ScrollingText", typeof(string),
                                                                                                      typeof(ScrollingTextBlockComponent),
                                                                                                      new UIPropertyMetadata(ScrollingTextChanged));

        public string ScrollingText { get { return (string)GetValue(ScrollingTextProperty); } set { SetValue(ScrollingTextProperty, value); } }

        public static readonly DependencyProperty DelayModifierProperty = DependencyProperty.Register("DelayModifier", typeof(double),
                                                                                                      typeof(ScrollingTextBlockComponent),
                                                                                                      new UIPropertyMetadata(1d));

        public double DelayModifier { get { return (double)GetValue(DelayModifierProperty); } set { SetValue(DelayModifierProperty, value); } }


        public static readonly DependencyProperty TargetWidthProperty = DependencyProperty.Register("TargetWidth", typeof(int),
                                                                                                    typeof(ScrollingTextBlockComponent),
                                                                                                    new UIPropertyMetadata(100));

        public int TargetWidth { get { return (int)GetValue(TargetWidthProperty); } set { SetValue(TargetWidthProperty, value); } }

        public static readonly DependencyProperty SpacingProperty = DependencyProperty.Register("Spacing", typeof(int),
                                                                                                typeof(ScrollingTextBlockComponent),
                                                                                                new UIPropertyMetadata(7));

        public int Spacing { get { return (int)GetValue(SpacingProperty); } set { SetValue(SpacingProperty, value); } }

        private static void ScrollingTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var tbox = d as ScrollingTextBlockComponent;
            if (tbox == null) return;
            if (DesignerProperties.GetIsInDesignMode(tbox)) { tbox.Text = (string)e.NewValue; return; }
            tbox.Text = (string)e.NewValue;
            if (tbox.DelayModifier <= 0) return;

            var typeface = new Typeface(tbox.FontFamily, tbox.FontStyle, tbox.FontWeight, tbox.FontStretch);

            var copy = tbox.Text.PadLeft(tbox.Text.Length + tbox.Spacing);
            var textGraphicalWidth =
                new FormattedText(copy, CultureInfo.CurrentCulture, tbox.FlowDirection, typeface, tbox.FontSize,
                                  tbox.Foreground).WidthIncludingTrailingWhitespace;
            double textLengthGraphicalWidth = 0;
            
            while (textLengthGraphicalWidth < tbox.TargetWidth)
            {
                tbox.Text += copy;
                textLengthGraphicalWidth =
                    new FormattedText(tbox.Text, CultureInfo.CurrentCulture, tbox.FlowDirection, typeface, tbox.FontSize,
                                      tbox.Foreground).WidthIncludingTrailingWhitespace;
            }
            tbox.Text += tbox.Text.PadLeft(tbox.Text.Length + tbox.Spacing);
            textLengthGraphicalWidth =
                new FormattedText(tbox.Text, CultureInfo.CurrentCulture, tbox.FlowDirection, typeface, tbox.FontSize,
                                  tbox.Foreground).WidthIncludingTrailingWhitespace;
            // If the text direction should be different from the flowdirection, correct the animation
            // FlowDirection doesn't affect the animation at all because of this, but it does affect the text itself
            // FD  Str rev
            // RTL RTL n
            // RTL LTR y
            // LTR RTL y
            // LTR LTR n
            var reverse = IsRtlString((string)e.NewValue) ^ tbox.FlowDirection == FlowDirection.RightToLeft;
            var doubleAnimation = new DoubleAnimation
            {
                From = reverse ? -textGraphicalWidth : 0,
                To = reverse ? 0 : -textGraphicalWidth,
                RepeatBehavior = RepeatBehavior.Forever,
                Duration = new Duration(TimeSpan.FromSeconds(Round(textLengthGraphicalWidth * tbox.DelayModifier)))
            };
            tbox.TranslateTransform.BeginAnimation(TranslateTransform.XProperty, doubleAnimation);
        }

        public static double Round(double src)
        {
            return double.Parse(src.ToString("F2"));
        }

        public static bool IsRtlString(string s)
        {
            if (s.Length < 1) return false;
            var c = char.ConvertToUtf32(s, 0);
            if (c >= 0x5BE && c <= 0x10B7F)
            {
                if (c <= 0x85E)
                {
                    if (c == 0x5BE) return true;
                    if (c == 0x5C0) return true;
                    if (c == 0x5C3) return true;
                    if (c == 0x5C6) return true;
                    if (0x5D0 <= c && c <= 0x5EA) return true;
                    if (0x5F0 <= c && c <= 0x5F4) return true;
                    if (c == 0x608) return true;
                    if (c == 0x60B) return true;
                    if (c == 0x60D) return true;
                    if (c == 0x61B) return true;
                    if (0x61E <= c && c <= 0x64A) return true;
                    if (0x66D <= c && c <= 0x66F) return true;
                    if (0x671 <= c && c <= 0x6D5) return true;
                    if (0x6E5 <= c && c <= 0x6E6) return true;
                    if (0x6EE <= c && c <= 0x6EF) return true;
                    if (0x6FA <= c && c <= 0x70D) return true;
                    if (c == 0x710) return true;
                    if (0x712 <= c && c <= 0x72F) return true;
                    if (0x74D <= c && c <= 0x7A5) return true;
                    if (c == 0x7B1) return true;
                    if (0x7C0 <= c && c <= 0x7EA) return true;
                    if (0x7F4 <= c && c <= 0x7F5) return true;
                    if (c == 0x7FA) return true;
                    if (0x800 <= c && c <= 0x815) return true;
                    if (c == 0x81A) return true;
                    if (c == 0x824) return true;
                    if (c == 0x828) return true;
                    if (0x830 <= c && c <= 0x83E) return true;
                    if (0x840 <= c && c <= 0x858) return true;
                    if (c == 0x85E) return true;
                }
                else if (c == 0x200F) return true;
                else if (c >= 0xFB1D)
                {
                    if (c == 0xFB1D) return true;
                    if (0xFB1F <= c && c <= 0xFB28) return true;
                    if (0xFB2A <= c && c <= 0xFB36) return true;
                    if (0xFB38 <= c && c <= 0xFB3C) return true;
                    if (c == 0xFB3E) return true;
                    if (0xFB40 <= c && c <= 0xFB41) return true;
                    if (0xFB43 <= c && c <= 0xFB44) return true;
                    if (0xFB46 <= c && c <= 0xFBC1) return true;
                    if (0xFBD3 <= c && c <= 0xFD3D) return true;
                    if (0xFD50 <= c && c <= 0xFD8F) return true;
                    if (0xFD92 <= c && c <= 0xFDC7) return true;
                    if (0xFDF0 <= c && c <= 0xFDFC) return true;
                    if (0xFE70 <= c && c <= 0xFE74) return true;
                    if (0xFE76 <= c && c <= 0xFEFC) return true;
                    if (0x10800 <= c && c <= 0x10805) return true;
                    if (c == 0x10808) return true;
                    if (0x1080A <= c && c <= 0x10835) return true;
                    if (0x10837 <= c && c <= 0x10838) return true;
                    if (c == 0x1083C) return true;
                    if (0x1083F <= c && c <= 0x10855) return true;
                    if (0x10857 <= c && c <= 0x1085F) return true;
                    if (0x10900 <= c && c <= 0x1091B) return true;
                    if (0x10920 <= c && c <= 0x10939) return true;
                    if (c == 0x1093F) return true;
                    if (c == 0x10A00) return true;
                    if (0x10A10 <= c && c <= 0x10A13) return true;
                    if (0x10A15 <= c && c <= 0x10A17) return true;
                    if (0x10A19 <= c && c <= 0x10A33) return true;
                    if (0x10A40 <= c && c <= 0x10A47) return true;
                    if (0x10A50 <= c && c <= 0x10A58) return true;
                    if (0x10A60 <= c && c <= 0x10A7F) return true;
                    if (0x10B00 <= c && c <= 0x10B35) return true;
                    if (0x10B40 <= c && c <= 0x10B55) return true;
                    if (0x10B58 <= c && c <= 0x10B72) return true;
                    if (0x10B78 <= c && c <= 0x10B7F) return true;
                }
            }
            return false;
        }
    }
}
