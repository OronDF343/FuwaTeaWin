using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using PropertyChanged;
using Sage.ViewModels;
using Sage.Views;

namespace Sage
{
    [DoNotNotify]
    public class App : Application
    {
        public override void OnFrameworkInitializationCompleted()
        {
            if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
                desktop.MainWindow = new MainWindow { DataContext = new MainWindowViewModel() };
            //else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
                //singleView.MainView = new MainView();
            base.OnFrameworkInitializationCompleted();
        }

        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
