using Api_Lucho.Models;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Api_Lucho.Services
{
    public class JwtService
    {
        private readonly IConfiguration _configuration;
        public JwtService(IConfiguration configuration )
        {
            _configuration = configuration;
        }

        public string GenerateJwtToken(Usuario usuario)
        {
            string claimId = usuario.Id.ToString();
            string claimNombre = usuario.NombreUsuario;

            if (string.IsNullOrEmpty(usuario.Role)) throw new Exception("Rol null");

            if (string.IsNullOrEmpty(claimId)) throw new Exception("Id null"); 

            var claims = new[]
            {                
                new Claim(JwtRegisteredClaimNames.Sub, usuario.NombreUsuario),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(ClaimTypes.Role, usuario.Role ), // Aquí se incluye el rol del usuario
                new Claim(ClaimTypes.NameIdentifier, claimId),   
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: creds);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}