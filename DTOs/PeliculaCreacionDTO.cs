using MasPelículasAPI.Helpers; // Make sure to import the namespace of the Binder
using MasPelículasAPI.Validaciones;
using MasPeliculasAPI.Validaciones;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace MasPelículasAPI.DTOs
{
    public class PeliculaCreacionDTO : PeliculaPatchDTO
    {
        [PesoArchivoValidacion(5)]
        [TipoArchivoValidacion(GrupoTipoArchivo.Imagen)]
        public IFormFile Poster { get; set; }
 
        [ModelBinder(BinderType = typeof(TypeBinder<List<int>>))]
        public List<int> GenerosIDs { get; set; }

        [ModelBinder(BinderType = typeof(TypeBinder<List<ActorPeliculasCreacionDTO>>))]
        public List<ActorPeliculasCreacionDTO> Actores { get; set; }
    }
}