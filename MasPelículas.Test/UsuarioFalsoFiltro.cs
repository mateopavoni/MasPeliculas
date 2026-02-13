using Microsoft.AspNetCore.Mvc.Filters;
using System.Security.Claims;
using System.Threading.Tasks;

namespace MasPelículas.Tests
{
    public class UsuarioFalsoFiltro : IAsyncActionFilter
    {
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, "usuario_prueba_id"),
                new Claim(ClaimTypes.Email, "ejemplo@prueba.com"),
                new Claim(ClaimTypes.Name, "usuario_prueba"),
                new Claim(ClaimTypes.Role, "Admin")
            };

            var identity = new ClaimsIdentity(claims, "TestAuthentication");
            context.HttpContext.User = new ClaimsPrincipal(identity);

            await next();
        }
    }
}