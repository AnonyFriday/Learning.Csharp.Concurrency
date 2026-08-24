using System.Diagnostics;

Console.WriteLine("\n--- Test .Result, .Wait, .WaitAll ---\n");
Console.WriteLine($"Main Thread Id Before: {Thread.CurrentThread.ManagedThreadId}");
var sw = Stopwatch.StartNew();

using var client = new HttpClient();
var task = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon");

// If do this -> block the calling thread, which is the main thread, like thread.join()
// - In this case, it's a main thread
string result = task.Result;
Console.WriteLine($"Result: {result.Length}, with time: {sw.ElapsedMilliseconds}");
Console.WriteLine($"Main Thread Id After: {Thread.CurrentThread.ManagedThreadId}");


// === Start with ContinueWith to see it does not block the calling thread ===
Console.WriteLine("\n--- Test ContinueWith (non-block) ---\n");
sw.Restart();

using var client1 = new HttpClient();
var task1 = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon");
task1.ContinueWith((t) =>
{
    for (int i = 0; i < 1_000_000_000; i++) ;
    Console.WriteLine($"Result: {result.Length}, with time: {sw.ElapsedMilliseconds}");
});

Console.WriteLine($"Main Thread Id After: {Thread.CurrentThread.ManagedThreadId}");


// === Start with await to see it does not block the calling thread ===
// - after await, the continuation may result on a differnt worke thread, not the same previous thread
// - does not blocking the calling thread, just release it back to the threadpool.
// - suspend method execution
Console.WriteLine("\n--- Test await (non-block) ---\n");
sw.Restart();

using var client2 = new HttpClient();
Console.WriteLine($"Before await - Thread Id: {Thread.CurrentThread.ManagedThreadId}");
Console.WriteLine($"Hello Word from thread Id: {Thread.CurrentThread.ManagedThreadId}");

var task2 = client2.GetStringAsync("https://pokeapi.co/api/v2/pokemon");
Console.WriteLine($"Right after calling GetStringAsync (not awaited yet) - elapsed: {sw.ElapsedMilliseconds}ms, Thread Id: {Thread.CurrentThread.ManagedThreadId}");

string result2 = await task2;
Console.WriteLine($"After await - Thread Id: {Thread.CurrentThread.ManagedThreadId}, elapsed: {sw.ElapsedMilliseconds}ms, Result length: {result2.Length}");

Console.WriteLine("\n--- Test WhenAll + ContinueWith chaining ---\n");
sw.Restart();

Console.WriteLine($"Before await - Thread Id: {Thread.CurrentThread.ManagedThreadId}");
var taskA = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon/1");
var taskB = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon/2");
var taskC = client.GetStringAsync("https://pokeapi.co/api/v2/pokemon/3");

Task.WhenAll(taskA, taskB, taskC)
    .ContinueWith(t =>
    {
        // t.Result = string[] (all 3 result), since taskA/B/C all Task<string>
        string[] results = t.Result;
        int totalLength = results.Sum(r => r.Length);
        Console.WriteLine($"[Step 1] All 3 done, total length: {totalLength}, elapsed: {sw.ElapsedMilliseconds}ms, Thread: {Thread.CurrentThread.ManagedThreadId}");
        return totalLength;
    })
    .ContinueWith(t =>
    {
        Console.WriteLine($"[Step 2] Final processed value: {t.Result * 2}, elapsed: {sw.ElapsedMilliseconds}ms, Thread: {Thread.CurrentThread.ManagedThreadId}");
    });
Console.WriteLine($"After await - Thread Id: {Thread.CurrentThread.ManagedThreadId}, elapsed: {sw.ElapsedMilliseconds}ms");

Console.WriteLine("\n--- Test WhenAny + ContinueWith chaining ---\n");
sw.Restart();

var taskX = client2.GetStringAsync("https://pokeapi.co/api/v2/pokemon/4");
var taskY = client2.GetStringAsync("https://pokeapi.co/api/v2/pokemon/5");

await Task.WhenAny(taskX, taskY)
    .ContinueWith(t =>
    {
        // t.Result = Task<string> (whichever finished first)
        Task<string> firstDone = t.Result;
        Console.WriteLine($"[Step 1] First finished, length: {firstDone.Result.Length}, elapsed: {sw.ElapsedMilliseconds}ms, Thread: {Thread.CurrentThread.ManagedThreadId}");
        return firstDone.Result.Length;
    })
    .ContinueWith(t =>
    {
        Console.WriteLine($"[Step 2] Doubled: {t.Result * 2}, elapsed: {sw.ElapsedMilliseconds}ms, Thread: {Thread.CurrentThread.ManagedThreadId}");
    });
Console.WriteLine($"After await - Thread Id: {Thread.CurrentThread.ManagedThreadId}, elapsed: {sw.ElapsedMilliseconds}ms");
