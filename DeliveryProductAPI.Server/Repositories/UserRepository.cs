using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeliveryProductAPI.Server.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly AppDbContext _context;

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<User?> AddAsync(User user)
        {
            User? result = (await _context.Users.AddAsync(user))?.Entity;
            if (result != null)
                await SaveChangesAsync();
            return result;
        }

        public async Task Delete(User user)
        {
            user.Status = false;
            await SaveChangesAsync();
        }

        public async Task<User?> FirstAsync(int id, params string[] includes)
            => await _context.Users.Includes(includes).FirstOrDefaultAsync(u => u.Id == id);

        public async Task<User?> FirstAsync(string login, params string[] includes)
            => await _context.Users.Includes(includes).FirstOrDefaultAsync(u => u.Login.ToLower() == login.ToLower());

        public async Task<User?> FirstWhereAsync(Expression<Func<User, bool>> expression, params string[] includes)
            => await _context.Users.Includes(includes).FirstOrDefaultAsync(expression);

        public async Task<List<User>> WhereAsync(Expression<Func<User, bool>> expression, params string[] includes)
            => await _context.Users.Includes(includes).Where(expression).ToListAsync();

        public AppDbContext GetDbContext()
            => _context;

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
