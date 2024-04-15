using APIAuthors.Controllers;
using APIAuthors.Domain.Authors;
using APIAuthors.Domain.Books;
using APIAuthors.Domain.Shared;
using APIAuthors.Infraestructure;
using APIAuthors.Infraestructure.Authors;
using APIAuthors.Infraestructure.Books;
using APIAuthors.Infraestructure.Shared;
using APIAuthors.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace APIAuthors
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
            services.AddDbContext<ApiAuthorsDbContext>(opt =>
                opt.UseInMemoryDatabase("APIAuthorsDB")
                .ReplaceService<IValueConverterSelector, StronglyEntityIdValueConverterSelector>());
                
            /*services.AddDbContext<ApiAuthorsDbContext>(options =>
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
                endpoints.MapGrpcService<BooksGrpcController>();
                endpoints.MapGrpcService<AuthorsGrpcController>();
            });
        }

        public void ConfigureMyServices(IServiceCollection services)
        {
            services.AddTransient<IUnitOfWork,UnitOfWork>();
            
            services.AddTransient<IBooksRepository, BooksRepository>();
            services.AddTransient<IAuthorsRepository, AuthorRepository>();
            
            services.AddTransient<IBooksService, BooksService>();
            services.AddTransient<IAuthorsService, AuthorsService>();
            
            services.AddTransient<BooksService>();
            services.AddTransient<AuthorsService>();
        }
        
    }
}
