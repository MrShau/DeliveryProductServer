using System.Linq.Expressions;
using System;
using DeliveryProductAPI.Server.Models;

namespace DeliveryProductAPI.Server.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        public Task<Category?> AddAsync(Category category);
        public Task<Category?> FirstAsync(int id, params string[] includes);
        public Task<Category?> FirstWhereAsync(Expression<Func<Category, bool>> expression, params string[] includes);
        public Task<List<Category>> WhereAsync(Expression<Func<Category, bool>> expression, params string[] includes);

        public Task SaveChangesAsync();
        public AppDbContext GetDbContext();
    }
}
