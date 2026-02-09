namespace MasPelículasAPI
{
    public class Startup
    {
        public IConfiguration Configuration { get; }

        // Constructor que recibe la configuración
        public Startup(IConfiguration configuration)
        {
            var dbPassword = Configuration["DB_PASS"];
            var jwtSecret = Configuration["JWT_SECRET"];

            var connectionString = $"Server={Configuration["DB_SERVER"]};Database={Configuration["DB_NAME"]};User Id={Configuration["DB_USER"]};Password={Configuration["DB_PASS"]};TrustServerCertificate=True;";
            Configuration = configuration;
        }

        // Método para agregar servicios al contenedor
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
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