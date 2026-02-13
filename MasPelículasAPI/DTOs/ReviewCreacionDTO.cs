using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class ReviewCreacionDTO
    {
        [Required]
        public string Comentario { get; set; }

        [Range(1, 5)]
        public int Puntuacion { get; set; }
    }
}