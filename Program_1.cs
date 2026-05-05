using System;
using System.Collections.Generic;

public interface IEntity
{
    int Id { get; }
}

public class Repository<T> where T : IEntity
{
    private readonly Dictionary<int, T> _items = new();

    public void Add(T item)
    {
        if (_items.ContainsKey(item.Id))
            throw new InvalidOperationException($"Элемент с Id={item.Id} уже существует.");
        _items[item.Id] = item;
    }

    public bool Remove(int id)
    {
        return _items.Remove(id);
    }

    public T? GetById(int id)
    {
        _items.TryGetValue(id, out T? value);
        return value;
    }

    public IReadOnlyList<T> GetAll()
    {
        return new List<T>(_items.Values).AsReadOnly();
    }

    public int Count => _items.Count;

    public IReadOnlyList<T> Find(Predicate<T> predicate)
    {
        var result = new List<T>();
        foreach (var item in _items.Values)
        {
            if (predicate(item))
                result.Add(item);
        }
        return result.AsReadOnly();
    }
}

public class Product : IEntity
{
    public int Id { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }

    public Product(int id, string name, decimal price)
    {
        Id = id;
        Name = name;
        Price = price;
    }

    public override string ToString() => $"Product(Id={Id}, Name=\"{Name}\", Price={Price})";
}

public class User : IEntity
{
    public int Id { get; set; }
    public string Username { get; set; }

    public User(int id, string username)
    {
        Id = id;
        Username = username;
    }

    public override string ToString() => $"User(Id={Id}, Username=\"{Username}\")";
}

public class Program_1
{
    public static void Main()
    {
        var productRepo = new Repository<Product>();
        productRepo.Add(new Product(1, "Ноутбук", 1500m));
        productRepo.Add(new Product(2, "Мышка", 50m));

        Product? p2 = productRepo.GetById(2);
        Console.WriteLine(p2);

        Console.WriteLine($"Количество продуктов: {productRepo.Count}");

        var expensive = productRepo.Find(p => p.Price > 1000);
        Console.WriteLine("Продукты дороже 1000:");
        foreach (var p in expensive)
            Console.WriteLine($"  {p}");

        try
        {
            productRepo.Add(new Product(1, "Другой ноутбук", 1600m));
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Ошибка: {ex.Message}");
        }

        var userRepo = new Repository<User>();
        userRepo.Add(new User(101, "alice"));
        userRepo.Add(new User(102, "bob"));
        Console.WriteLine(string.Join(", ", (IEnumerable<User>)userRepo.GetAll()));
    }
}
