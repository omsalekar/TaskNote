using SQLite;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace TaskNotesApp.Models;

public class TaskItem : INotifyPropertyChanged
{
    [PrimaryKey, AutoIncrement]
    public int Id { get; set; }

    string _title;
    public string Title
    {
        get => _title;
        set
        {
            if (_title == value) return;
            _title = value;
            OnPropertyChanged();
            OnPropertyChanged(nameof(HasChanges));  
        }
    }

    bool _isCompleted;
    [Ignore]
    public bool IsCompleted
    {
        get => _isCompleted;
        set
        {
            _isCompleted = value;
            OnPropertyChanged();
        }
    }

   
    bool _isEditing;
    [Ignore]
    public bool IsEditing
    {
        get => _isEditing;
        set
        {
            _isEditing = value;
            OnPropertyChanged();
        }
    }

    [Ignore]
    public string OriginalTitle { get; set; }

    [Ignore]
    public bool HasChanges =>
        !string.Equals(Title, OriginalTitle, StringComparison.Ordinal);

    public event PropertyChangedEventHandler PropertyChanged;
    void OnPropertyChanged([CallerMemberName] string name = null) =>
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));


    public bool IsModified => Title != OriginalTitle;
}
