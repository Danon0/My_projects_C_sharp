using System;
using System.Collections.Generic;

namespace TaskHub;

public class Repository<T> where T : IEntity
{
    private readonly Dictionary<int, T> _items = new();
    private readonly object _lock = new();
    private int _nextId = 1;

    public void Add(T item)
    {
        lock (_lock)
        {
            if (item is TaskItem task)
            {
                task.Id = _nextId++;
            }
            else if (item.Id == 0)
            {
                throw new ArgumentException("Объект должен иметь уникальный Id");
            }
            if (_items.ContainsKey(item.Id))
            {
                throw new InvalidOperationException($"Элемент с Id={item.Id} уже существует.");
            }
            _items[item.Id] = item;
        }
    }

    public bool Remove(int id)
    {
        lock (_lock)
        {
            return _items.Remove(id);
        }
    }

    public T? GetById(int id)
    {
        lock (_lock)
        {
            _items.TryGetValue(id, out T? value);
            return value;
        }
    }

    public IReadOnlyList<T> GetAll()
    {
        lock (_lock)
        {
            return new List<T>(_items.Values).AsReadOnly();
        }
    }

    public IReadOnlyList<T> Find(Predicate<T> match)
    {
        lock (_lock)
        {
            var result = new List<T>();
            foreach (var item in _items.Values)
            {
                if (match(item))
                {
                    result.Add(item);
                }
                    
            }
            return result.AsReadOnly();
        }
    }

    public int Count
    {
        get
        {
            lock (_lock)
            {
                return _items.Count;
            }
        }
    }

    public int NextId()
    {
        lock (_lock)
        {
            return _nextId;
        }
    }

    public void ReplaceAll(IEnumerable<T> items)
    {
        lock (_lock)
        {
            _items.Clear();
            int maxId = 0;
            foreach (var item in items)
            {
                _items[item.Id] = item;
                if (item.Id > maxId)
                {
                    maxId = item.Id;
                }
            }
            _nextId = maxId + 1;
        }
    }
}