// /*
//     Mutex mechanism is just like Monitor and Lock, but it's used for Process level synchronization, which means it can be used to synchronize access to resources across different processes, not just threads within the same process.
// */

// int count = 0;
// string filePath = "countMutex.txt";

// // Create a file if not exist
// if (!File.Exists(filePath))
// {
//     File.WriteAllText(filePath, "0");
// }

// void IncreaseTo100()
// {
//     for (int i = 0; i < 100; i++)
//     {
//         Thread.Sleep(100);
//         int currentCount = ReadFile(filePath);
//         count = currentCount + 1;
//         WriteFile(filePath);
//     }

//     Console.WriteLine($"Final count: {count}");
// }

// // If we open 4 process, the count value will not be 400, since multiple processses try to access the countMutext.txt file
// IncreaseTo100();

// // Stop to see the process finished
// Console.ReadLine();

// // Read Write and attach to count
// int ReadFile(string filePath)
// {
//     using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
//     using StreamReader sr = new StreamReader(fs);
//     string content = sr.ReadToEnd();
//     count = int.TryParse(content, out int result) ? result : 0;
//     return count;
// }

// // Write into the file 
// void WriteFile(string filePath)
// {
//     using FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
//     using StreamWriter sw = new StreamWriter(fs);
//     sw.Write(count);
// }

// =================================
// === Using Murex to synchronize access to the file across processes
// =================================

/*
    Mutex mechanism is just like Monitor and Lock, but it's used for Process level synchronization, which means it can be used to synchronize access to resources across different processes, not just threads within the same process.
*/

int count = 0;
string filePath = "countMutex.txt";

// Create a file if not exist
if (!File.Exists(filePath))
{
    File.WriteAllText(filePath, "0");
}

void IncreaseTo100()
{
    // "global\\countMutex" is the name of the mutex, it's used to identify the mutex across different processes
    // MUST use the same name, as the same lock occurring for the system-wide
    using (Mutex globalLock = new Mutex(false, $"Global\\countMutex"))
    {
        try
        {
            globalLock.WaitOne();

            for (int i = 0; i < 100; i++)
            {
                Thread.Sleep(100);
                int currentCount = ReadFile(filePath);
                count = currentCount + 1;
                WriteFile(filePath);
            }

            Console.WriteLine($"Final count: {count}");
        }
        finally
        {
            // alwasy release the lock in anycase after finishing the processing, if not released, other process will not be able to access the file
            globalLock.ReleaseMutex();
        }
    }
}

// If we open 4 process, the count value will not be 400, since multiple processses try to access the countMutext.txt file
IncreaseTo100();

// Stop to see the process finished
Console.ReadLine();

// Read Write and attach to count
int ReadFile(string filePath)
{
    using FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
    using StreamReader sr = new StreamReader(fs);
    string content = sr.ReadToEnd();
    count = int.TryParse(content, out int result) ? result : 0;
    return count;
}

// Write into the file 
void WriteFile(string filePath)
{
    using FileStream fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
    using StreamWriter sw = new StreamWriter(fs);
    sw.Write(count);
}