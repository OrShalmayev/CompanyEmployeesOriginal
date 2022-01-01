using Contracts;
using Entities;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceExtensions {
    // configure CORS in our application
    public static void ConfigureCors(this IServiceCollection services) {
        services.AddCors(options => {
            options.AddPolicy("CorsPolicy",
                builder => builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
            );
        });
    }

    // configure IIS in our application
    public static void ConfigureIISIntegration(this IServiceCollection services) {
        services.Configure<IISOptions>(options => { });
    }
    // configure logger service in our application
    public static void ConfigureLoggerService(this IServiceCollection services)
    {
        services.AddScoped<ILoggerManager, LoggerManager>();
    }

    // register the database
    public static void ConfigureSqlContext(this IServiceCollection services, IConfiguration configuration) =>
         services.AddDbContext<RepositoryContext>(opts =>
             opts.UseSqlServer(
                 configuration["ConnectionStrings:DefaultConnection"],
                 b => b.MigrationsAssembly("CompanyEmployeesOriginal")
             )
         );
    // register the repository manager
    public static void ConfigureRepositoryManager(this IServiceCollection services) =>
        services.AddScoped<IRepositoryManager, RepositoryManager>();

}