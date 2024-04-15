using System.Collections.Generic;
using APICategories.Domain.Categories;

namespace APICategories.Domain.Books
{
    public class BooksDto
    {
        public string Id { get; set; }
        
        public List<CategoryDto> Categories { get; set; }

        public BooksDto(string id, List<CategoryDto> categories)
        {
            this.Id = id;
            this.Categories = categories;
        }

    }
}