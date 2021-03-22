namespace RESTLibrary.Models
{
    public enum Role
    {
        Librarian,
        Reader,
        Undefined
    }

    public class User
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public Role Role { get; set; }        
    }

    public interface IUserService
    {
        public bool AddUser(User newUser);
        public bool ContainUser(string email, string password);
        public Role UserRole(string email);
    }

    public interface IUserServicePersister
    {
        public bool StoreUser(User user);
        public bool UpdateUser(User user);
        public User ReadUser(string email);
        public bool DeleteUser(string email);
    }

    public class UserService : IUserService
    {
        private readonly IUserServicePersister persister;

        public UserService(IUserServicePersister persister)
        {
            this.persister = persister;
        }

        public bool AddUser(User newUser)
        {
            return persister.StoreUser(newUser);
        }

        public bool ContainUser(string email, string password)
        {
            return persister.ReadUser(email)?.Password == password;
        }

        public Role UserRole(string email)
        {
            return persister.ReadUser(email)?.Role ?? Role.Undefined;
        }
    }
}