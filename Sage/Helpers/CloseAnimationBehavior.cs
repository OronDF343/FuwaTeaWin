using Avalonia.Animation;
using Avalonia.Controls;
using Avalonia.Threading;
using Avalonia.Xaml.Interactivity;
using PropertyChanged;
using System.Threading.Tasks;

namespace Sage.Helpers;

[DoNotNotify]
public class CloseAnimationBehavior : Behavior<Window>
{
    protected override void OnAttached()
    {
        AssociatedObject.Closing += AssociatedObject_Closing;
        base.OnAttached();
    }

    private void AssociatedObject_Closing(object sender, WindowClosingEventArgs e)
    {
        // If the animation exists, cancel the event and run it to completion before closing.
        if (AssociatedObject.TryFindResource("CloseAnimation", out var resObj) && resObj is Animation anim)
        {
            // Unsubscribe from the event so this logic only happens once
            AssociatedObject.Closing -= AssociatedObject_Closing;
            // Cancel the event so the window doesn't close
            e.Cancel = true;
            // Start the animation on the current (UI) thread
            var animTask = anim.RunAsync(AssociatedObject);
            // Wait for it on a different thread
            Task.Run(async () =>
            {
                // Wait for the animation to complete
                await animTask;
                // Close the window from the UI thread
                Dispatcher.UIThread.Invoke(() => AssociatedObject.Close());
            });
        }
    }
}
