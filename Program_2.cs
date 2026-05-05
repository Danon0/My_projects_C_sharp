using System;
using System.Collections.Generic;

public static class CollectionUtils
{
    public static List<T> Distinct<T>(List<T> source)
    {
        if (source == null) {
          throw new ArgumentNullException(nameof(source));
        }
        var seen = new HashSet<T>();
        var result = new List<T>();
        foreach (var item in source)
        {
            if (seen.Add(item))
                result.Add(item);
        }
        return result;
    }

    public static Dictionary<TKey, List<TValue>> GroupBy<TValue, TKey>(
        List<TValue> source,
        Func<TValue, TKey> keySelector) where TKey : notnull
    {
        if (source == null) {
          throw new ArgumentNullException(nameof(source));
        }
        if (keySelector == null) {
          throw new ArgumentNullException(nameof(keySelector));
        }
        var groups = new Dictionary<TKey, List<TValue>>();
        foreach (var item in source)
        {
            TKey key = keySelector(item);
            if (!groups.TryGetValue(key, out var list))
            {
                list = new List<TValue>();
                groups[key] = list;
            }
            list.Add(item);
        }
        return groups;
    }

    public static Dictionary<TKey, TValue> Merge<TKey, TValue>(
        Dictionary<TKey, TValue> first,
        Dictionary<TKey, TValue> second,
        Func<TValue, TValue, TValue> conflictResolver) where TKey : notnull
    {
        if (first == null) {
          throw new ArgumentNullException(nameof(first));
        }
        if (second == null) {
          throw new ArgumentNullException(nameof(second));
        }
        if (conflictResolver == null) {
          throw new ArgumentNullException(nameof(conflictResolver));
        }
        var merged = new Dictionary<TKey, TValue>(first);
        foreach (var kvp in second)
        {
            if (merged.TryGetValue(kvp.Key, out var existingValue))
            {
                merged[kvp.Key] = conflictResolver(existingValue, kvp.Value);
            }
            else
            {
                merged[kvp.Key] = kvp.Value;
            }
        }
        return merged;
    }

    public static T MaxBy<T, TKey>(List<T> source, Func<T, TKey> selector)
        where TKey : IComparable<TKey>
    {
        if (source == null) {
          throw new ArgumentNullException(nameof(source));
        }
        if (source.Count == 0) {
          throw new InvalidOperationException("Коллекция пуста.");
        }
        if (selector == null) {
          throw new ArgumentNullException(nameof(selector));
        }
        T maxItem = source[0];
        TKey maxKey = selector(maxItem);
        for (int i = 1; i < source.Count; i++)
        {
            T current = source[i];
            TKey currentKey = selector(current);
            if (currentKey.CompareTo(maxKey) > 0)
            {
                maxItem = current;
                maxKey = currentKey;
            }
        }
        return maxItem;
    }
}

public class Product
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

public class Program_2
{
    public static void Main()
    {
        var intList = new List<int> { 1, 3, 2, 3, 1, 4, 2 };
        var distinctInts = CollectionUtils.Distinct(intList);
        Console.WriteLine("Distinct ints: " + string.Join(", ", distinctInts));

        var strList = new List<string> { "apple", "banana", "apple", "cherry", "banana" };
        var distinctStrs = CollectionUtils.Distinct(strList);
        Console.WriteLine("Distinct strings: " + string.Join(", ", distinctStrs));

        var words = new List<string> { "а", "аа", "ааа", "б", "бб", "ббб", "в", "вв", "ввв" };
        var groupsByLength = CollectionUtils.GroupBy(words, w => w.Length);
        foreach (var g in groupsByLength)
        {
            Console.WriteLine($"Длина {g.Key}: [{string.Join(", ", g.Value)}]");
        }

        var dict1 = new Dictionary<string, int> { ["a"] = 1, ["b"] = 2, ["c"] = 3 };
        var dict2 = new Dictionary<string, int> { ["b"] = 10, ["c"] = 20, ["d"] = 40 };
        var merged = CollectionUtils.Merge(dict1, dict2, (v1, v2) => v1 + v2);
        foreach (var kv in merged)
            Console.WriteLine($"{kv.Key}: {kv.Value}");

        var products = new List<Product>
        {
            new Product(1, "Товар А", 500),
            new Product(2, "Товар Б", 1200),
        };
      
        Product mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
        Console.WriteLine(mostExpensive);

        try
        {
            CollectionUtils.MaxBy(new List<Product>(), p => p.Price);
        }
        catch (InvalidOperationException ex)
        {
            Console.WriteLine($"Ошибка MaxBy для пустого списка: {ex.Message}");
        }
    }
}
