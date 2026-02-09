using AutoMapper;
using MasPelículasAPI.Entidades;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.Json;
using System.Runtime.CompilerServices;

namespace MasPelículasAPI.Controllers
{
    [ApiController]
    [Route("api/generos")]
    public class GenerosController: ControllerBase
    {
        private readonly ApplicationDbContext context;
        private readonly IMapper mapper;

        public GenerosController(ApplicationDbContext context, IMapper mapper)
        {
            this.context = context;
            this.mapper
        }

        [HttpGet]
        public async Task<ActionResult<List<GeneroDTO>>> GetAll()
        {
            var entidades = await context.Generos.ToListAsync();
            var dtos = mapper.Map<List<Genero>>(entidades);
            return dtos;
        }
    }
}
