using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using Tradera.Models;
using Tradera.Services.Utils;

namespace Tradera.Services
{
    public class NotificationsService : INotificationsService
    {
        private readonly NotificationOptions _options;
        //private readonly KeyedSemaphoresCollection _semaphoresCollection = new();
        //private readonly Dictionary<ProcessorIdentifier, decimal> lastHighest = new();
        // test with this decentralized key 2aa3cfb9769761102e4419215505a77f12dc48fd75796645a691a1b3029301e0

        public NotificationsService(IOptions<NotificationOptions> options)
        {
            _options = options.Value;
        }

        public Task DataUpdated(IEnumerable<ExchangeTicker> updatedData)
        {
            var notifyWith = ShouldNotify(updatedData);
            if (notifyWith != null) Notify(notifyWith);
            return Task.CompletedTask;
        }

        public Task Clear(ProcessorIdentifier identifier)
        {
            //lastHighest.Remove(identifier);
            return Task.CompletedTask;
        }

        private ExchangeTicker ShouldNotify(IEnumerable<ExchangeTicker> updatedData)
        {
            var highestAmount = updatedData.OrderByDescending(u => u.Price).First();
            var lastTrade = updatedData.OrderByDescending(u => u.EventTime).First();
           // var semaphore = _semaphoresCollection.GetOrCreate(highestAmount.Identifier);
          //  semaphore.Wait();
           // if (lastHighest.TryGetValue(highestAmount.Identifier, out var prev))
           // {
                if (highestAmount.Price > lastTrade.Price * (1 + _options.Threshold / 100))
                {
                    //lastHighest[highestAmount.Identifier] = highestAmount.Price;
                   // semaphore.Release();
                    return highestAmount;
                }
           /* }
            else
            {
               // lastHighest.Add(highestAmount.Identifier, highestAmount.Price);
                semaphore.Release();
                return highestAmount;
            }

            semaphore.Release(); */
            return null;
        }

        private static void Notify(ExchangeTicker data)
        {
            Log.Information("Event triggered with data {price} at time {time}",
                data.Price.ToString(CultureInfo.InvariantCulture), data.EventTime);
        }
    }
}