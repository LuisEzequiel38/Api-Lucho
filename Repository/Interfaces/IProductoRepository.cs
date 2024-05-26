using Api_Lucho.Models;

namespace Api_Lucho.Repository.Interfaces
{
    public interface IProductoRepository
    {
        Task<IEnumerable<Producto>> GetProductosAsync();
        Task<Producto> GetProductoAsync(int sku);
        Task AddProductoAsync(Producto producto);
        Task UpdateProductoAsync(Producto producto);
        Task DeleteProductoAsync(int sku);
    }
}