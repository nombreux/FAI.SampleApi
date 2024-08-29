namespace SampleAPI.Services
{
    public class HolidayService
    {
        private static readonly List<DateTime> Holidays = new List<DateTime>
        {

            new DateTime(DateTime.Now.Year, 1, 1), 
            new DateTime(DateTime.Now.Year, 12, 25),
            // Add other holidays as needed
        };

        public static bool IsHoliday(DateTime date)
        {
            return Holidays.Contains(date.Date);
        }
    }
}
