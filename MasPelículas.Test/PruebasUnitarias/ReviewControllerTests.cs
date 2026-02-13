using AutoMapper;
using MasPelículasAPI;
using MasPelículasAPI.Controllers;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MasPelículas.Tests.PruebasUnitarias
{
    [TestClass]
    public class ReviewControllerTests
    {
        private ApplicationDbContext ConstruirContext(string dbName)
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: dbName)
                .Options;

            return new ApplicationDbContext(options);
        }

        private IMapper ConfigurarMapper()
        {
            var config = new MapperConfiguration(cfg =>
            {
                cfg.CreateMap<ReviewCreacionDTO, Review>();
                cfg.CreateMap<Review, ReviewDTO>();
            });

            return config.CreateMapper();
        }

        private ReviewController ConstruirController(ApplicationDbContext context)
        {
            var mapper = ConfigurarMapper();
            var controller = new ReviewController(context, mapper);

            var user = new ClaimsPrincipal(new ClaimsIdentity(new[]
            {
                new Claim(ClaimTypes.NameIdentifier, "user1"),
                new Claim(ClaimTypes.Name, "Chiki")
            }, "mock"));

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext { User = user }
            };

            return controller;
        }

        [TestMethod]
        public async Task GetPorId_NoExiste_RetornaNotFound()
        {
            var context = ConstruirContext(nameof(GetPorId_NoExiste_RetornaNotFound));
            var controller = ConstruirController(context);

            var respuesta = await controller.GetPorId(1, 1);

            Assert.IsInstanceOfType(respuesta.Result, typeof(NotFoundResult));
        }

        [TestMethod]
        public async Task Post_PeliculaNoExiste_RetornaNotFound()
        {
            var context = ConstruirContext(nameof(Post_PeliculaNoExiste_RetornaNotFound));
            var controller = ConstruirController(context);

            var respuesta = await controller.Post(1, new ReviewCreacionDTO { Puntuacion = 5, Comentario = "Excelente" });

            Assert.IsInstanceOfType(respuesta.Result, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task Post_Exitoso_CreaReview()
        {
            var context = ConstruirContext(nameof(Post_Exitoso_CreaReview));
            context.Peliculas.Add(new Pelicula { Id = 1, Titulo = "Test" });
            await context.SaveChangesAsync();

            var controller = ConstruirController(context);

            var respuesta = await controller.Post(1, new ReviewCreacionDTO
            {
                Puntuacion = 5,
                Comentario = "Excelente"
            });

            Assert.IsInstanceOfType(respuesta.Result, typeof(CreatedAtRouteResult));

            var reviewEnBD = await context.Reviews.FirstOrDefaultAsync();
            Assert.IsNotNull(reviewEnBD);
            Assert.AreEqual("user1", reviewEnBD.UsuarioId);
        }

        [TestMethod]
        public async Task Put_NoEsElMismoUsuario_RetornaForbid()
        {
            var context = ConstruirContext(nameof(Put_NoEsElMismoUsuario_RetornaForbid));

            context.Peliculas.Add(new Pelicula { Id = 1, Titulo = "Test" });
            context.Reviews.Add(new Review
            {
                Id = 1,
                PeliculaId = 1,
                UsuarioId = "otroUser",
                Comentario = "Viejo",
                Puntuacion = 3
            });

            await context.SaveChangesAsync();

            var controller = ConstruirController(context);

            var respuesta = await controller.Put(1, 1, new ReviewCreacionDTO
            {
                Puntuacion = 4,
                Comentario = "Actualizado"
            });

            Assert.IsInstanceOfType(respuesta, typeof(ForbidResult));
        }

        [TestMethod]
        public async Task Delete_NoEsElMismoUsuario_RetornaForbid()
        {
            var context = ConstruirContext(nameof(Delete_NoEsElMismoUsuario_RetornaForbid));

            context.Peliculas.Add(new Pelicula { Id = 1, Titulo = "Test" });
            context.Reviews.Add(new Review
            {
                Id = 1,
                PeliculaId = 1,
                UsuarioId = "otroUser",
                Comentario = "Viejo",
                Puntuacion = 3
            });

            await context.SaveChangesAsync();

            var controller = ConstruirController(context);

            var respuesta = await controller.Delete(1, 1);

            Assert.IsInstanceOfType(respuesta, typeof(ForbidResult));
        }
    }
}
