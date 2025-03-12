using System.Linq.Expressions;
using System;
using DeliveryProductAPI.Server.Models;

namespace DeliveryProductAPI.Server.Repositories.Interfaces
{
    public interface IOrderRepository
    {
        public Task<Order?> AddAsync(Order order);
        public Task<Order?> FirstAsync(int id, params string[] includes);
        public Task<Order?> FirstWhereAsync(Expression<Func<Order, bool>> expression, params string[] includes);
        public Task<List<Order>> WhereAsync(Expression<Func<Order, bool>> expression, params string[] includes);
        public Task<OrderProduct> AddProductAsync(OrderProduct orderProduct);
        public Task<Status?> GetFirstStatus();
        public Task SaveChangesAsync();
        public AppDbContext GetDbContext();
    }
}
