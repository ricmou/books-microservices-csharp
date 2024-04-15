using APIPublisher.Domain.Publishers;

namespace APIPublisher.Domain.Books
{
    public class BooksDto
    {
        public string Id { get; set; }
        
        public PublisherDto Publisher { get; set; }

        public BooksDto(string id, PublisherDto publisher)
        {
            this.Id = id;
            this.Publisher = publisher;
        }

    }
}