using Api_Lucho.Models;
using Api_Lucho.DTOs;

namespace Api_Lucho.Services
{
    public class UsuarioService
    {
        public IEnumerable<UsuarioInfo> ConvertirDtos(IEnumerable<Usuario> usuarios)
        {
            return usuarios.Select(usuario => new UsuarioInfo
            {
                Id = usuario.Id,
                Nombre = usuario.NombreUsuario, 
                Email = usuario.Email,
                Role = usuario.Role
            });
        }
        public UsuarioInfo ConvertirDto(Usuario usuario)
        {
            return new UsuarioInfo
            {
                Id = usuario.Id,
                Nombre = usuario.NombreUsuario,
                Email = usuario.Email,
                Role = usuario.Role
            };
        }
        public UsuarioMod ConvertirDtoMod(Usuario usuario)
        {
            return new UsuarioMod
            {
                Nombre = usuario.NombreUsuario,
                Email = usuario.Email
            };
        }
    }
}
