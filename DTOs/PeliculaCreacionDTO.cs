using MasPelículasAPI.Validaciones;
using MasPeliculasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class PeliculaCreacionDTO: PeliculaPatchDTO
    {
        [PesoArchivoValidacion(5)]
        [TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }
    }
}
