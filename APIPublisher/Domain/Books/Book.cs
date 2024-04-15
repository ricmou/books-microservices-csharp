using APIPublisher.Domain.Publishers;
using APIPublisher.Domain.Shared;

namespace APIPublisher.Domain.Books
{
    public class Book : Entity<BookId>, IAggregateRoot
    {
        public Publisher Publisher { get;  private set; }

        private Book()
        {
        }

        public Book(string isbn, Publisher publisher)
        {
            this.Id = new BookId(isbn);

            this.Publisher = publisher ?? throw new BusinessRuleValidationException("Invalid Publisher.");
        }

        public void ChangePublisher(Publisher publisher)
        {
            this.Publisher = publisher ?? throw new BusinessRuleValidationException("Invalid Publisher.");
        }
        
    }
}