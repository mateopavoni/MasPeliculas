using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.Entidades
{
    public class Actor : IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Nombre { get; set; } = null!;
        public DateTime FechaNacimiento { get; set; }
        [StringLength(300)]
        public string? Foto { get; set; }
        public List<PeliculasActores> PeliculasActores { get; set; }
    }
}