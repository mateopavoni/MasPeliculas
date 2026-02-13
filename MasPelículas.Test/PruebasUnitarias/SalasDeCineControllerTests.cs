using MasPeliculas.Tests;
using MasPelículasAPI.Controllers;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NetTopologySuite;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MasPelículas.Tests.PruebasUnitarias
{
    [TestClass]
    public class SalasDeCineControllerTests : BasePruebas
    {
        private GeometryFactory geometryFactory = NtsGeometryServices.Instance.CreateGeometryFactory(srid: 4326);

        [TestMethod]
        public async Task ObtenerSalasDeCineCercanas()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.SalasDeCine.AddRange(new List<SalaDeCine>
            {
                new SalaDeCine { Nombre = "Cerca", Ubicacion = geometryFactory.CreatePoint(new Coordinate(0, 0)) },
                new SalaDeCine { Nombre = "Lejos", Ubicacion = geometryFactory.CreatePoint(new Coordinate(10, 10)) }
            });

            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new SalasDeCineController(context2, mapper, geometryFactory);

            var filtro = new SalaDeCineCercanoFiltroDTO
            {
                Latitud = 0,
                Longitud = 0,
                DistanciaEnKms = 5
            };

            var respuesta = await controller.Cercanos(filtro);
            var valor = respuesta.Value;

            Assert.IsNotNull(valor);
            Assert.IsTrue(valor.Count > 0);
            Assert.AreEqual("Cerca", valor[0].Nombre);
        }

        [TestMethod]
        public async Task ObtenerTodasLasSalas()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.SalasDeCine.Add(new SalaDeCine { Nombre = "Sala 1", Ubicacion = geometryFactory.CreatePoint(new Coordinate(1, 1)) });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new SalasDeCineController(context2, mapper, geometryFactory);

            var respuesta = await controller.Get();
            var salas = respuesta.Value;

            Assert.AreEqual(1, salas.Count);
        }

        [TestMethod]
        public async Task CrearSalaDeCine()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            var controller = new SalasDeCineController(context, mapper, geometryFactory);

            var nuevaSala = new SalaDeCineCreacionDTO
            {
                Nombre = "Nueva Sala",
                Latitud = 10,
                Longitud = 10
            };

            var respuesta = await controller.Post(nuevaSala);
            var resultado = respuesta as CreatedAtRouteResult;

            Assert.AreEqual(201, resultado.StatusCode);

            var context2 = ConstruirContext(nombreBD);
            Assert.AreEqual(1, context2.SalasDeCine.Count());
        }

        [TestMethod]
        public async Task BorrarSalaDeCine()
        {
            var nombreBD = Guid.NewGuid().ToString();
            var context = ConstruirContext(nombreBD);
            var mapper = ConfigurarAutoMapper();

            context.SalasDeCine.Add(new SalaDeCine { Nombre = "A Borrar", Ubicacion = geometryFactory.CreatePoint(new Coordinate(1, 1)) });
            await context.SaveChangesAsync();

            var context2 = ConstruirContext(nombreBD);
            var controller = new SalasDeCineController(context2, mapper, geometryFactory);

            var respuesta = await controller.Delete(1);
            var resultado = respuesta as StatusCodeResult;

            Assert.AreEqual(204, resultado.StatusCode);

            var context3 = ConstruirContext(nombreBD);
            Assert.AreEqual(0, context3.SalasDeCine.Count());
        }
    }
}