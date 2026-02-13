using AutoMapper;
using MasPelículas.Tests;
using MasPelículasAPI;
using MasPelículasAPI.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
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

            return new ApplicationDbContext(options);
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

        protected WebApplicationFactory<Startup> ConstruirWebApplicationFactory(string nombreDB)
        {
            return new WebApplicationFactory<Startup>()
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        // 1. LIMPIEZA PROFUNDA DE EF CORE
                        // Eliminamos las opciones, el contexto y CUALQUIER servicio interno de EF
                        var efServices = services.Where(d =>
                            d.ServiceType.FullName.Contains("EntityFrameworkCore") ||
                            d.ServiceType == typeof(ApplicationDbContext) ||
                            d.ServiceType == typeof(DbContextOptions<ApplicationDbContext>)).ToList();

                        foreach (var service in efServices)
                        {
                            services.Remove(service);
                        }

                        // 2. REGISTRAR IN-MEMORY DESDE CERO
                        services.AddDbContext<ApplicationDbContext>(options =>
                        {
                            options.UseInMemoryDatabase(nombreDB);
                        });

                        // 3. LIMPIEZA Y REEMPLAZO DE AUTOMAPPER
                        services.RemoveAll(typeof(IMapper));
                        services.AddSingleton(ConfigurarAutoMapper());

                        // 4. SOBRESCRIBIR CONTROLADORES Y SEGURIDAD
                        services.AddControllers(options =>
                        {
                            options.Filters.Add(new UsuarioFalsoFiltro());
                        }).AddNewtonsoftJson();

                        services.AddSingleton<IAuthorizationHandler, AllowAnonymousHandler>();
                    });
                });
        }
        protected void ConstruirControllerUsuario(ControllerBase controller, string idUsuario)
        {
            var user = new ClaimsPrincipal(new ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.NameIdentifier, idUsuario),
                new Claim(ClaimTypes.Email, "test@test.com")
            }, "TestAuthentication"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext() { User = user }
            };
        }
    }
}