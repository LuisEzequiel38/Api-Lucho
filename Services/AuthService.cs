using Api_Lucho.Models;
using Api_Lucho.Repository.Interfaces;

namespace Api_Lucho.Services
{
    public class AuthService : IAuthService
    {
        private readonly IUsuarioRepository _usuarioRepository;

        public AuthService(IUsuarioRepository usuarioRepository)
        {
            _usuarioRepository = usuarioRepository;
        }

        public async Task<Usuario?> AuthenticateAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.GetUsuarioPorEmailAsync(email);

            if (usuario == null || !BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                return null;
            }
            return usuario;
        }

        public async Task<Usuario> RegisterAsync(string nombreUsuario, string password, string email, string rol )
        {
            var existingUsuario = await _usuarioRepository.GetUsuarioNombreAsync(nombreUsuario);
            if (existingUsuario != null)
            {
                throw new Exception("Username name already exists");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var usuario = new Usuario
            {
                Email = email,
                NombreUsuario = nombreUsuario,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Rol = rol
            };

            await _usuarioRepository.AddUsuarioAsync(usuario);
            return usuario;
        }
    }
}