using Microsoft.EntityFrameworkCore;
using SampleAPI.Entities;
using SampleAPI.Requests;
using SampleAPI.Services;

namespace SampleAPI.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly SampleApiDbContext _context;


        public OrderRepository(SampleApiDbContext context)
        {
            _context = context;
        }

        public IEnumerable<Order> GetRecentOrders()
        {
            return _context.Orders
                           .Where(order => (DateTime.Now - order.EntryDate).TotalDays < 1 && !order.IsDeleted)
                           .OrderByDescending(order => order.EntryDate)
                           .ToList();
        }

        public Guid AddOrder(Order order)
        {
            _context.Orders.Add(order);
            _context.SaveChanges(); 
            return order.Id;
        }
        private bool IsWeekendOrHoliday(DateTime date)
        {
            return date.DayOfWeek == DayOfWeek.Saturday ||
                   date.DayOfWeek == DayOfWeek.Sunday ||
                   HolidayService.IsHoliday(date);
        }

        public IEnumerable<Order> GetOrdersWithinBusinessDays(int days)
        {
            DateTime currentDate = DateTime.Now;
            int daysCounted = 0;

            while (daysCounted < days)
            {
                currentDate = currentDate.AddDays(-1);

                if (!IsWeekendOrHoliday(currentDate))
                {
                    daysCounted++;
                }
            }

            return _context.Orders
                           .Where(order => order.EntryDate >= currentDate && !order.IsDeleted)
                           .OrderByDescending(order => order.EntryDate)
                           .ToList();
        }

    }
}
