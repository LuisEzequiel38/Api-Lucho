using Microsoft.EntityFrameworkCore;
using Api_Lucho.Models;

namespace Api_Lucho.Context
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {
            
        }
        public DbSet<Producto> Productos { get; set; }
        public DbSet<Usuario> Usuarios { get; set; }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Usuario>(entity =>
            {
                entity.ToTable("Usuarios");
                entity.HasKey(u => u.Id); // Configura 'Id' como clave primaria
                entity.Property(u => u.Id)
                      .ValueGeneratedOnAdd(); // Asegúrate de que se generará al añadir
                entity.Property(u => u.NombreUsuario).IsRequired();
                entity.Property(u => u.Email).IsRequired();
            });

            modelBuilder.Entity<Producto>()
                .Property(p => p.Sku)
                .ValueGeneratedOnAdd();
        }
    }
}