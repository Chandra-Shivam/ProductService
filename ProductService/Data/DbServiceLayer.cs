using System;
using Microsoft.EntityFrameworkCore;
using ProductService.DBContext;
using ProductService.Models;

namespace ProductService.Data;

public class DbServiceLayer
{
    private readonly ProductDbContext _context;

    public DbServiceLayer(ProductDbContext context)
    {
        _context = context;
    }

    // Example method to fetch products
    public async Task<List<Product>> GetProductsAsync()
    {
        return await _context.Products.ToListAsync();
    }

    // Example method to add a product
    public async Task<int> AddProductAsync(Product product)
    {
        _context.Products.Add(product);
        return await _context.SaveChangesAsync();
    }

    public async Task<int> UpdateProductAsync(Product product)
    {
        _context.Products.Update(product);
        return await _context.SaveChangesAsync();
    }
}
