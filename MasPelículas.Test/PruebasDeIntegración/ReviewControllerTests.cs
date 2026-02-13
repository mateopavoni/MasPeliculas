using MasPeliculas.Tests;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace MasPelículas.Tests.PruebasDeIntegración
{
    [TestClass]
    public class ReviewControllerTests : BasePruebas
    {
        private static readonly string urlBase = "/api/peliculas/1/reviews";

        [TestMethod]
        public async Task Post_SiPeliculaNoExiste_Retorna404()
        {
            var nombreDB = Guid.NewGuid().ToString();
            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();

            var nuevaReview = new ReviewCreacionDTO { Puntuacion = 5, Comentario = "Excelente" };
            var contenido = new StringContent(JsonConvert.SerializeObject(nuevaReview), Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync(urlBase, contenido);

            Assert.AreEqual(HttpStatusCode.NotFound, respuesta.StatusCode);
        }

        [TestMethod]
        public async Task Post_Exitoso_GuardaUsuarioIdCorrectamente()
        {
            var nombreDB = Guid.NewGuid().ToString();
            using (var context = ConstruirContext(nombreDB))
            {
                context.Peliculas.Add(new Pelicula { Id = 1, Titulo = "Pelicula Test" });
                await context.SaveChangesAsync();
            }

            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();

            var nuevaReview = new ReviewCreacionDTO { Puntuacion = 5, Comentario = "Muy buena" };
            var contenido = new StringContent(JsonConvert.SerializeObject(nuevaReview), Encoding.UTF8, "application/json");

            var respuesta = await cliente.PostAsync(urlBase, contenido);

            Assert.AreEqual(HttpStatusCode.Created, respuesta.StatusCode);

            using (var context2 = ConstruirContext(nombreDB))
            {
                var reviewBD = await context2.Reviews.FirstOrDefaultAsync();
                Assert.IsNotNull(reviewBD);
                Assert.AreEqual("usuario_prueba_id", reviewBD.UsuarioId);
            }
        }

        [TestMethod]
        public async Task Put_SiUsuarioNoEsDuenio_Retorna403Forbid()
        {
            var nombreDB = Guid.NewGuid().ToString();
            using (var context = ConstruirContext(nombreDB))
            {
                context.Peliculas.Add(new Pelicula { Id = 1, Titulo = "Pelicula Test" });
                context.Reviews.Add(new Review { Id = 1, PeliculaId = 1, UsuarioId = "otro_id", Puntuacion = 1 });
                await context.SaveChangesAsync();
            }

            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();

            var reviewEditada = new ReviewCreacionDTO { Puntuacion = 5, Comentario = "Editada" };
            var contenido = new StringContent(JsonConvert.SerializeObject(reviewEditada), Encoding.UTF8, "application/json");

            var respuesta = await cliente.PutAsync($"{urlBase}/1", contenido);

            Assert.AreEqual(HttpStatusCode.Forbidden, respuesta.StatusCode);
        }

        [TestMethod]
        public async Task GetReview_RetornaDatosCorrectos()
        {
            var nombreDB = Guid.NewGuid().ToString();
            using (var context = ConstruirContext(nombreDB))
            {
                context.Peliculas.Add(new Pelicula { Id = 1, Titulo = "Inception" });
                context.Reviews.Add(new Review { Id = 1, PeliculaId = 1, UsuarioId = "1", Puntuacion = 5, Comentario = "Increible" });
                await context.SaveChangesAsync();
            }

            var factory = ConstruirWebApplicationFactory(nombreDB);
            var cliente = factory.CreateClient();

            var respuesta = await cliente.GetAsync($"{urlBase}/1");
            respuesta.EnsureSuccessStatusCode();

            var reviewDTO = JsonConvert.DeserializeObject<ReviewDTO>(await respuesta.Content.ReadAsStringAsync());
            Assert.AreEqual("Increible", reviewDTO.Comentario);
            Assert.AreEqual(5, reviewDTO.Puntuacion);
        }
    }
}