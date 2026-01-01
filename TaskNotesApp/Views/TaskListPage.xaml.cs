using System.ComponentModel;

namespace TaskNotesApp.Views;

public partial class TaskListPage : ContentPage
{
    public TaskListPage()
    {
        InitializeComponent();
    }

    private void Entry_Focused(object sender, FocusEventArgs e)
    {
        if (sender is Entry entry && entry.Text != null)
        {
            entry.CursorPosition = entry.Text.Length;
        }
    }

    private void Entry_Loaded(object sender, EventArgs e)
    {
        if (sender is Entry entry)
        {
            MainThread.BeginInvokeOnMainThread(() =>
            {
                entry.Focus();
                entry.CursorPosition = entry.Text?.Length ?? 0;
            });
        }
    }


    private void Entry_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(VisualElement.IsVisible))
        {
            if (sender is Entry entry && entry.IsVisible)
            {
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



