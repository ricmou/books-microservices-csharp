using System;
using System.Linq;
using APICategories.Domain.Books;
using APICategories.Domain.Categories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APICategories.Infraestructure;

public class InMemoryData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new APICategoriesDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<APICategoriesDbContext>>()))
        {
            if (context.Books.Any())
            {
                return;   // Data was already seeded
            }
            
            if (context.Categories.Any())
            {
                return;   // Data was already seeded
            }

            var cat1 = new Category("SPR", "Spring");
            var cat2 = new Category("JAV", "Java");
            var cat3 = new Category("BEG", "Beginner");
            var cat4 = new Category("ADV", "Advanced");
            var cat5 = new Category("SCI", "Science Fiction");
            var cat6 = new Category("OPT", "Optimization");
            
            context.Categories.AddRange(cat1, cat2, cat3, cat4, cat5 ,cat6);

            var book = new Book("978-0321349606");
            book.AddCategory(cat2);
            context.Books.Add(book);
            
            book = new Book("978-1491900864");
            book.AddCategory(cat2);
            book.AddCategory(cat3);
            context.Books.Add(book);
            
            book = new Book("978-1617292545");
            book.AddCategory(cat1);
            book.AddCategory(cat2);
            book.AddCategory(cat4);
            context.Books.Add(book);
            
            book = new Book("978-0321356680");
            book.AddCategory(cat4);
            book.AddCategory(cat2);
            book.AddCategory(cat6);
            context.Books.Add(book);

            /*context.Books.AddRange(
                new Book("978-0321349606", "Java Concurrency in Practice", "Addison Wesley"),
                new Book("978-1491900864", "Java 8 Pocket Guide", "O'Reilly"),
                new Book("978-1617292545", "Spring Boot in Action", "Manning Publications"),
                new Book("978-0321356680", "Effective Java", "Addison Wesley")
                );*/

            context.SaveChanges();
        }
    }
}