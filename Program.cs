using System;
using System.Threading.Tasks;

namespace TaskHub;

public static class Program
{
    private static Repository<TaskItem> repo = new();
    private static FileManager? fileManager;
    private static BackgroundDeadlineChecker? deadlineChecker;

    public static async Task Main(string[] args)
    {
        Console.WriteLine("TaskHub – Менеджер задач");
        fileManager = new FileManager("tasks.json");

        try
        {
            var loadedTasks = await fileManager.LoadAsync();
            repo.ReplaceAll(loadedTasks);
            Console.WriteLine($"Загружено {repo.Count} задач из файла.");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Не удалось загрузить данные: {ex.Message}");
        }

        deadlineChecker = new BackgroundDeadlineChecker(repo, message =>
        {
            var prevColor = Console.ForegroundColor;
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[ФОН] {DateTime.Now:T} {message}");
            Console.ForegroundColor = prevColor;
        });
        deadlineChecker.Start(TimeSpan.FromSeconds(5));

        while (true)
        {
            Console.WriteLine("\n========== МЕНЮ ==========");
            Console.WriteLine("1. Показать все задачи");
            Console.WriteLine("2. Показать выполненные");
            Console.WriteLine("3. Показать невыполненные");
            Console.WriteLine("4. Показать с высоким приоритетом");
            Console.WriteLine("5. Создать задачу");
            Console.WriteLine("6. Редактировать задачу");
            Console.WriteLine("7. Удалить задачу");
            Console.WriteLine("8. Поиск по названию");
            Console.WriteLine("9. Поиск по статусу");
            Console.WriteLine("10. Поиск по приоритету");
            Console.WriteLine("11. Статистика");
            Console.WriteLine("12. Сохранить в файл");
            Console.WriteLine("0. Выход");
            Console.Write("Ваш выбор: ");

            string? choice = Console.ReadLine();
            try
            {
                switch (choice)
                {
                    case "1": ShowTasks(repo.GetAll()); break;
                    case "2": ShowTasks(repo.Find(t => t.Status == Status.Done)); break;
                    case "3": ShowTasks(repo.Find(t => t.Status != Status.Done)); break;
                    case "4": ShowTasks(repo.Find(t => t.Priority == Priority.High)); break;
                    case "5": await CreateTask(); break;
                    case "6": await EditTask(); break;
                    case "7": await DeleteTask(); break;
                    case "8": SearchByTitle(); break;
                    case "9": SearchByStatus(); break;
                    case "10": SearchByPriority(); break;
                    case "11": ShowStatistics(); break;
                    case "12": await SaveTasks(); break;
                    case "0":
                        deadlineChecker.Stop();
                        fileManager.Dispose();
                        Console.WriteLine("До свидания!");
                        return;
                    default:
                        Console.WriteLine("Неверный пункт меню.");
                        break;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Ошибка: {ex.Message}");
            }
        }
    }

    private static void ShowTasks(IReadOnlyList<TaskItem> tasks)
    {
        if (tasks.Count == 0)
        {
            Console.WriteLine("Задачи не найдены.");
            return;
        }
        foreach (var t in tasks)
            Console.WriteLine(t);
    }

    private static async Task CreateTask()
    {
        Console.Write("Название: ");
        string title = Console.ReadLine() ?? "Без названия";
        Console.Write("Описание: ");
        string desc = Console.ReadLine() ?? "";
        Console.Write("Приоритет (Low/Medium/High): ");
        if (!Enum.TryParse(Console.ReadLine(), true, out Priority prio))
        {
            prio = Priority.Medium;
        }
        Console.Write("Дедлайн (дд.ММ.гггг ЧЧ:мм): ");
        DateTime deadline;
        while (!DateTime.TryParse(Console.ReadLine(), out deadline))
        {
            Console.Write("Неверный формат. Попробуйте ещё раз: ");
        }
        Console.Write("Статус (New/InProgress/Done): ");
        if (!Enum.TryParse(Console.ReadLine(), true, out Status st))
        {
            st = Status.New;
        }
        var task = new TaskItem
        {
            Title = title,
            Description = desc,
            Priority = prio,
            Deadline = deadline,
            Status = st,
            CreatedAt = DateTime.Now
        };

        repo.Add(task);
        Console.WriteLine($"Задача создана с Id={task.Id}");
    }

    private static async Task EditTask()
    {
        Console.Write("Введите Id задачи: ");
        if (!int.TryParse(Console.ReadLine(), out int id))
        {
            Console.WriteLine("Неверный Id");
            return;
        }
        var task = repo.GetById(id);
        if (task == null)
        {
            Console.WriteLine("Задача не найдена.");
            return;
        }

        Console.Write($"Название ({task.Title}): ");
        string? title = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(title))
        {
            task.Title = title;
        }
        Console.Write($"Описание ({task.Description}): ");
        string? desc = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(desc))
        {
            task.Description = desc;
        }
        Console.Write($"Приоритет ({task.Priority}): ");
        string? prioStr = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(prioStr) && Enum.TryParse(prioStr, true, out Priority prio))
        {
            task.Priority = prio;
        }
        Console.Write($"Дедлайн ({task.Deadline:dd.MM.yyyy HH:mm}): ");
        string? dateStr = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(dateStr) && DateTime.TryParse(dateStr, out DateTime dt))
        {
            task.Deadline = dt;
        }
        Console.Write($"Статус ({task.Status}): ");
        string? statusStr = Console.ReadLine();
        if (!string.IsNullOrWhiteSpace(statusStr) && Enum.TryParse(statusStr, true, out Status st))
        {
            task.Status = st;
        }
        Console.WriteLine("Задача обновлена.");
    }

    private static async Task DeleteTask()
    {
        Console.Write("Id задачи для удаления: ");
        if (int.TryParse(Console.ReadLine(), out int id) && repo.Remove(id))
        {
            Console.WriteLine("Удалено.");
        }
        else
        {
            Console.WriteLine("Не удалось удалить задачу (возможно, неверный Id).");
        }
    }

    private static void SearchByTitle()
    {
        Console.Write("Часть названия: ");
        string? part = Console.ReadLine()?.ToLower();
        var result = repo.Find(t => t.Title.ToLower().Contains(part ?? ""));
        ShowTasks(result);
    }

    private static void SearchByStatus()
    {
        Console.Write("Статус (New/InProgress/Done): ");
        if (Enum.TryParse(Console.ReadLine(), true, out Status st))
        {
            var result = repo.Find(t => t.Status == st);
            ShowTasks(result);
        }
        else
        {
            Console.WriteLine("Неверный статус.");
        }
    }

    private static void SearchByPriority()
    {
        Console.Write("Приоритет (Low/Medium/High): ");
        if (Enum.TryParse(Console.ReadLine(), true, out Priority p))
        {
            var result = repo.Find(t => t.Priority == p);
            ShowTasks(result);
        }
        else
        {
            Console.WriteLine("Неверный приоритет.");
        }
    }

    private static void ShowStatistics()
    {
        Console.WriteLine("=== СТАТИСТИКА ===");
        Console.WriteLine($"Всего задач: {TaskStats.Total(repo)}");
        Console.WriteLine($"Выполнено: {TaskStats.Completed(repo)}");
        Console.WriteLine($"Просрочено: {TaskStats.Overdue(repo)}");
        var byPri = TaskStats.ByPriority(repo);
        Console.WriteLine("По приоритетам:");
        foreach (var kv in byPri)
            Console.WriteLine($"  {kv.Key}: {kv.Value}");
    }

    private static async Task SaveTasks()
    {
        if (fileManager == null)
        {
            return;
        }
        await fileManager.SaveAsync(repo);
        Console.WriteLine("Данные сохранены в файл.");
    }
}