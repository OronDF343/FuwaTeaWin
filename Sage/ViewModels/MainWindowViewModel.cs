using System;
using Avalonia;
using Avalonia.Animation;
using Avalonia.Data;

namespace Sage.ViewModels
{
    public class MainWindowViewModel : ViewModelBase
    {
        public MainWindowViewModel()
        {

        }

        public string Message => "Hello World!";
        public double MinScrollingMargin => 50;
        public double ScrollingVelocity => 50;
        public TimeSpan Duration => TimeSpan.FromSeconds(3);
    }
}
