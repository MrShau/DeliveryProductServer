using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeliveryProductAPI.Server.Repositories
{
    public class ChatRepository : IChatRepository
    {
        private readonly AppDbContext _context;

        public ChatRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Chat?> AddAsync(Chat chat)
        {
            var result = (await _context.Chats.AddAsync(chat))?.Entity;
            if (result == null)
                return null;
            await SaveChangesAsync();
            return result;
        }

        public async Task<Chat?> FirstAsync(int id, params string[] includes)
            => await _context.Chats.Includes(includes).FirstOrDefaultAsync(c => c.Id == id);

        public async Task<Chat?> FirstWhereAsync(Expression<Func<Chat, bool>> expression, params string[] includes)
            => await _context.Chats.Includes(includes).FirstOrDefaultAsync(expression);

        public AppDbContext GetDbContext()
            => _context;

        public async Task SaveChangesAsync()
            => await _context.SaveChangesAsync();

        public async Task<List<Chat>> WhereAsync(Expression<Func<Chat, bool>> expression, params string[] includes)
            => await _context.Chats.Includes(includes).Where(expression).ToListAsync();
    }
}
