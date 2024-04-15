using System;
using System.Linq;
using APIPublisher.Domain.Books;
using APIPublisher.Domain.Publishers;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APIPublisher.Infraestructure;

public class InMemoryData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new APIPublisherDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<APIPublisherDbContext>>()))
        {
            if (context.Books.Any())
            {
                return;   // Data was already seeded
            }

            /*context.Books.AddRange(
                new Book("978-0321349606", "Java Concurrency in Practice", "Addison Wesley"),
                new Book("978-1491900864", "Java 8 Pocket Guide", "O'Reilly"),
                new Book("978-1617292545", "Spring Boot in Action", "Manning Publications"),
                new Book("978-0321356680", "Effective Java", "Addison Wesley")
                );*/

            var pub1 = new Publisher("AWE", "Addison Wesley", "US");
            var pub2 = new Publisher("ORE", "O'Reilly", "GB");
            var pub3 = new Publisher("MAN", "Manning Publications", "FR");
            
            context.Publishers.AddRange(
                pub1, pub2, pub3);
            
            context.Books.AddRange(
                new Book("978-0321349606", pub1),
                new Book("978-1491900864", pub2),
                new Book("978-1617292545", pub3),
                new Book("978-0321356680", pub1)
                );

            context.SaveChanges();
        }
    }
}