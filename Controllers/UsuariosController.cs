using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Api_Lucho.Models;
using Api_Lucho.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Api_Lucho.Services;
using Api_Lucho.DTOs;

namespace Api_Lucho.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsuariosController : ControllerBase
    {
        private readonly IUsuarioRepository _usuarioRepository;
        private readonly UsuarioService _usuarioService;
        public UsuariosController(IUsuarioRepository usuarioRepository , UsuarioService usuarioService)
        {
            _usuarioRepository = usuarioRepository;
            _usuarioService = usuarioService;
        }

        //-------------------------------------------------Listar Usuarios
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Usuario>>> GetUsuarios()
        {
            var usuarios = await _usuarioRepository.GetUsuariosAsync(); 

            var usuariosDto = _usuarioService.ConvertirDtos(usuarios);

            //si es OK devuelve usuarios y codigo http 200
            return Ok(usuariosDto);
        }

        //-------------------------------------------------Listar usuario por id
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Usuario>> GetUsuario(int id)
        {
            var usuario = await _usuarioRepository.GetUsuarioAsync(id);

            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

         //si no es null pasa datos de la db a DtoUsuario para no exponerlos directamente y retorna usuario

            var usuarioDto = _usuarioService.ConvertirDto(usuario);
           
            return Ok(usuario);
        }

        //-------------------------------------------------Modificar Usuario
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUsuario([FromBody]int id , UsuarioInfo usuario)
        {
            if (usuario == null)
            {
                throw new Exception("Peticion vacia");
            }
            var existingUsuario = await _usuarioRepository.GetUsuarioAsync(id);
            if (existingUsuario == null)
            {
                //no encontrado , error 404
                return NoContent ();
            }

            try
            {
                await _usuarioRepository.UpdateUsuarioAsync(existingUsuario);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Ocurrio un error al intentar modificar la entidad.", ex);
            }
        }

        //-------------------------------------------------Borrar Usuario
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> DeleteUsuario(int id)
        {
            var usuario = await _usuarioRepository.GetUsuarioAsync(id);
            if (usuario == null)
            {
                return NotFound(new { message = "Usuario no encontrado." });
            }

            try
            {
                await _usuarioRepository.DeleteUsuarioAsync(id);
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