using System;
using System.Linq;
using APIClients.Domain.Clients;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace APIClients.Infraestructure;

public class InMemoryData
{
    public static void Initialize(IServiceProvider serviceProvider)
    {
        using (var context = new APIClientsDbContext(
            serviceProvider.GetRequiredService<DbContextOptions<APIClientsDbContext>>()))
        {
            if (context.Clients.Any())
            {
                return;   // Data was already seeded
            }

            context.Clients.AddRange(
                new Client("bbbbbbbbbbbbbbbbbbbbbbbbbbbbbbbb", "Juan Pablo Montoya Roldán", new Address("Autopista Norte, 54", "Bogotá", "6546-444","CO")),
                new Client("aaaaaaaaaaaaaaaaaaaaaaaaaaaaaaaa", "Sebastian Vettel", new Address("Wald-Michelbacher Straße, 66", "Heppenheim", "5553-351","DE")),
                new Client("dddddddddddddddddddddddddddddddd", "Zé Martin", new Address("Praceta Manuel Gonçalves Ramos, 25", "Maia", "4470-332","PT")),
                new Client("cccccccccccccccccccccccccccccccc", "Daniel Joseph Ricciardo", new Address("Cliff Street, 77", "Perth", "4201-898","AU"))
                );

            context.SaveChanges();
        }
    }
}