using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Interop;
using FTWPlayer.ViewModel;
using FuwaTea.Lib;
using Application = System.Windows.Application;

namespace FTWPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : IDisposable
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

        #region NotifyIcon

        private NotifyIcon _controlNotifyIcon;
        private void InitNotifyIcon()
        {
            // TODO: Clean up old code
            _controlNotifyIcon = new NotifyIcon();
            var iconContextMenuStrip = new ContextMenuStrip();
            var playPauseToolStripMenuItem = new ToolStripMenuItem();
            var previousToolStripMenuItem = new ToolStripMenuItem();
            var nextToolStripMenuItem = new ToolStripMenuItem();
            var stopToolStripMenuItem = new ToolStripMenuItem();
            var toolStripSeparator1 = new ToolStripSeparator();
            var showGuiToolStripMenuItem = new ToolStripMenuItem();
            var exitToolStripMenuItem = new ToolStripMenuItem();

            _controlNotifyIcon.BalloonTipIcon = ToolTipIcon.Info;
            _controlNotifyIcon.BalloonTipTitle = "Project Mousou";
            _controlNotifyIcon.ContextMenuStrip = iconContextMenuStrip;
            _controlNotifyIcon.Text = "Project Mousou";
            _controlNotifyIcon.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Assembly.GetExecutingAssembly().Location); // TODO: this is temporary
            _controlNotifyIcon.Visible = true;
            _controlNotifyIcon.DoubleClick += (sender, e) => Show();

            iconContextMenuStrip.Items.AddRange(new ToolStripItem[] {
            playPauseToolStripMenuItem,
            previousToolStripMenuItem,
            nextToolStripMenuItem,
            stopToolStripMenuItem,
            toolStripSeparator1,
            showGuiToolStripMenuItem,
            exitToolStripMenuItem});
            iconContextMenuStrip.Name = "iconContextMenuStrip";
            iconContextMenuStrip.Size = new System.Drawing.Size(178, 164);
            // 
            // playPauseToolStripMenuItem
            // 
            playPauseToolStripMenuItem.Name = "playPauseToolStripMenuItem";
            playPauseToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            playPauseToolStripMenuItem.Text = "Play";
            //playPauseToolStripMenuItem.Click += PlayButton_Click;
            // 
            // previousToolStripMenuItem
            // 
            previousToolStripMenuItem.Name = "previousToolStripMenuItem";
            previousToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            previousToolStripMenuItem.Text = "Previous";
            //previousToolStripMenuItem.Click += PreviousButton_Click;
            // 
            // nextToolStripMenuItem
            // 
            nextToolStripMenuItem.Name = "nextToolStripMenuItem";
            nextToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            nextToolStripMenuItem.Text = "Next";
            //nextToolStripMenuItem.Click += NextButton_Click;
            // 
            // stopToolStripMenuItem
            // 
            stopToolStripMenuItem.Name = "stopToolStripMenuItem";
            stopToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            stopToolStripMenuItem.Text = "Stop";
            //stopToolStripMenuItem.Click += StopButton_Click;
            // 
            // toolStripSeparator1
            // 
            toolStripSeparator1.Name = "toolStripSeparator1";
            toolStripSeparator1.Size = new System.Drawing.Size(174, 6);
            // 
            // showGuiToolStripMenuItem
            // 
            showGuiToolStripMenuItem.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold);
            showGuiToolStripMenuItem.Name = "showGuiToolStripMenuItem";
            showGuiToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            showGuiToolStripMenuItem.Text = "Show GUI";
            //showMiniPlayerToolStripMenuItem.Click += ShowButton_Click;
            // 
            // exitToolStripMenuItem
            // 
            exitToolStripMenuItem.Name = "exitToolStripMenuItem";
            exitToolStripMenuItem.Size = new System.Drawing.Size(177, 22);
            exitToolStripMenuItem.Text = "Exit";
            exitToolStripMenuItem.Click += (sender, args) => CloseButton_OnClick(sender, new RoutedEventArgs());
        }
        #endregion

        #region Single Instance Application

        private void MainWindow_OnSourceInitialized(object sender, EventArgs e)
        {
            // Single Instance Application hook
            var source = (HwndSource)PresentationSource.FromVisual(this);
            source.AddHook(HwndSourceHook); // should never be null
            // Continue:
            InitNotifyIcon();
            //LocalizeDictionary.Instance.Culture = CultureInfo.CreateSpecificCulture("he");
            //FlowDirectionUpdater.UpdateFlowDirection(FlowDirection.RightToLeft);
        }

        private IntPtr HwndSourceHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == ((App)Application.Current).Message)
            {
                Show();
                var clArgs = File.ReadAllLines(Path.Combine(Assembly.GetEntryAssembly().GetExeFolder(), @"ClArgs.txt")).ToList(); // TODO: find better way of passing args
                // TODO: better handling of clArgs
                // TODO: finish this
                MiscUtils.ParseClArgs(clArgs);
            }
            return IntPtr.Zero;
        }
        #endregion

        private void MinimizeButton_OnClick(object sender, RoutedEventArgs e)
        {
            Hide();
        }

        // Last
        private void CloseButton_OnClick(object sender, RoutedEventArgs e)
        {
            Close();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;
            if (_controlNotifyIcon == null) return;
            _controlNotifyIcon.Visible = false;
            _controlNotifyIcon.Dispose();
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        private void MainWindow_OnClosed(object sender, EventArgs e)
        {
            Dispose();
        }
    }
}