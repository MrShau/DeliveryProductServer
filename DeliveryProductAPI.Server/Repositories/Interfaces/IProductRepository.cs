using System.Linq.Expressions;
using System;
using DeliveryProductAPI.Server.Models;

namespace DeliveryProductAPI.Server.Repositories.Interfaces
{
    public interface IProductRepository
    {
        public Task<Product?> AddAsync(Product product);
        public Task<Product?> FirstAsync(int id, params string[] includes);
        public Task<Product?> FirstWhereAsync(Expression<Func<Product, bool>> expression, params string[] includes);
        public Task<List<Product>> WhereAsync(Expression<Func<Product, bool>> expression, params string[] includes);
        public Task Delete(Product product);
        public Task SaveChangesAsync();
        public AppDbContext GetDbContext();
    }
}
