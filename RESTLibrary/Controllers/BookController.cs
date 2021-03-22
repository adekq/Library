using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NJsonSchema.Annotations;
using NSwag.Annotations;
using RESTLibrary.Models;
using System.Text.Json.Serialization;

namespace RESTLibrary.Controllers
{
    public class BookController : ControllerBase
    {
        private readonly ILogger<BookController> logger;
        private readonly IBookService bookService;

        public BookController(ILogger<BookController> logger, IBookService bookService)
        {
            this.logger = logger;
            this.bookService = bookService;
        }

        [Authorize(Roles = nameof(Role.Librarian))]
        [HttpPost("add/book")]
        public ActionResult<AddBookResponse> AddBook([FromBody] AddBookRequest request)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }

            var newBook = request.Book;
            bookService.AddBook(ref newBook);

            return Ok(new AddBookResponse { BookId = newBook.Id });
        }
        
        public class AddBookRequest
        {
            private readonly Book book = new Book();
            
            [JsonSchemaIgnore]
            public Book Book 
            {
                get { return book; }
            }

            public string Title 
            {
                get { return Book.Title; }
                set { Book.Title = value; }
            }

            public string Author 
            {
                get { return Book.Author; }
                set { Book.Author = value; }
            }

            public int PublicationYear 
            {
                get { return Book.PublicationYear; }
                set { Book.PublicationYear = value; }
            }

            public string Description 
            {
                get { return Book.Description; }
                set { Book.Description = value; }
            }
        }

        public class AddBookResponse
        {
            public string BookId { get; set; }
        }        
    }
}