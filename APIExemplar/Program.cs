using APIExemplar.Infraestructure;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;

namespace APIExemplar
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();
            
            //Injecting memory info here
            using (var scope = host.Services.CreateScope())
            {
                //Get the instance of APIExemplarDbContext in our services layer
                var services = scope.ServiceProvider;
                var context = services.GetRequiredService<APIExemplarDbContext>();

                //Call the InMemoryData to create sample data
                InMemoryData.Initialize(services);
            }
            
            host.Run();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
