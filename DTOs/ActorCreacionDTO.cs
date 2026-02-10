using MasPelículasAPI.Validaciones;
using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class ActorCreacionDTO
    {
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        [PesoArchivoValidacion(4)]
        public IFormFile Foto { get; set; }
    }
}
