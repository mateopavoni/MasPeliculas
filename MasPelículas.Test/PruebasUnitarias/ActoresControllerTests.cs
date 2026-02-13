using MasPeliculas.Tests;
using MasPelículasAPI.Controllers;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using MasPelículasAPI.Servicios;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace MasPelículas.Tests.PruebasUnitarias
{
    [TestClass]
    public class ActoresControllerTests : BasePruebas
    {
        [TestMethod]
        public async Task ObtenerActoresPaginados()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.Actores.Add(new Actor() { Nombre = "Actor 1" });
            context.Actores.Add(new Actor() { Nombre = "Actor 2" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();
            var controller = new ActoresController(context2, mapper, mockAlmacenador.Object);

            controller.ControllerContext = new ControllerContext()
            {
                HttpContext = new DefaultHttpContext()
            };

            var respuesta = await controller.Get(new PaginacionDTO() { Pagina = 1, CantidadRegistrosPorPagina = 10 });
            var actores = respuesta.Value;

            Assert.AreEqual(2, actores.Count);
        }

        [TestMethod]
        public void PaginacionDTO_NoDebeSuperarMaximoDeRegistros()
        {
            var paginacion = new PaginacionDTO();
            var cantidadSolicitada = 100;
            var maximoEsperado = 50;

            paginacion.CantidadRegistrosPorPagina = cantidadSolicitada;

            Assert.AreEqual(maximoEsperado, paginacion.CantidadRegistrosPorPagina);
        }

        [TestMethod]
        public async Task ObtenerActorPorIdExistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();

            context.Actores.Add(new Actor() { Nombre = "Tom Hardy" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new ActoresController(context2, mapper, mockAlmacenador.Object);

            var respuesta = await controller.Get(1);
            var actor = respuesta.Value;

            Assert.IsNotNull(actor);
            Assert.AreEqual("Tom Hardy", actor.Nombre);
        }

        [TestMethod]
        public async Task ObtenerActorPorIdInexistente()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();

            var controller = new ActoresController(context, mapper, mockAlmacenador.Object);

            var respuesta = await controller.Get(1);
            var resultado = respuesta.Result as StatusCodeResult;

            Assert.AreEqual(404, resultado.StatusCode);
        }

        [TestMethod]
        public async Task CrearActorSinFoto()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();

            var controller = new ActoresController(context, mapper, mockAlmacenador.Object);
            var nuevoActor = new ActorCreacionDTO() { Nombre = "Actor Sin Foto", FechaNacimiento = DateTime.Now };

            var respuesta = await controller.Post(nuevoActor);
            var resultado = respuesta as CreatedAtRouteResult;

            Assert.AreEqual(201, resultado.StatusCode);

            var context2 = ConstruirContext(nombreBD);
            Assert.AreEqual(1, context2.Actores.Count());
        }

        [TestMethod]
        public async Task CrearActorConFoto()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();

            mockAlmacenador
                .Setup(x => x.GuardarArchivo(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("http://fake-url.com/foto.jpg");

            var controller = new ActoresController(context, mapper, mockAlmacenador.Object);

            var mockValidator = new Mock<IObjectModelValidator>();
            controller.ObjectValidator = mockValidator.Object;

            var contenido = "contenido fake"u8.ToArray();
            var ms = new MemoryStream(contenido);

            var archivo = new FormFile(ms, 0, ms.Length, "Foto", "test.jpg")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/jpeg"
            };

            var nuevoActor = new ActorCreacionDTO()
            {
                Nombre = "Con Foto",
                Foto = archivo
            };

            await controller.Post(nuevoActor);

            var context2 = ConstruirContext(nombreBD);
            var actorBD = await context2.Actores.FirstAsync();

            Assert.AreEqual("http://fake-url.com/foto.jpg", actorBD.Foto);
        }


        [TestMethod]
        public async Task EditarActorConFotoNueva()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();

            context.Actores.Add(new Actor()
            {
                Nombre = "Viejo",
                Foto = "url_vieja.jpg"
            });

            await context.SaveChangesAsync();

            mockAlmacenador
                .Setup(x => x.GuardarArchivo(
                    It.IsAny<byte[]>(),
                    It.IsAny<string>(),
                    It.IsAny<string>(),
                    It.IsAny<string>()))
                .ReturnsAsync("url_nueva.jpg");

            var context2 = ConstruirContext(nombreBD);
            var controller = new ActoresController(context2, mapper, mockAlmacenador.Object);

            var mockValidator = new Mock<IObjectModelValidator>();
            controller.ObjectValidator = mockValidator.Object;

            var ms = new MemoryStream("nueva foto"u8.ToArray());

            var archivo = new FormFile(ms, 0, ms.Length, "Foto", "nuevo.png")
            {
                Headers = new HeaderDictionary(),
                ContentType = "image/png"
            };

            var dto = new ActorCreacionDTO()
            {
                Nombre = "Editado",
                Foto = archivo
            };

            var respuesta = await controller.Put(1, dto);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);
        }


        [TestMethod]
        public async Task BorrarActor()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.Actores.Add(new Actor() { Nombre = "A eliminar", Foto = "foto.jpg" });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();

            var controller = new ActoresController(context2, mapper, mockAlmacenador.Object);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);
            mockAlmacenador.Verify(x => x.BorrarArchivo("foto.jpg", "actores"), Times.Once);
        }

        [TestMethod]
        public async Task PatchActor()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();
            var mockAlmacenador = new Mock<IAlmacenadorArchivos>();

            context.Actores.Add(new Actor() { Nombre = "Original", FechaNacimiento = DateTime.Now });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new ActoresController(context2, mapper, mockAlmacenador.Object);

            var mockValidator = new Mock<IObjectModelValidator>();
            controller.ObjectValidator = mockValidator.Object;

            var patchDoc = new JsonPatchDocument<ActorPatchDTO>();
            patchDoc.Replace(x => x.Nombre, "Parcheado");

            var respuesta = await controller.Patch(1, patchDoc);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);

            var context3 = ConstruirContext(nombreBD);
            var actorEnBD = await context3.Actores.FirstAsync();
            Assert.AreEqual("Parcheado", actorEnBD.Nombre);
        }
    }
}