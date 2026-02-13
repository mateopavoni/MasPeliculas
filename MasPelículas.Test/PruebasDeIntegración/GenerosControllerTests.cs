using MasPeliculas.Tests;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MasPelículas.Tests.PruebasDeIntegración
{
    [TestClass]
    public class GenerosControllerTests : BasePruebas
    {
        private static readonly string url = "/api/generos";

        [TestMethod]
        public async Task ObtenerTodosLosGeneros_Vacio()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();

            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();
            var cuerpo = await respuesta.Content.ReadAsStringAsync();
            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(cuerpo);
            Assert.AreEqual(0, generos.Count);
        }

        [TestMethod]
        public async Task ObtenerTodosLosGeneros_ConDatos()
        {
            var nombreDB = Guid.NewGuid().ToString();
            using (var context = ConstruirContext(nombreDB))
            {
                context.Generos.Add(new Genero { Nombre = "Acción" });
                context.Generos.Add(new Genero { Nombre = "Comedia" });
                await context.SaveChangesAsync();
            }

            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();

            var respuesta = await cliente.GetAsync(url);

            respuesta.EnsureSuccessStatusCode();
            var cuerpo = await respuesta.Content.ReadAsStringAsync();
            var generos = JsonConvert.DeserializeObject<List<GeneroDTO>>(cuerpo);
            Assert.AreEqual(2, generos.Count);
        }

        [TestMethod]
        public async Task CrearGenero_Exitoso()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();

            var nuevoGenero = new GeneroCreacionDTO { Nombre = "Terror" };
            var contenido = new StringContent(JsonConvert.SerializeObject(nuevoGenero), Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync(url, contenido);

            Assert.AreEqual(System.Net.HttpStatusCode.Created, respuesta.StatusCode);

            using (var context = ConstruirContext(nombreDB))
            {
                var existe = await context.Generos.AnyAsync(x => x.Nombre == "Terror");
                Assert.IsTrue(existe);
            }
        }

        [TestMethod]
        public async Task BorrarGenero_Exitoso()
        {
            var nombreDB = Guid.NewGuid().ToString();
            using (var context = ConstruirContext(nombreDB))
            {
                context.Generos.Add(new Genero { Nombre = "Genero a borrar" });
                await context.SaveChangesAsync();
            }

            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();
            var respuesta = await cliente.DeleteAsync($"{url}/1");

            Assert.AreEqual(System.Net.HttpStatusCode.NoContent, respuesta.StatusCode);

            using (var context2 = ConstruirContext(nombreDB))
            {
                var existe = await context2.Generos.AnyAsync();
                Assert.IsFalse(existe);
            }
        }
    }
}