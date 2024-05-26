using Api_Lucho.Models;

namespace Api_Lucho.Repository.Interfaces
{
    public interface IUsuarioRepository 
    {
        Task<IEnumerable<Usuario>> GetUsuariosAsync();
        Task<Usuario?> GetUsuarioAsync(int id);
        Task<Usuario?> GetUsuarioPorEmailAsync(string email);
        Task<Usuario?> GetUsuarioNombreAsync(string nombreUsuario);
        Task AddUsuarioAsync(Usuario usuario);
        Task UpdateUsuarioAsync(Usuario usuario);
        Task DeleteUsuarioAsync(int id);
    }
}