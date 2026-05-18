using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace TaskHub;

public class FileManager : IDisposable
{
    private readonly string _path;
    private FileStream? _fileStream;
    private bool _disposed;

    public FileManager(string path)
    {
        _path = path;
    }

    public async Task SaveAsync(Repository<TaskItem> repo)
    {
        var tasks = repo.GetAll();
        var json = JsonSerializer.Serialize(tasks, new JsonSerializerOptions { WriteIndented = true });
        await File.WriteAllTextAsync(_path, json);
    }

    public async Task<List<TaskItem>> LoadAsync()
    {
        if (!File.Exists(_path))
        {
            return new List<TaskItem>();
        }
        string json = await File.ReadAllTextAsync(_path);
        return JsonSerializer.Deserialize<List<TaskItem>>(json) ?? new List<TaskItem>();
    }

    public void OpenStream()
    {
        _fileStream = new FileStream(_path, FileMode.OpenOrCreate, FileAccess.ReadWrite, FileShare.None);
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            _fileStream?.Dispose();
            _fileStream = null;
            _disposed = true;
        }
    }
}