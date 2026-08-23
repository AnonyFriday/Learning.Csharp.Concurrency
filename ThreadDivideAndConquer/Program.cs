internal class Program
{
    private static void Main(string[] args)
    {
        // Using big array to demonstrate the overlapping of those threads
        // which demonstrate the race condition
        int[] arr = Enumerable.Range(1, 1_000_000).ToArray();

        Synchronous(arr);

        // As you can see, since multipl thread is gonna read and write the same variable sum1
        // which raise the concern of race condition
        // Thread 1 ─┐
        // Thread 2 ─┼──> sum1
        // Thread 3 ─┘
        ThreadWithRaceCondition(arr, 10);

        /*
            Thread 1 → localSum1
            Thread 2 → localSum2
            Thread 3 → localSum3

            Best scenario of using multi-thread to offload long running local sum computation
            and merge them together at the end -> independent work
        */
        ThreadResolveRaceConditionByLocalSum(arr, 10);

        /*
            Thread 1 → localSum1
            Thread 2 → localSum2
            Thread 3 → localSum3

            The higher the partition size, the more threads created, hence
            overhead occurs which affect the performance -> context switching
        */
        ThreadResolveRaceConditionByLocalSum(arr, 1000);
    }

    public static void ThreadResolveRaceConditionByLocalSum(int[] arr, int partitionCount)
    {
        // Using Threading
        long n = partitionCount;
        Thread[] threads = new Thread[n];
        long[] localSums = new long[n];
        long portionSize = arr.Length / n;
        long sum1 = 0;
        var startTime1 = DateTime.Now;
        for (long i = 0; i < arr.Length; i += portionSize)
        {
            // capture the variable, since by default the closure will 
            // capture the variable by reference, meaning only 1 i is being change
            // so create b to capture the value of that i
            long b = i;

            threads[b / portionSize] = new Thread(() =>
            {
                long localSum = 0;
                for (long a = b; a < b + portionSize; a++)
                {
                    localSum += arr[a];
                }
                localSums[b / portionSize] = localSum;
            });

            threads[b / portionSize].Start();
        }

        // Since the main thread could run first or last
        // make Main thread to wail for all threads to finish and run the
        // below Console.WriteLine, or else main thread will execute the WriteLine
        // first before other thread finish calculation

        foreach (var t in threads)
        {
            t.Join();
        }

        foreach (var localSum in localSums)
        {
            sum1 += localSum;
        }

        var endTime1 = DateTime.Now;
        var duration1 = endTime1 - startTime1;
        Console.WriteLine($"Sum: {sum1} | Time: {duration1.Milliseconds} ms");
    }


    public static void Synchronous(int[] arr)
    {
        // Synchronous
        long sum = 0;
        var startTime = DateTime.Now;
        foreach (var num in arr)
        {
            sum += num;
        }
        var endTime = DateTime.Now;

        var duration = endTime - startTime;
        Console.WriteLine($"Sum: {sum} | Time: {duration.Milliseconds} ms");
    }

    public static void ThreadWithRaceCondition(int[] arr, int partitionCount)
    {
        // Using Threading
        long n = partitionCount;
        Thread[] threads = new Thread[n];
        long portionSize = arr.Length / n;
        long sum1 = 0;
        var startTime1 = DateTime.Now;
        for (long i = 0; i < arr.Length; i += portionSize)
        {
            // capture the variable, since by default the closure will 
            // capture the variable by reference, meaning only 1 i is being change
            // so create b to capture the value of that i
            long b = i;
            threads[b / portionSize] = new Thread(() =>
            {
                for (long a = b; a < b + portionSize; a++)
                {
                    sum1 += arr[a];
                }
            });

            threads[b / portionSize].Start();
        }

        // Since the main thread could run first or last
        // make Main thread to wail for all threads to finish and run the
        // below Console.WriteLine, or else main thread will execute the WriteLine
        // first before other thread finish calculation

        foreach (var t in threads)
        {
            t.Join();
        }

        var endTime1 = DateTime.Now;
        var duration1 = endTime1 - startTime1;
        Console.WriteLine($"Sum: {sum1} | Time: {duration1.Milliseconds} ms");
    }
}