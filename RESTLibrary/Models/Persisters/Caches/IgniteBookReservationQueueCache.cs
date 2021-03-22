using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Microsoft.Extensions.Logging;
using RESTLibrary.Models;
using System.Collections.Generic;

namespace RESTLibrary.Persisters.Caches
{
    public interface IIgniteBookReservationQueueCache
    {
        /// <summary>
        /// Store user book reservation in a book queue.
        /// This method need to be executed in transaction.
        /// </summary>
        /// <param name="reservation"></param>
        /// <returns></returns>
        public bool StoreReservation_TxReq(Reservation reservation);        
        public BookQueue ReadBookQueue(string bookId);
    }

    public class IgniteBookReservationQueueCache : IIgniteBookReservationQueueCache
    {
        private readonly ILogger logger;
        private readonly IIgniteClient igniteClient;
        private readonly ICacheClient<BookQueueKey, List<Reservation>> reservationCache;
        
        public IgniteBookReservationQueueCache(ILogger logger, IIgniteClient igniteClient, IgniteClientConfiguration configuration)
        {
            this.logger = logger;
            this.igniteClient = igniteClient;

            reservationCache = this.igniteClient.GetOrCreateCache<BookQueueKey, List<Reservation>>(new CacheClientConfiguration
            {
                Name = "Reservations",
                AtomicityMode = CacheAtomicityMode.Transactional,
                DataRegionName = configuration.DataRegion
            });
        }

        public bool StoreReservation_TxReq(Reservation reservation)
        {
            var bookKey = new BookQueueKey { BookId = reservation.BookId };

            if (reservationCache.TryGet(bookKey, out List<Reservation> reservations))
            {
                reservations.Add(reservation);
                reservationCache.Put(bookKey, reservations);
            }
            else
            {
                reservationCache.Put(bookKey, new List<Reservation> { reservation } );
            }

            return true;
        }

        public BookQueue ReadBookQueue(string bookId)
        {
            logger.LogInformation("Reading book queue: {}", bookId);
            if (reservationCache.TryGet(new BookQueueKey { BookId = bookId }, out List<Reservation> reservations))
            {
                return new BookQueue { BookId = bookId, Reservations = reservations };
            }

            return null;
        }

        private class BookQueueKey
        {
            public string BookId { get; set; }
        }
    }    
}