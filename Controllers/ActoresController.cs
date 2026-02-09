using AutoMapper;
using MasPelículasAPI.DTOs;
using Microsoft.AspNetCore.Mvc;

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
            var dtos = mapper.Map<List<ActorDTO>>(entidades);
            return dtos;
        }

        [HttpGet("{id:int}", Name="obtenerActor")]
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
    }
