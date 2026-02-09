using Microsoft.EntityFrameworkCore; // Necesario para UseSqlServer
using Microsoft.OpenApi.Models;

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
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "MasPelículasAPI", Version = "v1" });
            });

            // --- LÓGICA DE CONEXIÓN (Híbrida: Windows Auth o SQL Auth) ---

            // Leemos las variables. Usamos "??" para valores por defecto si no existen en el .env
            var dbServer = Configuration["DB_SERVER"] ?? "localhost";
            var dbName = Configuration["DB_NAME"] ?? "MasPeliculasDB";
            var dbUser = Configuration["DB_USER"];
            var dbPass = Configuration["DB_PASS"];

            string connectionString;

            // Si NO hay usuario o password en el .env, asumimos Autenticación de Windows
            if (string.IsNullOrEmpty(dbUser) || string.IsNullOrEmpty(dbPass))
            {
                // Conexión segura con tus credenciales de Windows
                connectionString = $"Server={dbServer};Database={dbName};Trusted_Connection=True;TrustServerCertificate=True;";
            }
            else
            {
                // Conexión con usuario y contraseña (si los pones en el futuro)
                connectionString = $"Server={dbServer};Database={dbName};User Id={dbUser};Password={dbPass};TrustServerCertificate=True;";
            }

            // Inyectamos el DbContext con la cadena construida
            // NOTA: Asegúrate de que 'ApplicationDbContext' sea el nombre real de tu clase de contexto
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(connectionString));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            // Configuración del entorno de desarrollo
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
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