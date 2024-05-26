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
                Rol = usuario.Rol 
            });
        }
        public UsuarioInfo ConvertirDto(Usuario usuario)
        {
            return new UsuarioInfo
            {
                Id = usuario.Id,
                Nombre = usuario.NombreUsuario,
                Email = usuario.Email,
                Rol = usuario.Rol
            };
        }
    }
}
