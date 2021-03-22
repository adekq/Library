using Apache.Ignite.Core;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Log;
using Apache.Ignite.Core.Transactions;
using Microsoft.Extensions.Logging;
using RESTLibrary.Models;
using RESTLibrary.Persisters.Caches;

namespace RESTLibrary.Persisters
{
    public class IgnitePersister : IUserServicePersister, IBookServicePersister, IReservationServicePersister
    {
        private readonly ILogger<IgnitePersister> logger;
        private readonly IIgniteClient igniteClient;
        private readonly IIgniteUserCache userCache;
        private readonly IIgniteBookCache bookCache;
        private readonly IIgniteBookReservationQueueCache reservationCache;

        public IgnitePersister(ILogger<IgnitePersister> logger, AdminLibrarian adminLibrarian, IgniteClientConfiguration configuration)
        {
            this.logger = logger;

            igniteClient = Ignition.StartClient(new Apache.Ignite.Core.Client.IgniteClientConfiguration
            {
                Endpoints = new[] { configuration.Ip + ":" + configuration.Port },                
                EnablePartitionAwareness = true,                
                Logger = new ConsoleLogger { MinLevel = Apache.Ignite.Core.Log.LogLevel.Trace }
            });

            userCache = new IgniteUserCache(logger, igniteClient, adminLibrarian, configuration);
            bookCache = new IgniteBookCache(logger, igniteClient, configuration);
            reservationCache = new IgniteBookReservationQueueCache(logger, igniteClient, configuration);

            logger.LogInformation("Initialized caches");
        }

        bool IUserServicePersister.StoreUser(User user) => userCache.StoreUser(user);

        bool IUserServicePersister.UpdateUser(User user) => userCache.UpdateUser(user);

        User IUserServicePersister.ReadUser(string email) => userCache.ReadUser(email);

        bool IUserServicePersister.DeleteUser(string email) => userCache.DeleteUser(email);

        /// <summary>
        /// Storring books in cache. 
        /// </summary>
        /// <param name="book">Book to be stored persistently. Book Id will be updated to new value.</param>
        bool IBookServicePersister.StoreBook(ref Book book) => bookCache.StoreBook(ref book);

        bool IBookServicePersister.UpdateBook(Book book) => bookCache.UpdateBook(book);

        Book IBookServicePersister.ReadBook(string id) => bookCache.ReadBook(id);

        bool IBookServicePersister.DeleteBook(string id) => bookCache.DeleteBook(id);

        bool IReservationServicePersister.StoreReservation(Reservation reservation)
        {
            using (var transaction =  igniteClient.GetTransactions().TxStart(TransactionConcurrency.Pessimistic, TransactionIsolation.RepeatableRead))
            {
                if (userCache.ContainsUser(reservation.UserEmail) &&
                    bookCache.ContainsBook(reservation.BookId))
                {
                    reservationCache.StoreReservation_TxReq(reservation);
                    transaction.Commit();

                    return true;
                }
            }

            return false;
        }

        BookQueue IReservationServicePersister.ReadBookQueue(string bookId)
        {
            throw new System.NotImplementedException();
        }
    }    
}