using System;
using System.Linq;
using APIExemplar.Domain.Exemplars;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APIExemplar.Infraestructure;

public class InMemoryData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new APIExemplarDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<APIExemplarDbContext>>()))
        {
            if (context.Exemplars.Any())
            {
                return;   // Data was already seeded
            }

            context.Exemplars.AddRange(
                //new Exemplar("00000000000000000000000000000000", new BookId("978-0321349606"), new ExemplarState(2), new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"), new DateOnly(2016,1,5)),
                new Exemplar("11111111111111111111111111111111", new BookId("978-1491900864"), new ExemplarState(3), new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2017,1,5)),
                new Exemplar("22222222222222222222222222222222", new BookId("978-1617292545"), new ExemplarState(1), new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2018,1,5)),
                new Exemplar("55555555555555555555555555555555", new BookId("978-1617292545"), new ExemplarState(2), new ClientId("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb"), new DateOnly(2020,1,6)),
                new Exemplar("33333333333333333333333333333333", new BookId("978-0321356680"), new ExemplarState(4), new ClientId("cccccccccccccccccccccccccccccccc"), new DateOnly(2019,1,5)),
                new Exemplar("44444444444444444444444444444444", new BookId("978-1491900864"), new ExemplarState(0), new ClientId("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa"), new DateOnly(2010,1,5))
                );

            context.SaveChanges();
        }
    }
}