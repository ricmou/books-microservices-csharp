namespace APIPublisher.Domain.Books
{
    public class CreatingBooksDto
    {
        public string Id { get; set; }

        public string PublisherId { get; set; }
        
        public CreatingBooksDto(string id, string publisher)
        {
            this.Id = id;
            this.PublisherId = publisher;
        }
    }
}