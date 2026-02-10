using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class SalaDeCineCreacionDTO
    {
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
    }
}
