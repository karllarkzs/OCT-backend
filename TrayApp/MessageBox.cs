using System.Threading.Tasks;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Layout;
using Avalonia.Media;

namespace PharmaBack.TrayApp;

public static class MessageBox
{
    public static async Task<bool> Show(Window parent, string message, string title)
    {
        var dialog = new Window
        {
            Title = title,
            Width = 300,
            Height = 150,
            WindowStartupLocation = WindowStartupLocation.CenterOwner,
            CanResize = false,
        };

        var yesButton = new Button { Content = "Yes", Width = 80 };
        var noButton = new Button { Content = "No", Width = 80 };

        var tcs = new TaskCompletionSource<bool>();

        yesButton.Click += (_, _) =>
        {
            tcs.SetResult(true);
            dialog.Close();
        };

        noButton.Click += (_, _) =>
        {
            tcs.SetResult(false);
            dialog.Close();
        };

        dialog.Content = new StackPanel
        {
            Spacing = 10,
            Margin = new Thickness(15),
            VerticalAlignment = VerticalAlignment.Center,
            Children =
            {
                new TextBlock
                {
                    Text = message,
                    FontSize = 14,
                    TextWrapping = TextWrapping.Wrap,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    TextAlignment = TextAlignment.Center,
                },
                new StackPanel
                {
                    Orientation = Orientation.Horizontal,
                    HorizontalAlignment = HorizontalAlignment.Center,
                    Spacing = 15,
                    Children = { yesButton, noButton },
                },
            },
        };

        await dialog.ShowDialog(parent);
        return await tcs.Task;
    }
}
