using MasPelículasAPI.DTOs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace MasPelículasAPI.Controllers
{
    [ApiController]
    [Route("api/cuentas")]
    public class CuentasController : ControllerBase
    {
        private readonly UserManager<IdentityUser> userManager;
        private readonly SignInManager<IdentityUser> signInManager;
        private readonly IConfiguration configuration;
        private readonly ApplicationDbContext context;

        public CuentasController(UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            IConfiguration configuration,
            ApplicationDbContext context)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.context = context;
        }

        [HttpPost("registro")]
        public async Task<ActionResult> Registrar(UserInfo userInfo)
        {
            var usuario = new IdentityUser
            {
                UserName = userInfo.Email,
                Email = userInfo.Email
            };

            var resultado = await userManager.CreateAsync(usuario, userInfo.Password);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(userInfo);
            }
            else
            {
                foreach (var error in resultado.Errors)
                {
                    ModelState.AddModelError(string.Empty, error.Description);
                }

                return ValidationProblem();
            }
        }

        [HttpPost("login")]
        public async Task<ActionResult> Login(UserInfo userInfo)
        {
            var usuario = await userManager.FindByEmailAsync(userInfo.Email);

            if (usuario is null)
            {
                ModelState.AddModelError(string.Empty, "Login incorrecto");
                return ValidationProblem();
            }

            var resultado = await signInManager.CheckPasswordSignInAsync(usuario, userInfo.Password, lockoutOnFailure: false);

            if (resultado.Succeeded)
            {
                return await ConstruirToken(userInfo);
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Login incorrecto");
                return ValidationProblem();
            }
        }

        // Método auxiliar que devuelve un objeto anónimo
        private async Task<ActionResult> ConstruirToken(UserInfo userInfo)
        {
            var claims = new List<Claim>
            {
                new Claim("email", userInfo.Email),
                new Claim(JwtRegisteredClaimNames.Email, userInfo.Email)
            };

            var usuario = await userManager.FindByEmailAsync(userInfo.Email);
            var claimsDB = await userManager.GetClaimsAsync(usuario);

            claims.AddRange(claimsDB);

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(configuration["jwt:key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var expiration = DateTime.UtcNow.AddYears(1);

            var token = new JwtSecurityToken(
                issuer: null,
                audience: null,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            // AQUÍ ESTÁ EL CAMBIO: Devolvemos un objeto anónimo directamente con Ok()
            return Ok(new
            {
                token = new JwtSecurityTokenHandler().WriteToken(token),
                expiracion = expiration
            });
        }

        [HttpGet("listado")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
        public async Task<ActionResult<List<UsuarioDTO>>> GetUsuarios()
        {
            var usuarios = await context.Users
                .Select(u => new UsuarioDTO { Id = u.Id, Email = u.Email })
                .ToListAsync();

            return usuarios;
        }

        [HttpPost("asignar-rol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> AsignarRol(EditarRolDTO editarRolDTO)
        {
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UsuarioId);

            if (usuario == null) { return NotFound("Usuario no encontrado"); }

            await userManager.AddClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDTO.NombreRol));

            return NoContent();
        }

        [HttpPost("remover-rol")]
        [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme, Roles = "Admin")]
        public async Task<ActionResult> RemoverRol(EditarRolDTO editarRolDTO)
        {
            var usuario = await userManager.FindByIdAsync(editarRolDTO.UsuarioId);

            if (usuario == null) { return NotFound("Usuario no encontrado"); }

            await userManager.RemoveClaimAsync(usuario, new Claim(ClaimTypes.Role, editarRolDTO.NombreRol));

            return NoContent();
        }
    }
}