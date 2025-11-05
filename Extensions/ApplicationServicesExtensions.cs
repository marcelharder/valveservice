using ValveService.implementations;
using ValveService.interfaces;

namespace ValveService.Extensions;

    public static class ApplicationServicesExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
        {

           
            services.Configure<CloudinarySettings>(config.GetSection("CloudinarySettings"));
            services.Configure<ComSettings>(config.GetSection("ComSettings"));

           /*  services.AddDbContext<ApplicationDbContext>(options => options
                       .UseMySql(config.GetConnectionString("SQLConnection"),
                           mysqlOptions =>
                               mysqlOptions.ServerVersion(new Pomelo.EntityFrameworkCore.MySql.Storage.ServerVersion(new Version(10, 4, 6), ServerType.MariaDb)).EnableRetryOnFailure()));
        
          */ 
      var serverVersion = new MariaDbServerVersion(new Version(8, 0, 34));
          var connectionString = config.GetConnectionString("SQLConnection");
          services.AddDbContext<ApplicationDbContext>(
            dbContextOptions => dbContextOptions
                .UseMySql(connectionString, serverVersion)
                // The following three options help with debugging, but should
                // be changed or removed for production.
                .LogTo(Console.WriteLine, LogLevel.Information)
                .EnableSensitiveDataLogging()
                .EnableDetailedErrors()
        );
          
          
          
          
          
            services.AddSingleton<DapperContext>();
            services.AddScoped<reportMapper>();
            services.AddScoped<IValveCode, ValveCode>();
            services.AddAutoMapper(typeof(AutoMapperProfiles).Assembly);
            return services;
        }
    }
