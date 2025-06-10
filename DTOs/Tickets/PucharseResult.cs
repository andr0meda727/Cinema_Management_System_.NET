namespace Cinema_Management_System.DTOs.Tickets
{
    public class PurchaseResult
    {
        public bool Success { get; init; }
        public string? Message { get; init; }
        public List<int> TicketIds { get; init; } = new();

        public PurchaseResult(bool success, string? message, List<int> ticketIds)
        {
            Success = success;
            Message = message;
            TicketIds = ticketIds ?? new List<int>();
        }

        public static PurchaseResult CreateSuccessResult(List<int> ticketIds)
            => new PurchaseResult(true, null, ticketIds);

        public static PurchaseResult CreateFailureResult(string message)
            => new PurchaseResult(false, message, null);
    }
}
