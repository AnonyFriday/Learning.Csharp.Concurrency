internal class Program
{
    private static void Main(string[] args)
    {
        int[] array = Enumerable.Range(1, 100_000).ToArray();

        // Synchronous
        int sum = 0;
        var startTime = DateTime.Now;
        foreach (var num in array)
        {
            sum += num;
        }
        var endTime = DateTime.Now;

        var duration = endTime - startTime;
        Console.WriteLine($"Sum: {sum} | Time: {duration.Milliseconds} ms");

        // Using Threading
        int n = 10;
        Thread[] threads = new Thread[n];
        int portionSize = array.Length / n;
        int sum1 = 0;
        var startTime1 = DateTime.Now;
        for (int i = 0; i < array.Length; i += portionSize)
        {
            // capture the variable, since by default the closure will 
            // capture the variable by reference, meaning only 1 i is being change
            // so create b to capture the value of that i
            int b = i;
            threads[b / portionSize] = new Thread(() =>
            {
                for (int a = b; a < b + portionSize; a++)
                {
                    sum1 += array[a];
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