using Apache.Ignite.Core.Cache.Configuration;
using Apache.Ignite.Core.Client;
using Apache.Ignite.Core.Client.Cache;
using Microsoft.Extensions.Logging;
using RESTLibrary.Models;

namespace RESTLibrary.Persisters.Caches
{
    public interface IIgniteUserCache
    {
        public bool StoreUser(User user);
        public bool UpdateUser(User user);
        public User ReadUser(string email);
        public bool DeleteUser(string email);
        public bool ContainsUser(string email);
    }

    public class IgniteUserCache : IIgniteUserCache
    {
        private readonly ILogger logger;
        private readonly IIgniteClient igniteClient;
        private readonly ICacheClient<UserKey, User> userCache;

        public IgniteUserCache(ILogger logger, IIgniteClient igniteClient, AdminLibrarian adminLibrarian, IgniteClientConfiguration configuration)
        {
            this.igniteClient = igniteClient;
            this.logger = logger;

            userCache = this.igniteClient.GetOrCreateCache<UserKey, User>(new CacheClientConfiguration
            {
                Name = "Users",
                AtomicityMode = CacheAtomicityMode.Transactional,
                DataRegionName = configuration.DataRegion
            });

            StoreUser(adminLibrarian);
        }

        public bool DeleteUser(string email)
        {
            logger.LogInformation("Deleting user: {}", email);
            return userCache.Remove(new UserKey { Email = email });
        }

        public User ReadUser(string email)
        {
            logger.LogInformation("Reading user: {}", email);
            userCache.TryGet(new UserKey { Email = email }, out User foundUser);
            return foundUser;
        }

        public bool StoreUser(User user)
        {
            logger.LogInformation("Storring user: {}", user.Email);
            return userCache.PutIfAbsent(new UserKey { Email = user.Email }, user);
        }

        public bool UpdateUser(User user)
        {
            logger.LogInformation("Replacing user: {}", user.Email);
            return userCache.Replace(new UserKey { Email = user.Email }, user);
        }

        public bool ContainsUser(string email)
        {
            return userCache.ContainsKey(new UserKey { Email = email });
        }

        private class UserKey
        {
            public string Email { get; set; }
        }
    }
}