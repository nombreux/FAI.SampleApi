using SampleAPI.Entities;
using SampleAPI.Requests;

namespace SampleAPI.Repositories
{
    public interface IOrderRepository
    {
        // TODO: Create repository methods.

        // Suggestions for repo methods:
        // public GetRecentOrders();
        // public AddNewOrder();

        public IEnumerable<Order> GetRecentOrders();
        public Guid AddOrder(Order order);
        IEnumerable<Order> GetOrdersWithinBusinessDays(int days);
    }
}
