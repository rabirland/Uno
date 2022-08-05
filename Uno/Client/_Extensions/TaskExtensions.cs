namespace Uno.Client;

/// <summary>
/// Extension methods for <see cref="Task"/>.
/// </summary>
public static class TaskExtensions
{
    /// <summary>
    /// Attaches a continuation to a task that checks if the task has faulted, and if yes, stops the debugger and logs the exception.
    /// </summary>
    /// <param name="task">The task to check.</param>
    /// <param name="logger">The logger.</param>
    public static void CatchExceptions(this Task task, ILogger? logger = null)
    {
        task.ContinueWith(task =>
        {
            Console.WriteLine(task.Exception);
            Console.WriteLine(task.Exception?.StackTrace);

            if (task.IsFaulted)
            {
                if (logger  != null)
                {
                    logger.LogError(task.Exception, "Task has faulted");
                }
#if DEBUG
                System.Diagnostics.Debugger.Break();
#endif
            }
        });
    }
}
