using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api_Lucho.Models;
using Api_Lucho.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Api_Lucho.Services;
using Api_Lucho.DTOs;
using System.Security.Claims;

namespace Api_Lucho.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UsuarioService _usuarioService;
        private readonly IAuthService _authService;
        private readonly JwtService _jwtService;

        public UsuariosController( IUsuarioRepository usuarioRepository , UsuarioService usuarioService , IAuthService authService , JwtService jwtService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
            _authService = authService;
            _jwtService = jwtService;
        }

        //-------------------------------------------------Listar Usuarios
        [HttpGet]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _usuarioRepository.GetUsuariosAsync(); 

            var usuariosDto = _usuarioService.ConvertirDtos(usuarios);

            //si es OK devuelve usuarios y codigo http 200
            return Ok(usuariosDto);
        }

        //-------------------------------------------------Listar usuario por id
        [HttpGet("{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _usuarioRepository.GetUsuarioAsync(id);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

         //si no es null pasa datos de la db a DtoUsuario para no exponerlos directamente y retorna usuario

            var usuarioDto = _usuarioService.ConvertirDto(usuario);
           
            return Ok(usuarioDto);
        }

        //-------------------------------------------------Modificar Usuario
        [HttpPut("modificar-usuario")]
        [Authorize]
        public async Task<IActionResult> UpdateUsuario([FromBody] UsuarioMod usuarioMod)
        {
            if (usuarioMod.Nombre == null || usuarioMod.Email == null)
            {
                throw new Exception("Complete todos los campos");
            }
            
            var nombre = User.FindFirst(ClaimTypes.NameIdentifier);

            if (nombre == null)
            {
                throw new Exception("error");
            }

            string nombreString = nombre.Value.ToString();

            if (nombre == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            var existingUsuario = await _usuarioRepository.GetUsuarioNombreAsync(nombreString);

            if (existingUsuario == null)
            {
                //no encontrado , error 404
                return NoContent ();
            }

            try
            {   // Mapear los campos del DTO a la entidad

                existingUsuario.NombreUsuario = usuarioMod.Nombre;
                existingUsuario.Email = usuarioMod.Email;

                await _usuarioRepository.UpdateUsuarioAsync(existingUsuario);

                var newToken = _jwtService.GenerateJwtToken(existingUsuario);

                return Ok ( new { message = "Usuario actualizado." ,newToken } );

            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Ocurrio un error al intentar modificar la entidad.", ex);
            }
        }

        //-------------------------------------------------Cambiar contraseña
        [HttpPut("cambiar-contraseña")]
        [Authorize]
        public async Task<IActionResult> ContraseñaUsuario([FromBody] UsuarioPass usuarioPass)
        {
            if (usuarioPass == null)
            {
                throw new Exception("Peticion vacia");
            }
            //----------------  Busca usuario por nombre

            var nombre = User.FindFirst(ClaimTypes.NameIdentifier);

            if (nombre == null)
            {
                throw new Exception("error");
            }

            string nombreString = nombre.Value.ToString();

            if (nombre == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            var existingUsuario = await _usuarioRepository.GetUsuarioNombreAsync(nombreString);

            if (existingUsuario == null)
            {
                //no encontrado , error 404
                return NoContent();
            }

            try
            {
                //----------- Comprueba contraseña 

                var authenticatedUser = await _authService.AuthenticatePassAsync(existingUsuario.Id , usuarioPass.Password);

                //----------- Cambia contraseña

                await _authService.ContraseñaAsync(existingUsuario.Id , usuarioPass.NewPassword);

                //----------- Guarda

                await _usuarioRepository.UpdateUsuarioAsync(existingUsuario);
                return Ok(new { message = "Contraseña actualizada." });

            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Ocurrio un error al cambiar la contraseña", ex);
            }
        }
        //-------------------------------------------------Borrar Usuario
        [HttpDelete("borrar-usuario")]
        [Authorize]
        public async Task<IActionResult> DeleteUsuario()

        {   //----------------  Busca usuario por nombre

            var nombre = User.FindFirst(ClaimTypes.NameIdentifier);

            if (nombre == null)
            {
                throw new Exception("error");
            }

            string nombreString = nombre.Value.ToString();

            if (nombre == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            var usuario = await _usuarioRepository.GetUsuarioNombreAsync(nombreString);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            //----------------------Borra usuario y guarda
            try
            {
                usuario.Active = false;
                usuario.FechaBorrado = DateTime.UtcNow;

                await _usuarioRepository.UpdateUsuarioAsync(usuario);
                var Eliminado = _usuarioService.ConvertirDto(usuario);

                return Ok(new { message = "Usuario eliminado.", Eliminado }); 
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Hubo un error al intentar eliminar la entidad.", ex);
            }
        }

        //-------------------------------------------------Borrar Usuario por Id ----(Admin)----
        [HttpDelete("borrar-cliente/{id}")]
        [Authorize(Policy = "AdminPolicy")]
        public async Task<IActionResult> DeleteUsuarioId(int id)

        {
            var nombre = User.FindFirst(ClaimTypes.NameIdentifier);

            if (nombre == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            string nombreString = nombre.Value.ToString();                            

            var adminClaim = await _usuarioRepository.GetUsuarioNombreAsync(nombreString);

            if (adminClaim == null)
            {
                throw new Exception("Claim null");
            }

            if (id == adminClaim.Id)
            {
                throw new Exception("Solo elimina clientes");
            }

            //----------------  Busca usuario 

            var usuario = await _usuarioRepository.GetUsuarioAsync(id);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }
            //----------------------Verifica que sea cliente

            if (usuario.Role != "cliente")
            {
                throw new Exception("Solo elimina clientes");
            }

            //----------------------Ejecuta borrado logico y guarda
            try
            {
                usuario.Active = false;
                usuario.FechaBorrado = DateTime.UtcNow;
                await _usuarioRepository.UpdateUsuarioAsync(usuario);
                var Eliminado = _usuarioService.ConvertirDto(usuario);
                return Ok(new { message = "Usuario eliminado.", Eliminado });
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Hubo un error al intentar eliminar la entidad.", ex);
            }
        }
    }
}