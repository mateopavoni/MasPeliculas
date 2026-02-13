using MasPelículasAPI.Entidades; // Asegúrate de tener acceso a tu DbContext
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.EntityFrameworkCore;

namespace MasPelículasAPI.Helpers
{
    public class PeliculaExisteAttribute : TypeFilterAttribute
    {
        public PeliculaExisteAttribute() : base(typeof(PeliculaExisteFilter))
        {
        }

        private class PeliculaExisteFilter : IAsyncActionFilter
        {
            private readonly ApplicationDbContext context;

            public PeliculaExisteFilter(ApplicationDbContext context)
            {
                this.context = context;
            }

            public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
            {
                var parametro = context.ActionArguments["peliculaId"];

                if (parametro == null)
                {
                    context.Result = new BadRequestObjectResult("El parámetro 'peliculaId' es requerido en la ruta.");
                    return;
                }

                var peliculaId = (int)parametro;

                var existe = await this.context.Peliculas.AnyAsync(x => x.Id == peliculaId);

                if (!existe)
                {
                    context.Result = new NotFoundObjectResult($"No existe una película con el ID {peliculaId}");
                }
                else
                {
                    await next();
                }
            }
        }
    }
}