using AutoMapper;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace MasPelículasAPI.Controllers
{
    [Route("api/peliculas/{peliculaId:int}/reviews")]
    [ApiController]
    public class ReviewController : CustomBaseController
    {
        public ReviewController(ApplicationDbContext context, IMapper mapper)
            : base(context, mapper)
        {
        }

        [HttpGet("{reviewId:int}", Name = "ObtenerReview")]
        public async Task<ActionResult<ReviewDTO>> GetPorId(int peliculaId, int reviewId)
        {
            var review = await context.Reviews
                .Include(x => x.Usuario)
                .FirstOrDefaultAsync(x => x.Id == reviewId && x.PeliculaId == peliculaId);

            if (review == null)
            {
                return NotFound();
            }

            return mapper.Map<ReviewDTO>(review);
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<ReviewDTO>> Post(int peliculaId, [FromBody] ReviewCreacionDTO reviewCreacionDTO)
        {
            var existePelicula = await context.Peliculas.AnyAsync(x => x.Id == peliculaId);
            if (!existePelicula)
            {
                return NotFound($"No existe película con id {peliculaId}");
            }

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;
            var usuarioNombre = HttpContext.User.Identity?.Name;

            var reviewExiste = await context.Reviews
                .AnyAsync(x => x.PeliculaId == peliculaId && x.UsuarioId == usuarioId);

            if (reviewExiste)
            {
                return BadRequest("Ya has escrito una reseña para esta película.");
            }

            var review = mapper.Map<Review>(reviewCreacionDTO);
            review.PeliculaId = peliculaId;
            review.UsuarioId = usuarioId;

            context.Add(review);
            await context.SaveChangesAsync();

            var reviewDTO = mapper.Map<ReviewDTO>(review);
            reviewDTO.UsuarioNombre = usuarioNombre;

            return CreatedAtRoute("ObtenerReview",
                new { peliculaId = peliculaId, reviewId = review.Id },
                reviewDTO);
        }

        [HttpPut("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Put(int peliculaId, int reviewId, [FromBody] ReviewCreacionDTO reviewCreacionDTO)
        {
            var reviewDB = await context.Reviews
                .FirstOrDefaultAsync(x => x.Id == reviewId && x.PeliculaId == peliculaId);

            if (reviewDB == null) { return NotFound(); }

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (reviewDB.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            reviewDB = mapper.Map(reviewCreacionDTO, reviewDB);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{reviewId:int}")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult> Delete(int peliculaId, int reviewId)
        {
            var reviewDB = await context.Reviews
                .FirstOrDefaultAsync(x => x.Id == reviewId && x.PeliculaId == peliculaId);

            if (reviewDB == null) { return NotFound(); }

            var usuarioId = HttpContext.User.Claims.FirstOrDefault(x => x.Type == ClaimTypes.NameIdentifier)?.Value;

            if (reviewDB.UsuarioId != usuarioId)
            {
                return Forbid();
            }

            context.Remove(reviewDB);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}