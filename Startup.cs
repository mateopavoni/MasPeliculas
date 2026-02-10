using AutoMapper;
using MasPelículasAPI.Helpers; // Asegúrate de que este using exista
using MasPelículasAPI.Servicios;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite.Geometries;

namespace MasPelículasAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(AutoMapperProfiles));

            services.AddControllers().AddNewtonsoftJson();
            services.AddEndpointsApiExplorer();
            services.AddHttpContextAccessor();
            services.AddScoped<IAlmacenadorArchivos, AlmacenadorArchivosLocal>();

            var dbServer = Configuration["DB_SERVER"] ?? "localhost";
            var dbName = Configuration["DB_NAME"] ?? "MasPeliculasDB";
            var dbUser = Configuration["DB_USER"];
            var dbPass = Configuration["DB_PASS"];

            string connectionString;
            if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPass))
            {
                connectionString = $"Server={dbServer};Database={dbName};Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";
            }

            services.AddDbContext<ApplicationDbContext>(options =>
            {
                options.UseSqlServer(connectionString, sqlServerOptions =>
                {
                    sqlServerOptions.UseNetTopologySuite();
                });
            });

            services.AddSingleton(provider => new GeometryFactory(new PrecisionModel(), 4326));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) { }
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthorization();
            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}