// Using the 100K item size to see the real speed
int[] array = Enumerable.Range(0, 999_999_999).ToArray();
Console.WriteLine($"Running on {array.Length} numbers");

#region Using Sync
int sumSync = 0;
var startTimeSync = DateTime.Now;

for (int i = 0; i < array.Length; i++)
{
    sumSync += array[i];
}

var endTimeSync = DateTime.Now;
var timeSpanSync = endTimeSync - startTimeSync;

Console.WriteLine($"The sum is {sumSync}");
Console.WriteLine($"The time is takes: {timeSpanSync.TotalMilliseconds}");
#endregion

#region Using Tasks for Divide and Conquer
var startTime = DateTime.Now;

int sum = 0;
int numOfTasks = 4;
int segmentLength = array.Length / numOfTasks;

int SumSegment(int start, int end)
{
    int sum = 0;
    for (int i = start; i < end; i++)
    {
        sum += array[i];
    }

    return sum;
}

Task<int>[] tasks = new Task<int>[numOfTasks];
for (int i = 0; i < numOfTasks; i++)
{
    int start = i * segmentLength;
    // since the segmentLength drop the remainder 1, last task should be an array.Length to cover the last odd element
    int end = (i == numOfTasks - 1) ? array.Length : start + segmentLength;
    tasks[i] = Task.Run(() => SumSegment(start, end));
}

Task.WaitAll(tasks);
for (int i = 0; i < numOfTasks; i++)
{
    sum += tasks[i].Result;
}

var endTime = DateTime.Now;
var timeSpan = endTime - startTime;

Console.WriteLine($"The sum is {sum}");
Console.WriteLine($"The time is takes: {timeSpan.TotalMilliseconds}");
#endregion