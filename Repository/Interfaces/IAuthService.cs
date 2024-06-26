﻿using Api_Lucho.Models;

namespace Api_Lucho.Repository.Interfaces
{
    public interface IAuthService
    {
        Task<Usuario?> AuthenticateAsync(string email, string password);
        Task<Usuario> RegisterAsync(string nombreUsuario, string password, string email, string role);
        Task<Usuario> ContraseñaAsync(int id, string password);
        Task<Usuario?> AuthenticatePassAsync(int id, string password);
    }
}