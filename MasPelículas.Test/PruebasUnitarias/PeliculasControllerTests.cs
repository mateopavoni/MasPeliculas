using MasPeliculas.Tests;
using MasPelículasAPI.Controllers;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using MasPelículasAPI.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasPelículas.Tests.PruebasUnitarias
{
    [TestClass]
    public class PeliculasControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerPeliculasIndex_ProximosEstrenosYEnCines()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();
            var mockLogger = new Mock<ILogger<PeliculasController>>();

            var hoy = DateTime.Today;

            context.Peliculas.Add(new Pelicula() { Titulo = "Futura", FechaEstreno = hoy.AddDays(1), EnCines = false });
            context.Peliculas.Add(new Pelicula() { Titulo = "En Cines", FechaEstreno = hoy.AddDays(-1), EnCines = true });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(context2, mapper, mockAlmacenador.Object, mockLogger.Object);

            var respuesta = await controller.Get();
            var resultado = respuesta.Value;

            Assert.AreEqual(1, resultado.FuturosEstrenos.Count);
            Assert.AreEqual(1, resultado.PeliculasEnCines.Count);
        }

        [TestMethod]
        public async Task FiltrarPorTitulo()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.Peliculas.Add(new Pelicula() { Titulo = "Spider-Man" });
            context.Peliculas.Add(new Pelicula() { Titulo = "Batman" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();
            var mockLogger = new Mock<ILogger<PeliculasController>>();
            var controller = new PeliculasController(context2, mapper, mockAlmacenador.Object, mockLogger.Object);

            controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };

            var filtro = new FiltroPeliculasDTO() { Titulo = "Spider" };
            var respuesta = await controller.Filtrar(filtro);
            var peliculas = respuesta.Value;

            Assert.AreEqual(1, peliculas.Count);
            Assert.AreEqual("Spider-Man", peliculas[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarPorGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var genero = new Genero() { Nombre = "Terror" };
            context.Add(genero);
            await context.SaveChangesAsync();

            var pelicula = new Pelicula() { Titulo = "Peli Terror" };
            context.Add(pelicula);
            await context.SaveChangesAsync();

            context.Add(new PeliculasGeneros() { GeneroId = genero.Id, PeliculaId = pelicula.Id });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(context2, mapper, new Mock<IAlmacenadorArchivos>().Object, new Mock<ILogger<PeliculasController>>().Object);
            controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };

            var filtro = new FiltroPeliculasDTO() { GeneroId = genero.Id };
            var respuesta = await controller.Filtrar(filtro);

            Assert.AreEqual(1, respuesta.Value.Count);
        }

        [TestMethod]
        public async Task FiltrarEnCines()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);

            context.Peliculas.Add(new Pelicula() { Titulo = "Peli 1", EnCines = true });
            context.Peliculas.Add(new Pelicula() { Titulo = "Peli 2", EnCines = false });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(context2, ConfigurarAutoMapper(),
                new Mock<IAlmacenadorArchivos>().Object, new Mock<ILogger<PeliculasController>>().Object);
            controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };

            var filtro = new FiltroPeliculasDTO() { EnCines = true };
            var respuesta = await controller.Filtrar(filtro);

            Assert.AreEqual(1, respuesta.Value.Count);
            Assert.AreEqual("Peli 1", respuesta.Value[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarProximosEstrenos()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var hoy = DateTime.Today;

            context.Peliculas.Add(new Pelicula() { Titulo = "Estreno Mañana", FechaEstreno = hoy.AddDays(1) });
            context.Peliculas.Add(new Pelicula() { Titulo = "Estreno Ayer", FechaEstreno = hoy.AddDays(-1) });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(context2, ConfigurarAutoMapper(),
                new Mock<IAlmacenadorArchivos>().Object, new Mock<ILogger<PeliculasController>>().Object);
            controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };

            var filtro = new FiltroPeliculasDTO() { ProximosEstrenos = true };
            var respuesta = await controller.Filtrar(filtro);

            Assert.AreEqual(1, respuesta.Value.Count);
            Assert.AreEqual("Estreno Mañana", respuesta.Value[0].Titulo);
        }


        [TestMethod]
        public async Task OrdenarPeliculasPorTituloDescendente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);

            context.Peliculas.Add(new Pelicula() { Titulo = "A" });
            context.Peliculas.Add(new Pelicula() { Titulo = "Z" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(context2, ConfigurarAutoMapper(), new Mock<IAlmacenadorArchivos>().Object, new Mock<ILogger<PeliculasController>>().Object);
            controller.ControllerContext = new ControllerContext() { HttpContext = new DefaultHttpContext() };

            var filtro = new FiltroPeliculasDTO() { CampoOrdenar = "Titulo", OrdenAscendente = false };
            var respuesta = await controller.Filtrar(filtro);

            Assert.AreEqual("Z", respuesta.Value[0].Titulo);
        }

        [TestMethod]
        public async Task ObtenerPeliculaPorIdInexistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var controller = new PeliculasController(context, ConfigurarAutoMapper(), new Mock<IAlmacenadorArchivos>().Object, new Mock<ILogger<PeliculasController>>().Object);

            var respuesta = await controller.Get(1);
            var resultado = respuesta.Result as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task BorrarPelicula()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            context.Peliculas.Add(new Pelicula() { Titulo = "A borrar", Poster = "poster.jpg" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();
            var controller = new PeliculasController(context2, ConfigurarAutoMapper(), mockAlmacenador.Object, new Mock<ILogger<PeliculasController>>().Object);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);
            mockAlmacenador.Verify(x => x.BorrarArchivo("poster.jpg", "peliculas"), Times.Once);
        }

        [TestMethod]
        public async Task OrdenarPeliculasPorTituloAscendente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);

            context.Peliculas.Add(new Pelicula() { Titulo = "Z" });
            context.Peliculas.Add(new Pelicula() { Titulo = "A" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(
                context2,
                ConfigurarAutoMapper(),
                new Mock<IAlmacenadorArchivos>().Object,
                new Mock<ILogger<PeliculasController>>().Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var filtro = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "Titulo",
                OrdenAscendente = true
            };

            var respuesta = await controller.Filtrar(filtro);

            Assert.AreEqual("A", respuesta.Value[0].Titulo);
        }

        [TestMethod]
        public async Task FiltrarPorTituloSinResultados()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);

            context.Peliculas.Add(new Pelicula() { Titulo = "Batman" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(
                context2,
                ConfigurarAutoMapper(),
                new Mock<IAlmacenadorArchivos>().Object,
                new Mock<ILogger<PeliculasController>>().Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var filtro = new FiltroPeliculasDTO()
            {
                Titulo = "Superman"
            };

            var respuesta = await controller.Filtrar(filtro);

            Assert.AreEqual(0, respuesta.Value.Count);
        }

        [TestMethod]
        public async Task OrdenarConCampoInvalido_NoDebeRomper()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);

            context.Peliculas.Add(new Pelicula() { Titulo = "A" });
            context.Peliculas.Add(new Pelicula() { Titulo = "B" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(
                context2,
                ConfigurarAutoMapper(),
                new Mock<IAlmacenadorArchivos>().Object,
                new Mock<ILogger<PeliculasController>>().Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var filtro = new FiltroPeliculasDTO()
            {
                CampoOrdenar = "CampoQueNoExiste"
            };

            var respuesta = await controller.Filtrar(filtro);

            Assert.IsNotNull(respuesta.Value);
        }

        [TestMethod]
        public async Task FiltrarPorGeneroYEnCines()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);

            var genero = new Genero() { Nombre = "Accion" };
            context.Add(genero);
            await context.SaveChangesAsync();

            var peli1 = new Pelicula() { Titulo = "Accion 1", EnCines = true };
            var peli2 = new Pelicula() { Titulo = "Accion 2", EnCines = false };

            context.AddRange(peli1, peli2);
            await context.SaveChangesAsync();

            context.Add(new PeliculasGeneros()
            {
                GeneroId = genero.Id,
                PeliculaId = peli1.Id
            });

            context.Add(new PeliculasGeneros()
            {
                GeneroId = genero.Id,
                PeliculaId = peli2.Id
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new PeliculasController(
                context2,
                ConfigurarAutoMapper(),
                new Mock<IAlmacenadorArchivos>().Object,
                new Mock<ILogger<PeliculasController>>().Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var filtro = new FiltroPeliculasDTO()
            {
                GeneroId = genero.Id,
                EnCines = true
            };

            var respuesta = await controller.Filtrar(filtro);

            Assert.AreEqual(1, respuesta.Value.Count);
            Assert.AreEqual("Accion 1", respuesta.Value[0].Titulo);
        }




    }
}