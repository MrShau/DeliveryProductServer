using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace DeliveryProductAPI.Server.Repositories
{
    public class RoleRepository : IRoleRepository
    {
        private readonly AppDbContext _context;

        public RoleRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Role?> AddAsync(Role role)
        {
            Role? result = (await _context.Roles.AddAsync(role))?.Entity;
            if (result != null)
                await SaveChangesAsync();
            return result;
        }

        public async Task<Role?> FirstAsync(int id, params string[] includes)
            => await _context.Roles.Includes(includes).FirstOrDefaultAsync(r => r.Id == id);

        public async Task<Role?> FirstAsync(string name, params string[] includes)
            => await _context.Roles.Includes(includes).FirstOrDefaultAsync(r => r.Name.ToUpper() == name.ToUpper());

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
