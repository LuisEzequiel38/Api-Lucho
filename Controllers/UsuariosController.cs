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
                return NotFound();
            }

         //si no es null pasa datos de la db a DtoUsuario para no exponerlos directamente y retorna usuario

            var usuarioDto = _usuarioService.ConvertirDto(usuario);
           
            return Ok(usuario);
        }

        //-------------------------------------------------Modificar Usuario
        [HttpPut("{id}")]
        [Authorize]
        public async Task<IActionResult> UpdateUsuario(int id, Usuario usuario)
        {
            if (id != usuario.Id)
            {
                //mala peticion , error 400
                return BadRequest();
            }

            var existingUsuario = await _usuarioRepository.GetUsuarioAsync(id);
            if (existingUsuario == null)
            {
                //no encontrado , error 404
                return NotFound();
            }

            try
            {
                await _usuarioRepository.UpdateUsuarioAsync(usuario);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while saving the entity.", ex);
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
                return NotFound();
            }

            try
            {
                await _usuarioRepository.DeleteUsuarioAsync(id);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("An error occurred while deleting the entity.", ex);
            }
        }
    }
}