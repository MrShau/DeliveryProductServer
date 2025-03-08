using DeliveryProductAPI.Server.Models;
using DeliveryProductAPI.Server.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace DeliveryProductAPI.Server.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly AppDbContext _context;

        public OrderRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Order?> AddAsync(Order order)
        {
            Order? result = (await _context.Orders.AddAsync(order))?.Entity;
            if (result != null)
                await SaveChangesAsync();
            return result;
        }

        public async Task<OrderProduct?> AddProductAsync(OrderProduct orderProduct)
        {
            if (orderProduct == null)
                return null;

            OrderProduct? product = (await _context.OrderProducts.AddAsync(orderProduct))?.Entity;
            if (product == null)
                return null;

            await SaveChangesAsync();

            return product;

        }

        public async Task<Order?> FirstAsync(int id, params string[] includes)
            => await _context.Orders.Includes(includes).FirstOrDefaultAsync(o => o.Id == id);

        public async Task<Order?> FirstWhereAsync(Expression<Func<Order, bool>> expression, params string[] includes)
            => await _context.Orders.Includes(includes).FirstOrDefaultAsync(expression);

        public AppDbContext GetDbContext()
            => _context;

        public async Task<Status?> GetFirstStatus()
            => await _context.Statuses.FirstOrDefaultAsync();

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }

        public async Task<List<Order>> WhereAsync(Expression<Func<Order, bool>> expression, params string[] includes)
            => await _context.Orders.Includes(includes).Where(expression).ToListAsync();
    }
}
