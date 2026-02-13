using MasPeliculas.Tests;
using MasPelículasAPI.Controllers;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace MasPelículas.Tests.PruebasUnitarias
{
    [TestClass]
    public class GeneroControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerTodosLosGeneros()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.Generos.Add(new Genero() { Nombre = "Acción" });
            context.Generos.Add(new Genero() { Nombre = "Drama" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(context2, mapper);

            var respuesta = await controller.Get();
            var generos = respuesta.Value;

            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.Generos.Add(new Genero() { Nombre = "Acción" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(context2, mapper);

            var respuesta = await controller.Get(1);
            var genero = respuesta.Value;

            Assert.IsNotNull(genero);
            Assert.AreEqual(1, genero.Id);
            Assert.AreEqual("Acción", genero.Nombre);
        }

        [TestMethod]
        public async Task ObtenerGeneroPorIdInexistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(context, mapper);

            var respuesta = await controller.Get(1);
            var resultado = respuesta.Result as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task CrearGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(context, mapper);
            var nuevoGenero = new GeneroCreacionDTO() { Nombre = "Terror" };

            var respuesta = await controller.Post(nuevoGenero);
            var resultado = respuesta as CreatedAtRouteResult;

            Assert.IsNotNull(resultado);
            Assert.AreEqual(201, resultado.StatusCode);

            var context2 = ConstruirContext(nombreBD);
            var cantidad = context2.Generos.Count();
            Assert.AreEqual(1, cantidad);
        }

        [TestMethod]
        public async Task EditarGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.Generos.Add(new Genero() { Nombre = "Comedia" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(context2, mapper);
            var dto = new GeneroCreacionDTO() { Nombre = "Comedia Editada" };

            var respuesta = await controller.Put(1, dto);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);

            var context3 = ConstruirContext(nombreBD);
            var existe = context3.Generos.Any(x => x.Nombre == "Comedia Editada");
            Assert.IsTrue(existe);
        }

        [TestMethod]
        public async Task EditarGeneroInexistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(context, mapper);
            var dto = new GeneroCreacionDTO() { Nombre = "No existe" };

            var respuesta = await controller.Put(1, dto);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task BorrarGenero()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.Generos.Add(new Genero() { Nombre = "Acción" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new GenerosController(context2, mapper);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);

            var context3 = ConstruirContext(nombreBD);
            var cantidad = context3.Generos.Count();
            Assert.AreEqual(0, cantidad);
        }

        [TestMethod]
        public async Task BorrarGeneroInexistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new GenerosController(context, mapper);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);
        }
    }
}