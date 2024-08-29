using SampleAPI.Entities;

namespace SampleAPI.Data
{
    public static class DbInitializer
    {
        public static void Initialize(SampleApiDbContext context)
        {
            context.Database.EnsureCreated();

            if (context.Orders.Any())
            {
                return;  
            }

            var random = new Random();

            for (int i = 0; i < 50; i++)
            {
                var order = new Order
                {
                    Name = $"Order {i + 1}",
                    Description = $"Description for Order {i + 1}",
                    EntryDate = DateTime.Now.AddDays(-random.Next(0, 10)),
                    IsInvoiced = random.Next(0, 2) == 1,
                    IsDeleted = false
                };
                context.Orders.Add(order);
            }

            context.SaveChanges(); 
        }
    }
}
