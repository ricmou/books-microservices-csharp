using APIExemplar.Controllers;
using APIExemplar.Domain.Exemplars;
using APIExemplar.Domain.Shared;
using APIExemplar.Infraestructure;
using APIExemplar.Infraestructure.Exemplars;
using APIExemplar.Infraestructure.Shared;
using APIExemplar.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace APIExemplar
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<APIExemplarDbContext>(opt =>
                opt.UseInMemoryDatabase("APIExemplarDB")
                .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());
                
            /*services.AddDbContext<APIExemplarDbContext>(options =>
                options.UseSqlServer(Configuration["Data:ConnectionStrings:DefaultConnection"])
               .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());*/

            ConfigureMyServices(services);

            services.AddControllers().AddNewtonsoftJson();
            
            services.AddGrpc();
            
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(builder =>
                {
                    builder.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
                });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
                app.UseHsts();
            }

            //app.UseHttpsRedirection();

            app.UseRouting();
            
            app.UseCors(builder =>
            {
                builder.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader();
            });

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
                endpoints.MapGrpcService<ExemplarGrpcController>();
            });
        }

        public void ConfigureMyServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork,UnitOfWork>();

            services.AddTransient<IExemplarRepository,ExemplarRepository>();
            services.AddTransient<IExemplarService, ExemplarService>();
            services.AddTransient<ExemplarService>();
            
        }
        
    }
}
