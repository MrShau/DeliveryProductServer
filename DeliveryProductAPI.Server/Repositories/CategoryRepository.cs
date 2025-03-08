using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeliveryProductAPI.Server.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly AppDbContext _context;

        public CategoryRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Category?> AddAsync(Category category)
        {
            Category? result = (await _context.Categories.AddAsync(category))?.Entity;
            if (result != null)
                await SaveChangesAsync();
            return result;
        }

        public async Task<Category?> FirstAsync(int id, params string[] includes)
            => await _context.Categories.Includes(includes).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Category?> FirstWhereAsync(Expression<Func<Category, bool>> expression, params string[] includes)
            => await _context.Categories.Includes(includes).FirstOrDefaultAsync(expression);

        public async Task<List<Category>> WhereAsync(Expression<Func<Category, bool>> expression, params string[] includes)
            => await _context.Categories.Includes(includes).Where(expression).Where(c => c.Status).ToListAsync();

        public AppDbContext GetDbContext()
            => _context;

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }


    }
}
