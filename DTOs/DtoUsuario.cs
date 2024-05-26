namespace Api_Lucho.DTOs
{
    public class UsuarioRegisterDto
    {
        public required string NombreUsuario { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public required string Rol { get; set; }
    }

    public class UsuarioLoginDto
    {
        public required string Email { get; set; }
        public required string Password { get; set; }
    }

    public class UsuarioInfo
    {
        public int Id { get; set; }
        public string? Nombre { get; set; }
        public string? Email { get; set; }
        public string? Rol { get; set; }
    }
 }

