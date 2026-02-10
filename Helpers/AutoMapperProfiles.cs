using AutoMapper;
using MasPelículasAPI.DTOs;
using MasPelículasAPI.Entidades;

namespace MasPelículasAPI.Helpers
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            // --- Mapeos Generales ---
            CreateMap<Genero, GeneroDTO>().ReverseMap();
            CreateMap<GeneroCreacionDTO, Genero>();

            CreateMap<Actor, ActorDTO>().ReverseMap();
            CreateMap<ActorCreacionDTO, Actor>()
                .ForMember(x => x.Foto, options => options.Ignore());
            CreateMap<ActorPatchDTO, Actor>().ReverseMap();

            CreateMap<Pelicula, PeliculaDTO>().ReverseMap();
            CreateMap<PeliculaPatchDTO, Pelicula>().ReverseMap();

            // --- Mapeo de CREACIÓN (DTO -> Entidad) ---
            CreateMap<PeliculaCreacionDTO, Pelicula>()
                .ForMember(x => x.Poster, options => options.Ignore())
                .ForMember(x => x.PeliculasGeneros, options => options.MapFrom(MapPeliculasGeneros))
                .ForMember(x => x.PeliculasActores, options => options.MapFrom(MapPeliculasActores));

            // --- Mapeo de LECTURA/DETALLES (Entidad -> DTO) --- 
            // Esto es lo nuevo que necesitas para el GET(id)
            CreateMap<Pelicula, PeliculaDetallesDTO>()
                .ForMember(x => x.Generos, options => options.MapFrom(MapPeliculaGeneros))
                .ForMember(x => x.Actores, options => options.MapFrom(MapPeliculaActores));
        }

        // =========================================================================
        // MÉTODOS PARA CREACIÓN (DTO -> Entidad)
        // Sirven para guardar en la BD las relaciones por ID
        // =========================================================================
        private List<PeliculasGeneros> MapPeliculasGeneros(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasGeneros>();
            if (peliculaCreacionDTO.GenerosIDs == null) { return resultado; }

            foreach (var id in peliculaCreacionDTO.GenerosIDs)
            {
                resultado.Add(new PeliculasGeneros() { GeneroId = id });
            }
            return resultado;
        }

        private List<PeliculasActores> MapPeliculasActores(PeliculaCreacionDTO peliculaCreacionDTO, Pelicula pelicula)
        {
            var resultado = new List<PeliculasActores>();
            if (peliculaCreacionDTO.Actores == null) { return resultado; }

            foreach (var actor in peliculaCreacionDTO.Actores)
            {
                resultado.Add(new PeliculasActores()
                {
                    ActorId = actor.ActorId,
                    Personaje = actor.Personaje
                });
            }
            return resultado;
        }

        // =========================================================================
        // MÉTODOS PARA LECTURA (Entidad -> DTO)
        // Sirven para sacar los datos de la BD y mostrarlos en el JSON
        // =========================================================================
        private List<GeneroDTO> MapPeliculaGeneros(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<GeneroDTO>();
            if (pelicula.PeliculasGeneros == null) { return resultado; }

            foreach (var generoPelicula in pelicula.PeliculasGeneros)
            {
                resultado.Add(new GeneroDTO()
                {
                    Id = generoPelicula.GeneroId,
                    Nombre = generoPelicula.Genero.Nombre
                });
            }
            return resultado;
        }

        private List<ActorPeliculaDetalleDTO> MapPeliculaActores(Pelicula pelicula, PeliculaDetallesDTO peliculaDetallesDTO)
        {
            var resultado = new List<ActorPeliculaDetalleDTO>();
            if (pelicula.PeliculasActores == null) { return resultado; }

            foreach (var actorPelicula in pelicula.PeliculasActores)
            {
                resultado.Add(new ActorPeliculaDetalleDTO()
                {
                    Id = actorPelicula.ActorId,
                    Nombre = actorPelicula.Actor.Nombre,
                    Personaje = actorPelicula.Personaje
                });
            }
            return resultado;
        }
    }
}