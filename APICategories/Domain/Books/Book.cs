using System;
using System.Collections.Generic;
using APICategories.Domain.Categories;
using APICategories.Domain.Shared;

namespace APICategories.Domain.Books
{
    public class Book : Entity<BookId>, IAggregateRoot
    {
        public List<Category> Categories { get;  private set; }

        private Book()
        {
        }

        public Book(string isbn)
        {
            this.Id = new BookId(isbn);
            this.Categories = new List<Category>();
        }

        public void AddCategory(Category cat)
        {
            if (cat == null)
                throw new BusinessRuleValidationException("Invalid Category.");
            Categories.Add(cat);
        }

        public void ClearCategories()
        {
            Categories.Clear();
        }

        public override string ToString()
        {
            return String.Format("{0}; {1}", Id.AsString(), Categories.ToArray().ToString());
        }

        public void ChangeCategories(List<Category> categories)
        {
            this.Categories = categories ?? throw new BusinessRuleValidationException("Invalid Category List.");
        }
    }
}