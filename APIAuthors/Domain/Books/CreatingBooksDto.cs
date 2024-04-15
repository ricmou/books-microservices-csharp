using System.Collections.Generic;

namespace APIAuthors.Domain.Books
{
    public class CreatingBooksDto
    {
        public string Id { get; set; }

        public List<string> Authors { get; set; }
        
        public CreatingBooksDto(string id, List<string> authors)
        {
            this.Id = id;
            this.Authors = authors;
        }
    }
}