using SQLite;
using TaskNotesApp.Models;

namespace TaskNotesApp.Services;

public class DatabaseService
{
    private readonly SQLiteAsyncConnection _db;

    public DatabaseService(string path)
    {
        _db = new SQLiteAsyncConnection(path);
        
    }

    public async Task InitAsync()
    {
        await _db.CreateTableAsync<TaskItem>();
        
    }

    public Task<List<TaskItem>> GetTasksAsync() =>
        _db.Table<TaskItem>().OrderBy(t => t.Id).ToListAsync();

    public Task<int> SaveTaskAsync(TaskItem task)
    {
        return task.Id == 0
            ? _db.InsertAsync(task)
            : _db.UpdateAsync(task);
    }

    public Task<int> DeleteTaskAsync(TaskItem task) =>
        _db.DeleteAsync(task);
}
