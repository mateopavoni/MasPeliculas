using MasPelículasAPI.Controllers;
using MasPelículasAPI.DTOs;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using SignInResult = Microsoft.AspNetCore.Identity.SignInResult;

namespace MasPelículas.Tests.PruebasUnitarias
{
    [TestClass]
    public class CuentasControllerTests
    {
        private Mock<UserManager<IdentityUser>> userManagerMock;
        private Mock<SignInManager<IdentityUser>> signInManagerMock;
        private Mock<IConfiguration> configurationMock;

        private UserManager<TUser> BuildUserManager<TUser>(IUserStore<TUser> store = null) where TUser : class
        {
            store ??= new Mock<IUserStore<TUser>>().Object;
            var options = new Mock<IOptions<IdentityOptions>>();
            var idOptions = new IdentityOptions();
            options.Setup(o => o.Value).Returns(idOptions);
            var userValidators = new List<IUserValidator<TUser>> { new Mock<IUserValidator<TUser>>().Object };
            var pwdValidators = new List<IPasswordValidator<TUser>> { new Mock<IPasswordValidator<TUser>>().Object };

            return new UserManager<TUser>(store, options.Object, new PasswordHasher<TUser>(),
                userValidators, pwdValidators, new UpperInvariantLookupNormalizer(),
                new IdentityErrorDescriber(), null, new Mock<ILogger<UserManager<TUser>>>().Object);
        }

        private SignInManager<TUser> SetupSignInManager<TUser>(UserManager<TUser> manager, HttpContext context) where TUser : class
        {
            var contextAccessor = new Mock<IHttpContextAccessor>();
            contextAccessor.Setup(x => x.HttpContext).Returns(context);
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<TUser>>();

            return new SignInManager<TUser>(manager, contextAccessor.Object, claimsFactory.Object,
                new Mock<IOptions<IdentityOptions>>().Object, new Mock<ILogger<SignInManager<TUser>>>().Object,
                new Mock<Microsoft.AspNetCore.Authentication.IAuthenticationSchemeProvider>().Object,
                new Mock<IUserConfirmation<TUser>>().Object);
        }

        private CuentasController ConstruirCuentasController()
        {
            var store = new Mock<IUserStore<IdentityUser>>();
            userManagerMock = new Mock<UserManager<IdentityUser>>(
                store.Object, null, null, null, null, null, null, null, null);

            var contextAccessor = new Mock<IHttpContextAccessor>();
            var claimsFactory = new Mock<IUserClaimsPrincipalFactory<IdentityUser>>();
            signInManagerMock = new Mock<SignInManager<IdentityUser>>(
                userManagerMock.Object,
                contextAccessor.Object,
                claimsFactory.Object,
                null, null, null, null);

            configurationMock = new Mock<IConfiguration>();
            configurationMock.Setup(x => x["jwt:key"]).Returns("CLAVE_SUPER_SECRETA_DE_PRUEBA_1234567890");

            return new CuentasController(
                userManagerMock.Object,
                signInManagerMock.Object,
                configurationMock.Object,
                null);
        }

        [TestMethod]
        public async Task Registrar_Exitoso()
        {
            var controller = ConstruirCuentasController();

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(new IdentityUser());
            userManagerMock.Setup(x => x.GetClaimsAsync(It.IsAny<IdentityUser>()))
                .ReturnsAsync(new List<Claim>());

            var respuesta = await controller.Registrar(new UserInfo { Email = "test@test.com", Password = "123" });

            Assert.IsInstanceOfType(respuesta, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Login_Exitoso()
        {
            var controller = ConstruirCuentasController();
            var user = new IdentityUser();

            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>())).ReturnsAsync(user);
            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Success);
            userManagerMock.Setup(x => x.GetClaimsAsync(user)).ReturnsAsync(new List<Claim>());

            var respuesta = await controller.Login(new UserInfo { Email = "test@test.com", Password = "123" });

            Assert.IsInstanceOfType(respuesta, typeof(OkObjectResult));
        }

        [TestMethod]
        public async Task Registrar_Falla_RetornaValidationProblem()
        {
            var controller = ConstruirCuentasController();

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<IdentityUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError { Description = "Password too short" }));

            var respuesta = await controller.Registrar(new UserInfo { Email = "test@test.com", Password = "1" });

            Assert.IsInstanceOfType(respuesta, typeof(ObjectResult));
            var resultado = respuesta as ObjectResult;
            Assert.IsTrue(resultado.StatusCode == 400 || resultado.StatusCode == null);
        }

        [TestMethod]
        public async Task Login_Falla_PasswordIncorrecto_RetornaValidationProblem()
        {
            var controller = ConstruirCuentasController();
            var user = new IdentityUser();

            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            signInManagerMock.Setup(x => x.CheckPasswordSignInAsync(user, It.IsAny<string>(), false))
                .ReturnsAsync(SignInResult.Failed);

            var respuesta = await controller.Login(new UserInfo { Email = "test@test.com", Password = "wrong_password" });

            Assert.IsInstanceOfType(respuesta, typeof(ObjectResult));
            var resultado = respuesta as ObjectResult;
            Assert.IsTrue(resultado.StatusCode == 400 || resultado.StatusCode == null);
        }

        [TestMethod]
        public async Task AsignarRol_UsuarioNoExiste()
        {
            var controller = ConstruirCuentasController();

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            var respuesta = await controller.AsignarRol(new EditarRolDTO { UsuarioId = "1", NombreRol = "Admin" });

            Assert.IsInstanceOfType(respuesta, typeof(NotFoundObjectResult));
        }

        [TestMethod]
        public async Task RemoverRol_UsuarioNoExiste()
        {
            var controller = ConstruirCuentasController();

            userManagerMock.Setup(x => x.FindByIdAsync(It.IsAny<string>())).ReturnsAsync((IdentityUser)null);

            var respuesta = await controller.RemoverRol(new EditarRolDTO { UsuarioId = "1", NombreRol = "Admin" });

            Assert.IsInstanceOfType(respuesta, typeof(NotFoundObjectResult));
        }
    }
}