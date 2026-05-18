using System;
using System.Threading;
using System.Threading.Tasks;

namespace TaskHub;

public class BackgroundDeadlineChecker
{
    private readonly Repository<TaskItem> _repo;
    private readonly Action<string> _onOverdueNotification;
    private readonly CancellationTokenSource _cts;
    private Task? _runningTask;

    public BackgroundDeadlineChecker(Repository<TaskItem> repo, Action<string> notificationCallback)
    {
        _repo = repo;
        _onOverdueNotification = notificationCallback;
        _cts = new CancellationTokenSource();
    }

    public void Start(TimeSpan checkInterval)
    {
        _runningTask = Task.Run(async () =>
        {
            while (!_cts.Token.IsCancellationRequested)
            {
                try
                {
                    await Task.Delay(checkInterval, _cts.Token);
                    var overdue = _repo.Find(t => t.Status != Status.Done && t.Deadline < DateTime.Now);
                    if (overdue.Count > 0)
                    {
                        foreach (var task in overdue)
                        {
                            _onOverdueNotification($"ПРОСРОЧЕНА: {task}");
                        }
                    }
                }
                catch (OperationCanceledException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    _onOverdueNotification($"Ошибка проверки: {ex.Message}");
                }
            }
        }, _cts.Token);
    }

    public void Stop()
    {
        _cts.Cancel();
        _runningTask?.Wait();
    }
}