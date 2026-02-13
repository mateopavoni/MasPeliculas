using AutoMapper;
using MasPelículas.Tests;
using MasPelículasAPI;
using MasPelículasAPI.Helpers;
using Microsoft.AspNetCore.Authorization; // Necesario para IAuthorizationHandler
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Linq;
using System.Security.Claims;

namespace MasPeliculas.Tests
{
    public class BasePruebas
    {
        protected ApplicationDbContext ConstruirContext(string nombreDB)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(nombreDB)
                .Options;

            var dbContext = new ApplicationDbContext(options);
            return dbContext;
        }

        protected IMapper ConfigurarAutoMapper()
        {
            var config = new MapperConfiguration(options =>
            {
                var geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);
                options.AddProfile(new AutoMapperProfiles(geometryFactory));
            });

            return config.CreateMapper();
        }

        protected void ConstruirControllerUsuario(ControllerBase controller, string idUsuario)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, idUsuario),
                new Claim(ClaimTypes.Email, "ejemplo@prueba.com"),
                new Claim(ClaimTypes.Name, "usuario_prueba")
            }, "TestAuthentication"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }

        protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreDB)
        {
            return new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var descriptor = services.SingleOrDefault(
                            d => d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>));

                        if (descriptor != null)
                        {
                            services.Remove(descriptor);
                        }

                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(nombreDB);
                        });

                        services.AddControllers(options =>
                        {
                            options.Filters.Add(new UsuarioFalsoFiltro());
                        });

                        services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
                    });
                });
        }
    }
}