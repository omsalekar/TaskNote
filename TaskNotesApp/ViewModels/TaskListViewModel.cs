using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using TaskNotesApp.Models;

namespace TaskNotesApp.ViewModels;

public class TaskListViewModel : BaseViewModel
{
    public ObservableCollection<TaskItem> Tasks { get; } = new();

    private string _newTaskTitle;
    public string NewTaskTitle
    {
        get => _newTaskTitle;
        set
        {
            _newTaskTitle = value;
            OnPropertyChanged();
        }
    }

    public ICommand AddCommand { get; }
    public ICommand DeleteCommand { get; }
    public ICommand SaveCommand { get; }
    public ICommand SelectAllCommand { get; }
    public ICommand DeleteAllCommand { get; }


    public TaskListViewModel()
    {
        AddCommand = new Command(async () => await AddTask());
        DeleteCommand = new Command<TaskItem>(async t => await DeleteTask(t));
        SaveCommand = new Command<TaskItem>(async t => await SaveTask(t));
        SelectAllCommand = new Command(ToggleSelectAll);
        DeleteAllCommand = new Command(async () => await DeleteAll());


        LoadTasks();
    }

    private void ToggleSelectAll()
    {
        bool selectAll = Tasks.Any(t => !t.IsCompleted);

        foreach (var task in Tasks)
        {
            task.IsCompleted = selectAll;
        }
    }

    private async Task DeleteAll()
    {
        // VALIDATION 1: No tasks at all
        if (!Tasks.Any())
        {
            await Application.Current.MainPage.DisplayAlert(
                "No Tasks",
                "There are no tasks to delete.",
                "OK");
            return;
        }

        // VALIDATION 2: Not all tasks selected
        if (!Tasks.All(t => t.IsCompleted))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Action Not Allowed",
                "Please select all tasks before using Delete All.",
                "OK");
            return;
        }

        // CONFIRMATION
        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete",
            "All tasks are selected. Do you want to delete everything?",
            "Yes",
            "No");

        if (!confirm)
            return;

        // DELETE ALL
        foreach (var task in Tasks.ToList())
        {
            await App.Database.DeleteTaskAsync(task);
            Tasks.Remove(task);
        }
    }




    private async void LoadTasks()
    {
        Tasks.Clear();
        var items = await App.Database.GetTasksAsync();

        foreach (var item in items)
        {
            item.OriginalTitle = item.Title;

            // Listen for checkbox changes
            item.PropertyChanged += Task_PropertyChanged;

            Tasks.Add(item);
        }

        OnPropertyChanged(nameof(AreAllTasksCompleted));
    }

    private void Task_PropertyChanged(object sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName == nameof(TaskItem.IsCompleted))
        {
            OnPropertyChanged(nameof(AreAllTasksCompleted));
        }
    }



    private async Task AddTask()
    {
        if (string.IsNullOrWhiteSpace(NewTaskTitle))
        {
            await Application.Current.MainPage.DisplayAlert(
                "Warning",
                "Please enter a task before adding.",
                "OK");
            return;
        }

        var task = new TaskItem { Title = NewTaskTitle };
        await App.Database.SaveTaskAsync(task);

        NewTaskTitle = string.Empty;
        LoadTasks();

        await Application.Current.MainPage.DisplayAlert(
            "Success",
            "Task added successfully.",
            "OK");
    }

    private async Task DeleteTask(TaskItem task)
    {
        if (task == null)
            return;

        if (!task.IsCompleted)
        {
            await Application.Current.MainPage.DisplayAlert(
                "Action Not Allowed",
                "Please complete the task first before deleting it.",
                "OK");
            return;
        }

        bool confirm = await Application.Current.MainPage.DisplayAlert(
            "Confirm Delete",
            "Do you want to delete this task?",
            "Yes",
            "No");

        if (!confirm)
            return;

        await App.Database.DeleteTaskAsync(task);
        Tasks.Remove(task);
    }

    private async Task SaveTask(TaskItem task)
    {
        if (task == null)
            return;

        // ENTER EDIT MODE
        if (!task.IsEditing)
        {
            foreach (var t in Tasks)
                t.IsEditing = false;

            task.OriginalTitle = task.Title;
            task.IsCompleted = false;
            task.IsEditing = true;
            return;
        }

        // CANCEL IF EMPTY
        if (string.IsNullOrWhiteSpace(task.Title))
        {
            task.Title = task.OriginalTitle;
            task.IsEditing = false;
            return;
        }

        // SAVE ONLY IF CHANGED
        if (!task.HasChanges)
        {
            task.IsEditing = false;
            return;
        }

        await App.Database.SaveTaskAsync(task);
        task.OriginalTitle = task.Title;
        task.IsEditing = false;

        await Application.Current.MainPage.DisplayAlert(
            "Updated",
            "Task updated successfully.",
            "OK");
    }

    public bool AreAllTasksCompleted =>
    Tasks.Any() && Tasks.All(t => t.IsCompleted);

}
