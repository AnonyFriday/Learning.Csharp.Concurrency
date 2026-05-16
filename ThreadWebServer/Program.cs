using System.Collections.Concurrent;

ConcurrentQueue<string> requestQueue = new ConcurrentQueue<string>();

Thread monitorThread = new(Monitor)
{
    IsBackground = true
};

monitorThread.Start();

void ReceiveRequest()
{
    Console.Write("Enter requests (type 'exit' to stop): ");
    string input = Console.ReadLine() ?? string.Empty;
    while (input != "exit")
    {
        Console.WriteLine($"Received request: {input}");
        requestQueue.Enqueue(input);
        input = Console.ReadLine() ?? string.Empty;
    }
}

ReceiveRequest();

// Monitor to handle threads from queue and process them one by one
void Monitor()
{
    while (true)
    {
        if (requestQueue.TryDequeue(out string? request))
        {
            Console.WriteLine($"Processing request: {request}");
            Thread.Sleep(1000); // Simulate processing time
        }
    }
}
