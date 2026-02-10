using AutoMapper;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace MasPelículasAPI.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController : ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper = mapper;
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> GetAll()
        {
            var entidades = await context.Generos.ToListAsync();

            var dtos = mapper.Map<List<GeneroDTO>>(entidades);

            return dtos;
        }

        [HttpGet("{id:int}")] 
        public async Task<ActionResult<GeneroDTO>> GetById(int id)
        {
            var entidad = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            var dto = mapper.Map<GeneroDTO>(entidad);
            return dto;
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var entidad = mapper.Map<Genero>(generoCreacionDTO);

            context.Add(entidad);
            await context.SaveChangesAsync();

            var generoDto = mapper.Map<GeneroDTO>(entidad);

            return new CreatedAtActionResult(
                actionName: nameof(GetById),
                controllerName: "Generos",
                routeValues: new { id = generoDto.Id },
                value: generoDto);
        }

        [HttpPut("{id:int}")] 
        public async Task<ActionResult> Put(int id, [FromBody] GeneroCreacionDTO generoCreacionDTO)
        {
            var entidad = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            mapper.Map(generoCreacionDTO, entidad);

            await context.SaveChangesAsync();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var entidad = await context.Generos.FirstOrDefaultAsync(x => x.Id == id);

            if (entidad == null)
            {
                return NotFound();
            }

            context.Remove(entidad);
            await context.SaveChangesAsync();
            return NoContent();
        }
    }
}