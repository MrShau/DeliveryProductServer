using DeliveryProductAPI.Server.Models;
using System.Data;

namespace DeliveryProductAPI.Server.Repositories.Interfaces
{
    public interface IRoleRepository
    {
        public Task<Role?> AddAsync(Role role);
        public Task<Role?> FirstAsync(int id, params string[] includes);
        public Task<Role?> FirstAsync(string name, params string[] includes);

        public Task SaveChangesAsync();
    }
}
