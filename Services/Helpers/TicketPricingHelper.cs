using Cinema_Management_System.Models.Cinema;

namespace Cinema_Management_System.Services.Helpers
{
    public static class TicketPricingHelper
    {
        public static decimal GetSeatTypeMultiplier(SeatTypes type)
        {
            switch (type)
            {
                case SeatTypes.STANDARD: return 1.0m;
                case SeatTypes.VIP: return 1.3m;
                case SeatTypes.DOUBLE: return 1.8m;
                default: return 1.0m;
            }
        }
    }
}