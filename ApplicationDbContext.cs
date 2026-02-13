using MasPelículasAPI.Entidades;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics; // Necesario para ignorar la advertencia
using System.Security.Claims;

namespace MasPelículasAPI
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
        {
        }

        // Tus DbSets
        public DbSet<Genero> Generos { get; set; }
        public DbSet<Actor> Actores { get; set; }
        public DbSet<Pelicula> Peliculas { get; set; }
        public DbSet<PeliculasActores> PeliculasActores { get; set; }
        public DbSet<PeliculasGeneros> PeliculasGeneros { get; set; }
        public DbSet<SalaDeCine> SalasDeCine { get; set; }
        public DbSet<PeliculasSalasDeCine> PeliculasSalasDeCine { get; set; }

        public DbSet<Review> Reviews { get; set; }  

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning));
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<PeliculasActores>().HasKey(x => new { x.ActorId, x.PeliculaId });
            modelBuilder.Entity<PeliculasGeneros>().HasKey(x => new { x.GeneroId, x.PeliculaId });
            modelBuilder.Entity<PeliculasSalasDeCine>().HasKey(x => new { x.SalaDeCineId, x.PeliculaId });


            var usuarioAdminId = "5673b680-9de9-43c3-8889-130d24b614e5";
            var passwordHasher = new PasswordHasher<IdentityUser>();
            var username = "admin@maspeliculas.com";

            var usuarioAdmin = new IdentityUser()
            {
                Id = usuarioAdminId,
                UserName = username,
                NormalizedUserName = username.ToUpper(),
                Email = username,
                NormalizedEmail = username.ToUpper(),
                EmailConfirmed = true,
                PasswordHash = passwordHasher.HashPassword(null, "Aa123456!"),
                SecurityStamp = "9842886c-4824-424d-a2f0-79809148d479", // Fijo
                ConcurrencyStamp = "8524a86c-4824-424d-a2f0-79809148d479" // Fijo (Esto faltaba antes)
            };

            // 1. Seeding del Usuario
            modelBuilder.Entity<IdentityUser>().HasData(usuarioAdmin);

            // 2. Seeding del Claim (Rol Admin)
            modelBuilder.Entity<IdentityUserClaim<string>>().HasData(new IdentityUserClaim<string>()
            {
                Id = 1,
                ClaimType = ClaimTypes.Role,
                UserId = usuarioAdminId,
                ClaimValue = "Admin"
            });
        }
    }
}