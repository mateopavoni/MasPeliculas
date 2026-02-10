using MasPelículasAPI.Validaciones;
using MasPeliculasAPI.Validaciones;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class ActorCreacionDTO: ActorPatchDTO
    {
        [PesoArchivoValidacion(pesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }
    }
}