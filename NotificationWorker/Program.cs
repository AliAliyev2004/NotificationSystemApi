using System;
using System.Threading.Tasks;

namespace NotificationWorker
{
    class Program
    {
        static async Task Main()
        {
            Console.WriteLine("Worker basladı, mesajları dinleyir...");
            var worker = new NotificationWorker();
            await worker.RunAsync();
        }
    }
}
