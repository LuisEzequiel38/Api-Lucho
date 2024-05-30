using Api_Lucho.Repository.Interfaces;
using Api_Lucho.DTOs;
using Microsoft.AspNetCore.Mvc;
using Api_Lucho.Services;

namespace Api_Lucho.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;

        public AuthController(IAuthService authService, JwtService jwtService)
        {
            _authService = authService;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioRegisterDto usuario)
        {
            try
            {
                var nuevoUsuario = await _authService.RegisterAsync(usuario.NombreUsuario, usuario.Password , usuario.Email , usuario.Role);

                if (nuevoUsuario != null)
                {
                    return Ok(new { message = "Usuario creado , bienvenido", nombre = nuevoUsuario.NombreUsuario, email = nuevoUsuario.Email, role = nuevoUsuario.Role });
                }
                else
                {
                    return BadRequest(new { message = "Error al registrar tu usuario." });
                }

            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuario)
        {
            var authenticatedUser = await _authService.AuthenticateAsync(usuario.Email, usuario.Password);
            if (authenticatedUser == null)
            {
                return Unauthorized(new { message = "No autorizado" });
            }

            var token = _jwtService.GenerateJwtToken(authenticatedUser);
            return Ok(new { token });
        }      
    }
}