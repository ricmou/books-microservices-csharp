using System;
using System.Collections.Generic;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Shared;

namespace APIAuthors.Domain.Books
{
    public class Book : Entity<BookId>, IAggregateRoot
    {
        public List<Author> Authors { get;  private set; }

        private Book()
        {
        }

        public Book(string isbn)
        {
            this.Id = new BookId(isbn);
            this.Authors = new List<Author>();
        }

        public void AddAuthor(Author auth)
        {
            if (auth == null)
                throw new BusinessRuleValidationException("Invalid Author.");
            Authors.Add(auth);
        }

        public void ClearAuthors()
        {
            Authors.Clear();
        }

        public override string ToString()
        {
            return String.Format("{0}; {1}", Id.AsString(), Authors.ToArray().ToString());
        }
    }
}