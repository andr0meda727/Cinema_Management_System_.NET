using Cinema_Management_System.DTOs.Tickets;
using Cinema_Management_System.Services.Interfaces;
using QRCoder;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace Cinema_Management_System.Services.PDF
{
    public class TicketPdfService : ITicketPdfService
    {
        public MemoryStream GeneratePdf(DetailedTicketDTO ticket)
        {
            var document = new TicketPdfDocument(ticket);
            var stream = new MemoryStream();
            document.GeneratePdf(stream);
            stream.Position = 0;
            return stream;
        }
    }

    public class TicketPdfDocument : IDocument
    {
        private readonly DetailedTicketDTO _ticket;

        public TicketPdfDocument(DetailedTicketDTO ticket)
        {
            _ticket = ticket;
        }

        public DocumentMetadata GetMetadata() => DocumentMetadata.Default;

        public void Compose(IDocumentContainer container)
        {
            container.Page(page =>
            {
                page.Margin(30);
                page.Size(PageSizes.A5.Landscape());

                page.Content().Row(row =>
                {
                    row.RelativeItem(1).Image(GetPosterImagePath());

                    row.RelativeItem(2).PaddingLeft(20).Column(col =>
                    {
                        col.Item().Text(_ticket.MovieTitle).FontSize(20).Bold();

                        col.Item().Text($"ID biletu: {_ticket.Id}");
                        col.Item().Text($"Data: {_ticket.DateStartTime:dd MMM yyyy}");
                        col.Item().Text($"Czas: {_ticket.DateStartTime:HH:mm} - {_ticket.DateEndTime:HH:mm}");
                        col.Item().Text($"Sala: {_ticket.ScreeningRoomName}");
                        col.Item().Text($"Miejsce: Rząd {_ticket.SeatRow}, nr {_ticket.SeatNumber}");
                        col.Item().Text($"Cena: {_ticket.FinalPrice:0.00} zł");
                        col.Item().Text($"Zakupiono: {_ticket.PurchaseDate:dd MMM yyyy HH:mm}");

                        col.Item().AlignCenter().Image(GetQrCodeBytes(), ImageScaling.FitWidth);
                    });
                });

                page.Footer().AlignCenter().Text(txt =>
                {
                    txt.Span("Dziękujemy za zakup! ");
                    txt.Span("© AbsoluteCinema").Bold();
                });
            });
        }

        private string GetPosterImagePath()
        {
            if (string.IsNullOrEmpty(_ticket.MoviePosterUrl))
                return Path.Combine("wwwroot", "images", "default.jpg");

            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", _ticket.MoviePosterUrl);
            return File.Exists(filePath) ? filePath : Path.Combine("wwwroot", "images", "default.jpg");
        }

        private byte[] GetQrCodeBytes()
        {
            var qrContent = $"BiletID:{_ticket.Id}|Film:{_ticket.MovieTitle}|Data:{_ticket.DateStartTime:yyyyMMddHHmm}|Sala:{_ticket.ScreeningRoomName}|Miejsce:R{_ticket.SeatRow}S{_ticket.SeatNumber}";

            using var qrGenerator = new QRCodeGenerator();
            using var qrCodeData = qrGenerator.CreateQrCode(qrContent, QRCodeGenerator.ECCLevel.Q);
            using var qrCode = new PngByteQRCode(qrCodeData);
            return qrCode.GetGraphic(20);
        }
    }
}