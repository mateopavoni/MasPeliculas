using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class ActorDTO
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }

        [StringLength(300)]
        public string? Foto { get; set; }
    }
}
