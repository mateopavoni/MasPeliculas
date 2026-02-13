using AutoMapper;
using MasPelículasAPI;
using MasPelículasAPI.Controllers;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;
using MasPelículasAPI.Servicios;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

[ApiController]
[Route("api/actores")]
public class ActoresController : CustomBaseController
{
    private readonly IAlmacenadorArchivos almacenadorArchivos;
    private readonly string contenedor = "actores";

    public ActoresController(ApplicationDbContext context, IMapper mapper, IAlmacenadorArchivos almacenadorArchivos)
        : base(context, mapper)
    {
        this.almacenadorArchivos = almacenadorArchivos;
    }

    [HttpGet]
    public async Task<ActionResult<List<ActorDTO>>> Get([FromQuery] PaginacionDTO paginacionDTO)
    {
        return await Get<Actor, ActorDTO>(paginacionDTO);
    }

    [HttpGet("{id:int}", Name = "obtenerActor")]
    public async Task<ActionResult<ActorDTO>> Get(int id)
    {
        return await Get<Actor, ActorDTO>(id);
    }

    [HttpPost]
    public async Task<ActionResult> Post([FromForm] ActorCreacionDTO actorCreacionDTO)
    {
        var entidad = mapper.Map<Actor>(actorCreacionDTO);

        if (actorCreacionDTO.Foto != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                var contenido = memoryStream.ToArray();
                var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);

                entidad.Foto = await almacenadorArchivos.GuardarArchivo(
                    contenido,
                    extension,
                    contenedor,
                    actorCreacionDTO.Foto.ContentType);
            }
        }

        context.Add(entidad);
        await context.SaveChangesAsync();
        var dto = mapper.Map<ActorDTO>(entidad);
        return new CreatedAtRouteResult("obtenerActor", new { id = entidad.Id }, dto);
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult> Put(int id, [FromForm] ActorCreacionDTO actorCreacionDTO)
    {
        var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

        if (entidad == null)
        {
            return NotFound();
        }

        entidad = mapper.Map(actorCreacionDTO, entidad);

        if (actorCreacionDTO.Foto != null)
        {
            using (var memoryStream = new MemoryStream())
            {
                await actorCreacionDTO.Foto.CopyToAsync(memoryStream);
                var contenido = memoryStream.ToArray();
                var extension = Path.GetExtension(actorCreacionDTO.Foto.FileName);

                entidad.Foto = await almacenadorArchivos.EditarArchivo(
                    contenido,
                    extension,
                    contenedor,
                    entidad.Foto,
                    actorCreacionDTO.Foto.ContentType);
            }
        }

        await context.SaveChangesAsync();
        return NoContent();
    }

    [HttpDelete("{id:int}")]
    public async Task<ActionResult> Delete(int id)
    {
        var entidad = await context.Actores.FirstOrDefaultAsync(x => x.Id == id);

        if (entidad == null)
        {
            return NotFound();
        }

        context.Remove(entidad);
        await context.SaveChangesAsync();

        await almacenadorArchivos.BorrarArchivo(entidad.Foto, contenedor);

        return NoContent();
    }


    [HttpPatch("{id:int}")]
    public async Task<ActionResult> Patch(int id, [FromBody] JsonPatchDocument<ActorPatchDTO> patchDocument)
    {
        return await Patch<Actor, ActorPatchDTO>(id, patchDocument);
    }
}