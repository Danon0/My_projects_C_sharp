using System;

namespace TaskHub;

public class TaskItem : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public Priority Priority { get; set; } = Priority.Medium;
    public DateTime Deadline { get; set; }
    public Status Status { get; set; } = Status.New;
    public DateTime CreatedAt { get; set; } = DateTime.Now;

    public override string ToString()
    {
        return $"[{Id}] {Title} | {Priority} | {Status} | до {Deadline:dd.MM.yyyy HH:mm}";
    }
}