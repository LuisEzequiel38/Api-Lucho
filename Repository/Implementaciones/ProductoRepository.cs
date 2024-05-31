using Api_Lucho.Context;
using Api_Lucho.Models;
using Api_Lucho.Repository.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api_Lucho.Repository.Implementaciones
{
    public class ProductoRepository : IProductoRepository
    {
        private readonly AppDbContext _context;

        public ProductoRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Producto>> GetProductosAsync()
        {
            return await _context.Productos.ToListAsync();
        }

        public async Task<Producto> GetProductoAsync(int sku)
        {              
            return await _context.Productos.FindAsync(sku);
        }

        public async Task AddProductoAsync(Producto producto)
        {
            _context.Productos.Add(producto);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateProductoAsync(Producto producto)
        {
            _context.Productos.Update(producto);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductoAsync(int sku)
        {
            var producto = await _context.Productos.FindAsync(sku);
            if (producto != null)
            {
                _context.Productos.Remove(producto);
                await _context.SaveChangesAsync();
            }
        }
    }
}
