using System.ComponentModel;

namespace TaskNotesApp.Views;

public partial class TaskListPage : ContentPage
{
    public TaskListPage()
    {
        InitializeComponent();
    }

    private async void Entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualElement.IsVisible))
        {
            if (sender is Entry entry && entry.IsVisible)
            {
                // REQUIRED for Windows MAUI
                await Task.Delay(50);

                MainThread.BeginInvokeOnMainThread(() =>
                {
                    entry.Focus();
                    entry.CursorPosition = entry.Text?.Length ?? 0;
                    entry.SelectionLength = 0;
                });
            }
        }
    }
}
