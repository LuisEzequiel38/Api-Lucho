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

            if (usuario == null  )
            {
                throw new Exception("Email invalido");
            }
            else if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                throw new Exception("Contraseña invalida");
            }

            return usuario;
        }

        public async Task<Usuario> RegisterAsync(string nombre, string password, string email, string rol )
        {
            var existingUsuario = await _usuarioRepository.GetUsuarioNombreAsync(nombre);
            var usuarioPorMail = await _usuarioRepository.GetUsuarioPorEmailAsync(email);
            if (existingUsuario != null)
            {
                throw new Exception("Nombre de usuario ocupado");
            }
            else if (usuarioPorMail != null)
            {
                throw new Exception("Email ya registrado");
            }


            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            var usuario = new Usuario
            {
                Email = email,
                NombreUsuario = nombre,
                PasswordHash = hashedPassword,
                PasswordSalt = salt,
                Rol = rol
            };

            await _usuarioRepository.AddUsuarioAsync(usuario);
            return usuario;
        }
    }
}