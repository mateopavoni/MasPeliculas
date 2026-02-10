using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.Entidades
{
    public class SalaDeCine: IId
    {
        public int Id { get; set; }
        [Required]
        [StringLength(120)]
        public string Name { get; set; }
        public List<PeliculasSalasDeCine> PeliculasSalasDeCines { get; set; }

    }
}
