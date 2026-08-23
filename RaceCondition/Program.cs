int x = 10;

Thread thread1 = new Thread(() =>
{
    for (int i = 0; i < 100; i++)
    {
        x++;
        Console.WriteLine($"Thread 1: {x}");
        Thread.Sleep(500);
    }
});

Thread thread2 = new Thread(() =>
{
    for (int i = 0; i < 100; i++)
    {
        x++;
        Console.WriteLine($"Thread 2: {x}");
        Thread.Sleep(500);

        /*
            Behind the scene of x++ operation, it is not atomic, it is actually 3 steps:

            int temp = x;       // read
            temp = temp + 1;    // modify
            x = temp;           // write
        */
    }
});

thread1.Start();
thread2.Start();

thread1.Join();
thread2.Join();

// The result is not 210 as expected
// - suppose t1 is current in the write step,
// - t2 is current in the read step, then the value of x in t2 is different than t1.
// - hence the final value of x is not 210, but less than 210, which is a race condition
Console.WriteLine($"Final value of x: {x}");