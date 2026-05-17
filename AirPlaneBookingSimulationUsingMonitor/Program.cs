// =========================================
// === Solutions
// =========================================

using System.Collections;
using System.Collections.Concurrent;

int currentAvailableTickets = 10;
ConcurrentQueue<string> bookingQueue = new ConcurrentQueue<string>();

// Mutual exclusion via lock/monitor
Lock currentAvailableTicketsLock = new Lock();

new Thread(SimulateBookingRequests)
{
    IsBackground = false
}.Start();

new Thread(MonitorBookingRequests)
{
    IsBackground = true
}.Start();

// Monitor method reading from the request queue
void MonitorBookingRequests()
{
    // server only support 20 work thread to process the booki requests
    for (int i = 0; i < 5; i++)
    {
        new Thread(() =>
        {
            while (true)
            {
                var request = bookingQueue.TryDequeue(out string? bookingRequest);
                if (!request || bookingRequest is null)
                {
                    continue;
                }

                // For every 3s, the OS Scheduler + thread timing determine if which thread acquires first, and wake first
                // If a lock is not released yet, then that thread will not be able to enter the critical section 
                // and return the else statement
                bool lockAcquired = false;

                // Each thread has a local copy of  of the lockAcquired variable,
                // Monitor.TryEnter will set the lockAcquired to true if the thread successfully acquires the lock, otherwise it will remain false
                Monitor.TryEnter(currentAvailableTicketsLock, 3000, ref lockAcquired);
                if (!lockAcquired)
                {
                    Console.WriteLine("System is busy, skipping the booking request.");
                    continue;
                }

                // Simulation the processing is over 4s
                // - A is handling request in 4s
                // - WIthin 4s, other thread join the cirtical section will return the Else statement since A is current holding
                // a lock
                Thread.Sleep(4000);

                try
                {
                    switch (bookingRequest)
                    {
                        case "b" when currentAvailableTickets > 0:
                            Thread.Sleep(1000);
                            currentAvailableTickets--;
                            Console.WriteLine($"Booking successful for {bookingRequest}. Tickets left: {currentAvailableTickets}");
                            break;
                        case "c" when currentAvailableTickets < 10:
                            Thread.Sleep(1000);
                            currentAvailableTickets++;
                            Console.WriteLine($"Ticket returned. Ticket left: {currentAvailableTickets}");
                            break;
                        default:
                            break;
                    }
                }
                finally
                {
                    if (lockAcquired)
                    {
                        // Release the lock if any exception occurs, or the thread finished the request processing
                        Monitor.Exit(currentAvailableTicketsLock);
                    }
                }

            }
        })
        { IsBackground = true }.Start();
    }
}

// A simulation of a thread recieving multiple requests of booking
// - assuming there are mroe than 20 requests, the result of the booking will be wrong due to race condition
// on the same Queue and also on the same currentAvailableTickets variable

void SimulateBookingRequests()
{
    string request = Console.ReadLine() ?? string.Empty;
    while (request != "exit")
    {
        bookingQueue.Enqueue(request);
        request = Console.ReadLine() ?? string.Empty;
    }
}

