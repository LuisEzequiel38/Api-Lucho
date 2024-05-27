using Api_Lucho.Repository.Interfaces;
using Api_Lucho.DTOs;
using Microsoft.AspNetCore.Mvc;
using Api_Lucho.Services;
using Api_Lucho.Repository.Implementaciones;
using Microsoft.AspNetCore.Http.HttpResults;
using Api_Lucho.Models;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;

namespace Api_Lucho.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AuthController : Controller
    {
        private readonly IAuthService _authRepository;
        private readonly JwtService _jwtService;

        public AuthController(IAuthService authRepository, JwtService jwtService)
        {
            _authRepository = authRepository;
            _jwtService = jwtService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UsuarioRegisterDto usuario)
        {
            try
            {
                var nuevoUsuario = await _authRepository.RegisterAsync(usuario.NombreUsuario, usuario.Password , usuario.Email , usuario.Rol);
                return CreatedAtAction (nameof(UsuarioRepository.GetUsuario), new { nombre = nuevoUsuario.NombreUsuario }, nuevoUsuario);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message + " ---- Hacelo bien la proxima :)" });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UsuarioLoginDto usuario)
        {
            var authenticatedUser = await _authRepository.AuthenticateAsync(usuario.Email, usuario.Password);
            if (authenticatedUser == null)
            {
                return Unauthorized(new { message = "Invalid email or password" });
            }

            var token = _jwtService.GenerateJwtToken(authenticatedUser);
            return Ok(new { token });
        }      
    }
}