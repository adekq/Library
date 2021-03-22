namespace RESTLibrary.Models
{
    public interface IBookServicePersister
    {
        public bool StoreBook(ref Book book);
        public bool UpdateBook(Book book);
        public Book ReadBook(string id);
        public bool DeleteBook(string id);
    }

    public interface IBookService
    {
        public bool AddBook(ref Book book);
    }

    public class Book
    {
        public string Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public int PublicationYear { get; set; }
        public string Description { get; set; }
    }

    public class BookService : IBookService
    {
        private readonly IBookServicePersister persister;

        public BookService(IBookServicePersister persister)
        {
            this.persister = persister;
        }

        public bool AddBook(ref Book book)
        {
            return persister.StoreBook(ref book);
        }
    }
}