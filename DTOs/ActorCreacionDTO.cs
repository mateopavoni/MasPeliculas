using MasPelículasAPI.Validaciones;
using MasPeliculasAPI.Validaciones;
using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class ActorCreacionDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; } = null!;

        public DateTime FechaNacimiento { get; set; }

        [PesoArchivoValidacion(pesoMaximoEnMegaBytes: 4)]
        [TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
        public IFormFile Foto { get; set; }
    }
}