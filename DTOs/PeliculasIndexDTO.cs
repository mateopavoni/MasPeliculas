namespace MasPelículasAPI.DTOs
{
    public class PeliculasIndexDTO
    {
        public List<PeliculaDTO> FuturosEstrenos { get; set; }
        public List<PeliculaDTO> PeliculasEnCines { get; set; }
    }
}