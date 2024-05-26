using Api_Lucho.Context;
using Api_Lucho.Models;
using Api_Lucho.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api_Lucho.Repository.Implementaciones
{
    public class UsuarioRepository : IUsuarioRepository
    {
        private readonly AppDbContext _context;

        public UsuarioRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Usuario>> GetUsuariosAsync()
        {
            return await _context.Usuarios.ToListAsync();
        }

        public async Task<Usuario?> GetUsuarioAsync(int id)
        {
            return await _context.Usuarios.FindAsync(id);
        }
        public async Task<Usuario?> GetUsuarioPorEmailAsync(string mailUsuario)
        {
            var usuarioPorMail = _context.Usuarios.FirstOrDefault(u => u.Email == mailUsuario);

            if (usuarioPorMail == null)
            {
                return null;
            }
            return await _context.Usuarios.FindAsync(usuarioPorMail.Id);
        }

        public async Task<Usuario?> GetUsuarioNombreAsync(string nombreUsuario)
        {
            var usuario = _context.Usuarios.FirstOrDefault(u => u.NombreUsuario == nombreUsuario);
            
            if (usuario == null)
            {
                return null;
            }
            return await _context.Usuarios.FindAsync(usuario.Id);
        }

        public async Task AddUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Add(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateUsuarioAsync(Usuario usuario)
        {
            _context.Usuarios.Update(usuario);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteUsuarioAsync(int id)
        {
            var usuario = await _context.Usuarios.FindAsync(id);
            if (usuario != null)
            {
                _context.Usuarios.Remove(usuario);
                await _context.SaveChangesAsync();
            }
        }
    }
}