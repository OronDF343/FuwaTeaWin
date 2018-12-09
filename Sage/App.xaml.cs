using Avalonia;
using Avalonia.Markup.Xaml;

namespace Sage
{
    public class App : Application
    {
        public override void Initialize()
        {
            AvaloniaXamlLoader.Load(this);
        }
    }
}
