using AutoMapper;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades; // Necesario para la clase Actor
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore; // Necesario para ToListAsync y FirstOrDefaultAsync

namespace MasPelículasAPI.Controllers
{
    [ApiController]
    [Route("api/actores")]
    public class ActoresController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public ActoresController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<ActorDTO>>> Get()
        {
            var entidades = await context.Actores.ToListAsync();
            // Mapeamos de Entidad -> DTO
            var dtos = mapper.Map<List<ActorDTO>>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name = "obtenerActor")]
        public async Task<ActionResult<ActorDTO>> Get(int id)
        {
            var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<ActorDTO>(entidad);
            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ActorCreacionDTO actorCreacionDTO)
        {
            // Mapeamos de DTO de Creación -> Entidad
            var entidad = mapper.Map<Actor>(actorCreacionDTO);

            context.Add(entidad);
            await context.SaveChangesAsync();

            // Mapeamos de Entidad guardada -> DTO de Lectura (para devolverlo)
            var dto = mapper.Map<ActorDTO>(entidad);

            return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
        }

        // --- MÉTODOS AGREGADOS ---

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Put(int id, [FromBody] ActorCreacionDTO actorCreacionDTO)
        {
            // 1. Buscamos el actor en la BD
            var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

            // 2. Si no existe, error 404
            if (entidad == null)
            {
                return NotFound();
            }

            // 3. AutoMapper actualiza la entidad existente con los datos nuevos
            // IMPORTANTE: Aquí no creamos una variable nueva, usamos la instancia 'entidad'
            mapper.Map(actorCreacionDTO, entidad);

            // 4. Guardamos los cambios
            await context.SaveChangesAsync();
            return NoContent(); // 204 Sin contenido (estándar para Updates)
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            // 1. Verificamos si existe
            var existe = await context.Actores.AnyAsync(x => x.Id == id);

            if (!existe)
            {
                return NotFound();
            }

            // 2. Removemos (truco de optimización: instanciamos uno nuevo con el ID para borrarlo sin traer todos los datos)
            context.Remove(new Actor() { Id = id });

            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}