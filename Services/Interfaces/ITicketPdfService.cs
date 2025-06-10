using Cinema_Management_System.DTOs.Tickets;

namespace Cinema_Management_System.Services.Interfaces
{
    public interface ITicketPdfService
    {
        public MemoryStream GeneratePdf(DetailedTicketDTO ticket);
    }
}
