using System.Linq.Expressions;
using System;
using DeliveryProductAPI.Server.Models;

namespace DeliveryProductAPI.Server.Repositories.Interfaces
{
    public interface IChatRepository
    {
        public Task<Chat?> AddAsync(Chat chat);
        public Task<Chat?> FirstAsync(int id, params string[] includes);
        public Task<Chat?> FirstWhereAsync(Expression<Func<Chat, bool>> expression, params string[] includes);
        public Task<List<Chat>> WhereAsync(Expression<Func<Chat, bool>> expression, params string[] includes);

        public Task SaveChangesAsync();
        public AppDbContext GetDbContext();
    }
}
