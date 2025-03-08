using System.Linq.Expressions;
using System;
using DeliveryProductAPI.Server.Models;

namespace DeliveryProductAPI.Server.Repositories.Interfaces
{
    public interface IUserRepository
    {
        public Task<User?> AddAsync(User user);
        public Task<User?> FirstAsync(int id, params string[] includes);
        public Task<User?> FirstAsync(string login, params string[] includes);
        public Task<User?> FirstWhereAsync(Expression<Func<User, bool>> expression, params string[] includes);
        public Task<List<User>> WhereAsync(Expression<Func<User, bool>> expression, params string[] includes);
        public Task Delete(User user);
        public Task SaveChangesAsync();
        public AppDbContext GetDbContext();
    }
}
