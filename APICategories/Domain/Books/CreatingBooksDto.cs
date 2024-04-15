using System.Collections.Generic;

namespace APICategories.Domain.Books
{
    public class CreatingBooksDto
    {
        public string Id { get; set; }

        public List<string> Categories { get; set; }
        
        public CreatingBooksDto(string id, List<string> categories)
        {
            this.Id = id;
            this.Categories = categories;
        }
    }
}