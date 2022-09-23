using System.Collections.Concurrent;

public class QueueThrottler
{
    private Queue<Task<object>> AllTasks { get; set; }
    private ConcurrentQueue<Task<object>> ActiveTasks { get; set; }

    private readonly int _limit;

    public QueueThrottler(int limit)
    {
        _limit = limit;
        AllTasks = new Queue<Task<object>>();
        ActiveTasks = new ConcurrentQueue<Task<object>>();
    }
    
    public void Add(Task<object?> task)
    {
        AllTasks.Enqueue(task);
        while (ActiveTasks.Count <= _limit)
        {
            if (AllTasks.TryDequeue(out var toActiveTasks))
            {
                Console.WriteLine($"now {ActiveTasks.Count} in active tasks");
                toActiveTasks.Start();
                ActiveTasks.Enqueue(toActiveTasks);
            }
            else
            {
                break;
            }
        }
    }

    public Task<object> WaitFirstTaskCompleted()
    {
        while (true)
        {
            if (ActiveTasks.FirstOrDefault()?.Status == TaskStatus.RanToCompletion)
            {
                if (ActiveTasks.TryDequeue(out var result))
                {
                    return result;
                }
            }
        }
    }
}