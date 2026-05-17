// /*
//     Simulation of more people booking a ticket for a flight 
//     Share resource here is the number of ticket left within the database
// */

// using System.Collections;
// using System.Collections.Concurrent;

// int currentAvailableTickets = 10;
// Queue<string> bookingQueue = new Queue<string>();

// new Thread(SimulateBookingRequests)
// {
//     IsBackground = false
// }.Start();

// new Thread(MonitorBookingRequests)
// {
//     IsBackground = true
// }.Start();

// // Monitor method reading from the request queue
// void MonitorBookingRequests()
// {
//     // server only support 20 work thread to process the booki requests
//     for (int i = 0; i < 5; i++)
//     {
//         new Thread(() =>
//         {
//             while (true)
//             {
//                 var request = bookingQueue.TryDequeue(out string? bookingRequest);
//                 if (!request || bookingRequest is null)
//                 {
//                     continue;
//                 }

//                 switch (bookingRequest)
//                 {
//                     case "b" when currentAvailableTickets > 0:
//                         // at the time 1 thread read the currentAvailableTickets, it sleep with the old copy of the value
//                         // other thread is gonna increment or decrement based on the request it tooks, which affect to the final value of currentAvailableTickets
//                         Thread.Sleep(1000);
//                         currentAvailableTickets--;
//                         Console.WriteLine($"Booking successful for {bookingRequest}. Tickets left: {currentAvailableTickets}");
//                         break;
//                     case "c" when currentAvailableTickets < 10:
//                         Thread.Sleep(1000);
//                         currentAvailableTickets++;
//                         Console.WriteLine($"Ticket returned. Ticket left: {currentAvailableTickets}");
//                         break;
//                     default:
//                         break;
//                 }
//             }
//         })
//         { IsBackground = true }.Start();
//     }
// }

// // A simulation of a thread recieving multiple requests of booking
// // - assuming there are mroe than 20 requests, the result of the booking will be wrong due to race condition
// // on the same Queue and also on the same currentAvailableTickets variable

// void SimulateBookingRequests()
// {
//     string request = Console.ReadLine() ?? string.Empty;
//     while (request != "exit")
//     {
//         bookingQueue.Enqueue(request);
//         request = Console.ReadLine() ?? string.Empty;
//     }
// }

// /*
//     Simulation of more people booking a ticket for a flight 
//     Share resource here is the number of ticket left within the database
// */

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

                // lock ing the critical section to make sure only 1 thread can read and write the currentAvailableTickets at the time 
                lock (currentAvailableTicketsLock)
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

