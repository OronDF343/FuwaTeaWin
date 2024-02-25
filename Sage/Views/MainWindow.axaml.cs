using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using PropertyChanged;
using Sage.ViewModels;

namespace Sage.Views
{
    [DoNotNotify]
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            DataContext = new MainWindowViewModel();
            //this.AttachDevTools();
        }

        private void InitializeComponent()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
