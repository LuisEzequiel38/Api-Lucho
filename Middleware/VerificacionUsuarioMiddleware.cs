using Api_Lucho.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;

namespace Api_Lucho.Middleware
{
    public class VerificacionUsuarioMiddleware 
    {
        private readonly RequestDelegate _next;

        public VerificacionUsuarioMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext contextoHttp, IUsuarioRepository usuarioRepository)
        {
            if (contextoHttp.GetEndpoint()?.Metadata?.GetMetadata<IAuthorizeData>() != null)
            {
                if (contextoHttp.User.Identity.IsAuthenticated)
                {
                    var userId = contextoHttp.User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                    if (!string.IsNullOrEmpty(userId))
                    {
                        var user = await usuarioRepository.GetUsuarioAsync(int.Parse(userId));
                        if (user == null)
                        {
                            contextoHttp.Response.StatusCode = StatusCodes.Status401Unauthorized;
                            await contextoHttp.Response.WriteAsync("Unauthorized: No autorizado.");
                            return;
                        }
                    }
                }
                await _next(contextoHttp);
            }
        }
    }
}