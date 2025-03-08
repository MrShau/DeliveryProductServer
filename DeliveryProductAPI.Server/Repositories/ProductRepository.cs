using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeliveryProductAPI.Server.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly AppDbContext _context;
        public ProductRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Product?> AddAsync(Product product)
        {
            Product? result = (await _context.Products.AddAsync(product))?.Entity;
            if (result != null)
                await SaveChangesAsync();
            return result;
        }

        public async Task Delete(Product product)
        {
            product.Status = false;
            await SaveChangesAsync();
        }

        public async Task<Product?> FirstAsync(int id, params string[] includes)
            => await _context.Products.Includes(includes).FirstOrDefaultAsync(p => p.Id == id);

        public async Task<Product?> FirstWhereAsync(Expression<Func<Product, bool>> expression, params string[] includes)
            => await _context.Products.Includes(includes).FirstOrDefaultAsync(expression);

        public AppDbContext GetDbContext()
            => _context;
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Product>> WhereAsync(Expression<Func<Product, bool>> expression, params string[] includes)
            => await _context.Products.Includes(includes).Where(expression).ToListAsync();
    }
}
