Console.WriteLine("Hello, World!");

void WriteThreadId()
{
    for (int i = 0; i < 100; i++)
    {
        Console.WriteLine($"Thread: {Thread.CurrentThread.Name} | ID: {Thread.CurrentThread.ManagedThreadId}");
        Thread.Sleep(50);
    }
}

// Create another thread to run the callback method WriteThreadId
Thread thread1 = new Thread(WriteThreadId)
{
    Priority = ThreadPriority.Highest, // priority this thread to be picked up by the thread scheduler
    Name = "Thread 1"
};

Thread thread2 = new Thread(WriteThreadId)
{
    Priority = ThreadPriority.Lowest,
    Name = "Thread 2"
};

// Main Thread running this method
thread1.Start();
thread2.Start();
WriteThreadId();

