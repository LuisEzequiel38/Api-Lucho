﻿using System.Net;

namespace Api_Lucho.Middleware
{
    public class ForbiddenMiddleware
    {
        private readonly RequestDelegate _next;

        public ForbiddenMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            await _next(context);

            if (context.Response.StatusCode == (int)HttpStatusCode.Forbidden)
            {
                context.Response.ContentType = "application/json";
                await context.Response.WriteAsync("{\"message\": \"No tienes permisos para acceder a este recurso.\"}");
            }
        }
    }
}