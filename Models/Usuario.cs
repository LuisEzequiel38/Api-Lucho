using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api_Lucho.Models
{
    public class Usuario
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int Id { get; set; }

        public required string NombreUsuario { get; set; }
        public string? PasswordHash { get; set; }
        public string? PasswordSalt { get; set; }
        public string? Email { get; set; }
        public string? Role { get; set; }
        public bool Active { get; set; } = true;
        public DateTime? FechaBorrado {get; set;}
    }
}