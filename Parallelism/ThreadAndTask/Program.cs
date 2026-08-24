namespace ThreadAndTask;

internal class Program
{
    protected Program() { }

    public static readonly Action printHelloWorld = () =>
    {
        Console.WriteLine("Hello World");

        // Task use Threadpool thread by default
        Console.WriteLine(Thread.CurrentThread.IsThreadPoolThread);
    };

    public static readonly Func<int> returnInt = () =>
    {
        return 1;
    };

    private static void Main(string[] args)
    {
        PrintActiveThreadInThreadPool();

        // Create a new thread and run the task
        Thread thread = new Thread(new ParameterizedThreadStart((obj) => printHelloWorld()));
        thread.Start();

        // Reuse the thread within the threadpool
        Task task = new Task(printHelloWorld);
        task.Start();

        // Return value Task
        var returnedValueTask = Task.Run(returnInt); // Create + start immediately
        var returnedValueTask1 = new Task<int>(returnInt); // Create, not started
        returnedValueTask1.Start();

        Console.WriteLine($"Returned Value Task: {returnedValueTask.Result}");
        Console.WriteLine($"Returned Value Task: {returnedValueTask1.Result}");

        // Force Main THread to stop, wait the thread to finish
        // Both are the same
        thread.Join();
        task.Wait();
        returnedValueTask.Wait();
        returnedValueTask1.Wait();

        PrintActiveThreadInThreadPool();
    }

    private static void PrintActiveThreadInThreadPool()
    {
        ThreadPool.GetMaxThreads(out int maxWorker, out int maxIO);
        ThreadPool.GetMinThreads(out int minWorker, out int minIO);
        ThreadPool.GetAvailableThreads(out int availWorker, out int availIO);

        // current active threads an working
        int activeWorker = maxWorker - availWorker;
        int activeIO = maxIO - availIO;

        Console.WriteLine($"--- ThreadPool Specs for this Machine ---");
        Console.WriteLine($"Min Threads (Baseline): Worker = {minWorker}, I/O = {minIO}");
        Console.WriteLine($"Max Threads (Ceiling) : Worker = {maxWorker}, I/O = {maxIO}");
        Console.WriteLine($"Active Right Now      : Worker = {activeWorker}, I/O = {activeIO}");
    }
}