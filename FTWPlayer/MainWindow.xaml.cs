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
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Interop;
using FTWPlayer.ViewModel;
using FuwaTea.Lib;

namespace FTWPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        /// <summary>
        /// Initializes a new instance of the MainWindow class.
        /// </summary>
        public MainWindow()
        {
            InitializeComponent();
            Closing += (s, e) => ViewModelLocator.Cleanup();
            // testing
            Application.Current.Resources.MergedDictionaries.Add(new ResourceDictionary { Source = new Uri("pack://application:,,,/Skins/GlacierSkin.xaml") });
        }

        #region Single Instance Application

        private void MainWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            // Single Instance Application hook
            var source = (HwndSource)PresentationSource.FromVisual(this);
            source.AddHook(HwndSourceHook); // should never be null
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == ((App)Application.Current).Message)
            {
                Show();
                var clArgs = File.ReadAllLines(Path.Combine(Assembly.GetExecutingAssembly().GetUserDataPath(), @"ClArgs.txt")).ToList(); // TODO: find better way of passing args
                // TODO: better handling of clArgs
                // TODO: finish this
                MiscUtils.ParseClArgs(clArgs);
            }
            return IntPtr.Zero;
        }
        #endregion

        private void MaximizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            Show();
        }

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        // Last
        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}