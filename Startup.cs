using AutoMapper;// Necesario para UseSqlServer
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace MasPelículasAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        // 1. El constructor solo debe recibir y asignar la configuración
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        // 2. Aquí es donde se procesa la lógica y se inyectan los servicios
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup).Assembly);
            services.AddControllers();
            services.AddEndpointsApiExplorer();

            var dbServer = Configuration["DB_SERVER"] ?? "localhost";
            var dbName = Configuration["DB_NAME"] ?? "MasPeliculasDB";
            var dbUser = Configuration["DB_USER"];
            var dbPass = Configuration["DB_PASS"];

            string connectionString;

            // Si NO hay usuario o password en el .env, asumimos Autenticación de Windows
            if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPass))
            {
                connectionString = $"Server={dbServer};Database={dbName};Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";
            }

            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {

            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}