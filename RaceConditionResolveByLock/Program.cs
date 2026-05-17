int x = 10;

// Before .net 9
// object integerLock = new object();

// After .net 9
Lock integerLock = new Lock();

Thread thread1 = new Thread(() =>
{
    for (int i = 0; i < 100; i++)
    {
        // make sure this critical section is only excecuted by 1 thread at the time
        lock (integerLock)
        {
            x++;
            Console.WriteLine($"Thread 1: {x}");
        }
        Thread.Sleep(50);
    }
});

Thread thread2 = new Thread(() =>
{
    for (int i = 0; i < 100; i++)
    {
        /*
            Behind the scene 
            Thread thread2 = new Thread((ThreadStart)delegate
            {
                for (int i = 0; i < 100; i++)
                {
                    using (integerLock.EnterScope())
                    {
                        x++;
                        Console.WriteLine($"Thread 2: {x}");
                    }
                    Thread.Sleep(50);
                }
            });
        
        */

        lock (integerLock)
        {
            x++;
            Console.WriteLine($"Thread 2: {x}");
        }
        Thread.Sleep(50);
    }
});

thread1.Start();
thread2.Start();

thread1.Join();
thread2.Join();

Console.WriteLine($"Final value of x: {x}");


