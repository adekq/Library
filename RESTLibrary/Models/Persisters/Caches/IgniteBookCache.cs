using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Microsoft.Extensions.Logging;
using RESTLibrary.Models;
using System;

namespace RESTLibrary.Persisters.Caches
{
    public interface IIgniteBookCache
    {
        public bool StoreBook(ref Book book);
        public bool UpdateBook(Book book);
        public Book ReadBook(string id);
        public bool DeleteBook(string id);
        public bool ContainsBook(string id);
    }

    public class IgniteBookCache : IIgniteBookCache
    {
        private readonly ILogger logger;
        private readonly ICacheClient<BookKey, Book> bookCache;
        private readonly IIgniteClient igniteClient;

        public IgniteBookCache(ILogger logger, IIgniteClient igniteClient, IgniteClientConfiguration configuration)
        {
            this.logger = logger;
            this.igniteClient = igniteClient;

            bookCache = this.igniteClient.GetOrCreateCache<BookKey, Book>(new CacheClientConfiguration
            {
                Name = "Books",
                AtomicityMode = CacheAtomicityMode.Transactional,
                DataRegionName = configuration.DataRegion
            });
        }

        public bool DeleteBook(string id)
        {
            logger.LogInformation("Deleting book: {}", id);
            return bookCache.Remove(new BookKey { Id = id });
        }

        public Book ReadBook(string id)
        {
            logger.LogInformation("Reading book: {}", id);
            bookCache.TryGet(new BookKey { Id = id }, out Book foundBook);
            return foundBook;
        }

        /// <summary>
        /// Storring books in cache. Ignite not provide atomic increament easilly accesible so used just guid (I am aware that is considerably bigger than data it is attached to).
        /// Other approach for this could be: 
        /// - ids reservation per instance
        /// - id generator with based on long/double etc (to save space) with PutIfAbsent approach
        /// - ids generation based on instance suffix/prefix
        /// - auto increment workaround: http://apache-ignite-users.70518.x6.nabble.com/Atomic-Sequence-Auto-Increment-via-IgniteClient-Thin-Client-Ignite-2-8-1-td33005.html
        /// </summary>
        /// <param name="book">Book to be stored persistently. Book Id will be updated to new value.</param>
        /// <returns></returns>
        public bool StoreBook(ref Book book)
        {
            logger.LogInformation("Storring book: {}/{}/{}", book.Author, book.Title, book.PublicationYear);

            BookKey key;
            do
            {
                book.Id = Guid.NewGuid().ToString();
                key = new BookKey { Id = book.Id };

            } while (!bookCache.PutIfAbsent(key, book));

            return true;
        }

        public bool UpdateBook(Book book)
        {
            logger.LogInformation("Replacing book: {}", book.Id);
            return bookCache.Replace(new BookKey { Id = book.Id }, book);
        }

        public bool ContainsBook(string id)
        {
            return bookCache.ContainsKey(new BookKey { Id = id });
        }

        private class BookKey
        {
            public string Id { get; set; }
        }
    }
}