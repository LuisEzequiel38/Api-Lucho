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

        //-----------------------Login
        public async Task<Usuario?> AuthenticateAsync(string email, string password)
        {
            var usuario = await _usuarioRepository.GetUsuarioPorEmailAsync(email);

            if (usuario == null)
            {
                throw new Exception("Email invalido");
            }
            else if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                throw new Exception("Contraseña invalida");
            }
            return usuario;
        }

        //-----------------------Registra usuario 
        public async Task<Usuario> RegisterAsync(string nombre, string password, string email, string role)
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
                Role = role
            };

            await _usuarioRepository.AddUsuarioAsync(usuario);
            return usuario;
        }

        //-----------------------Comprueba contraseña
        public async Task<Usuario?> AuthenticatePassAsync(int id, string password)
        {
            var usuario = await _usuarioRepository.GetUsuarioAsync(id);

            if (usuario == null)
            {
                throw new Exception("Escriba su contraseña actual");
            }

            if (!BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash))
            {
                throw new Exception("Contraseña invalida");
            }
            return usuario;
        }

        //-----------------------Cambia contraseña 
        public async Task<Usuario> ContraseñaAsync(int id, string password)
        {
            var existingUsuario = await _usuarioRepository.GetUsuarioAsync(id);

            if (existingUsuario == null)
            {
                throw new Exception("Usuario no existe");
            }

            var salt = BCrypt.Net.BCrypt.GenerateSalt();
            var hashedPassword = BCrypt.Net.BCrypt.HashPassword(password, salt);

            existingUsuario.PasswordHash = hashedPassword;
            existingUsuario.PasswordSalt = salt;

            return existingUsuario;
        }
    }
}