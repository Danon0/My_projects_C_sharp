using System;
using System.Collections.Generic;

namespace TaskHub;

public static class TaskStats
{
    public static int Total(Repository<TaskItem> repo) => repo.Count;

    public static int Completed(Repository<TaskItem> repo) =>
        repo.Find(t => t.Status == Status.Done).Count;

    public static int Overdue(Repository<TaskItem> repo) =>
        repo.Find(t => t.Status != Status.Done && t.Deadline < DateTime.Now).Count;

    public static Dictionary<Priority, int> ByPriority(Repository<TaskItem> repo)
    {
        var dict = new Dictionary<Priority, int>();
        foreach (Priority p in Enum.GetValues(typeof(Priority)))
            dict[p] = 0;

        foreach (var t in repo.GetAll())
            dict[t.Priority]++;

        return dict;
    }
}