using Api_Lucho.Context;
using Api_Lucho.Repository.Implementaciones;
using Api_Lucho.Repository.Interfaces;
using Api_Lucho.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

                                        // Add services to the container.
                        
// -------------------------------------variable para cadena de conexion
var connectionString = builder.Configuration.GetConnectionString("ConnectionLu");

//---------------------------------------HABILITO LOGGING DE CONSULTAs sql

builder.Logging.ClearProviders();
builder.Logging.AddConsole();

builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("ConnectionLu"))
           .EnableSensitiveDataLogging() // Opcional: Incluye valores de parámetros en los logs
           .LogTo(Console.WriteLine, LogLevel.Information)); // Logea las consultas SQL a la consola

//--------------------------------------registrar servicioS para conexion

builder.Services.AddDbContext<AppDbContext>( options => options.UseNpgsql(connectionString) );

//--------------------------------------registrar repositorios 

builder.Services.AddScoped<IProductoRepository, ProductoRepository>();
builder.Services.AddScoped<IUsuarioRepository, UsuarioRepository>();

//--------------------------------------registra servicios

builder.Services.AddScoped<IAuthService, AuthService>();
builder.Services.AddScoped<UsuarioService>();
builder.Services.AddScoped<JwtService>();


builder.Services.AddControllers();

// ------------------------------------------Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//--------------------------------------------  JWT  -------------------------------------------//

//--------------------------------------Parametros de validacion del Token

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["Jwt:Key"] ?? ""))
    };
});
//------------------------------------Configuracion para autenticar con token jwt
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "My API", Version = "v1" });

    // Añadir configuración de seguridad para JWT
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        In = ParameterLocation.Header,
        Description = "Por favor ingrese el token JWT con el prefijo 'Bearer' en el campo de texto",
        Name = "Authorization",
        Type = SecuritySchemeType.ApiKey,
        Scheme = "Bearer"
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

//-------------------------------------------------

var app = builder.Build();

// Configure the HTTP request pipeline.

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();