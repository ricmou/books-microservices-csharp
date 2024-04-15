using System.Collections.Generic;
using APIAuthors.Domain.Authors;

namespace APIAuthors.Domain.Books
{
    public class BooksDto
    {
        public string Id { get; set; }
        
        public List<AuthorDto> Authors { get; set; }

        public BooksDto(string id, List<AuthorDto> authors)
        {
            this.Id = id;
            this.Authors = authors;
        }

    }
}