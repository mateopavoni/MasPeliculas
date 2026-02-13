using Microsoft.EntityFrameworkCore; 
using Microsoft.AspNetCore.Http;    
using System;
using System.Linq;
using System.Threading.Tasks;

namespace MasPelículasAPI.Helpers
{
    public static class HttpContextExtensions
    {
        public static async Task InsertarParametrosPaginacion<T>(
            this HttpContext httpContext,
            IQueryable<T> queryable,
            int cantidadRegistrosPorPagina)
        {
            double cantidad = await queryable.CountAsync();
            double cantidadPaginas = Math.Ceiling(cantidad / cantidadRegistrosPorPagina);

            httpContext.Response.Headers.Add("cantidadPaginas", cantidadPaginas.ToString());
        }
    }
}