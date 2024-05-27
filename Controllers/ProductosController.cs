using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using Api_Lucho.Models;
using Api_Lucho.Repository.Interfaces;
using Microsoft.AspNetCore.Authorization;

namespace Api_Lucho.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductosController : ControllerBase
    {
        private readonly IProductoRepository _productoRepository;
        public ProductosController(IProductoRepository productoRepository)
        {
            _productoRepository = productoRepository;
        }

        //-------------------------------------------------Listar Productos
        [HttpGet]

        public async Task<ActionResult<IEnumerable<Producto>>> GetProductos()
        {
            var productos = await _productoRepository.GetProductosAsync();
            //si es OK devuelve productos y codigo http 200
            return Ok(productos);
        }

        //-------------------------------------------------Listar producto por sku
        [HttpGet("{sku}")]
        public async Task<ActionResult<Producto>> GetProducto(int sku)
        {
            var producto = await _productoRepository.GetProductoAsync(sku);

            //si es null devuelve codigo http 404
            if (producto == null)
            {
                return NotFound();
            }
            //no es null , devuelve al producto
            return Ok(producto);
        }

        //-------------------------------------------------Crear Producto
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Producto>> CreateProducto(Producto producto)
        {
            try
            {
                await _productoRepository.AddProductoAsync(producto);
                return CreatedAtAction(nameof(GetProducto), new { sku = producto.Sku }, producto);
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Ocurrio un error al intentar guardar el producto.", ex);
            }
        }

        //-------------------------------------------------Modificar Producto
        [HttpPut("{sku}")]
        [Authorize]
        public async Task<IActionResult> UpdateProducto(int sku, Producto producto)
        {
            if (sku != producto.Sku)
            {
                //mala peticion , error 400
                return BadRequest();
            }

            var existingProducto = await _productoRepository.GetProductoAsync(sku);
            if (existingProducto == null)
            {
                //no encontrado , error 404
                return NotFound();
            }

            try
            {
                await _productoRepository.UpdateProductoAsync(producto);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Ocurrio un error al intentar modificar el producto.", ex);
            }
        }

        //-------------------------------------------------Borrar Producto
        [HttpDelete("{sku}")]
        [Authorize]
        public async Task<IActionResult> DeleteProducto(int sku)
        {
            var producto = await _productoRepository.GetProductoAsync(sku);
            if (producto == null)
            {
                return NotFound();
            }

            try
            {
                await _productoRepository.DeleteProductoAsync(sku);
                return NoContent();
            }
            catch (DbUpdateException ex)
            {
                throw new Exception("Ocurrio un error al intentar borrar el producto.", ex);
            }
        }
    }
}