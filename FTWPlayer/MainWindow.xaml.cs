using System;
using System.IO;
using System.Reflection;
using System.Windows;

namespace FTWPlayer
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            LayerFramework.LayerFactory.LoadFolder(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), Console.WriteLine);
            MessageBox.Show(LayerFramework.LayerFactory.GetFactory("Data").LayerName);
        }
    }
}
