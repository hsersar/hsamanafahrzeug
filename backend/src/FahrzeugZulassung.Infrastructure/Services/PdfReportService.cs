using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using FahrzeugZulassung.Application.Interfaces;

namespace FahrzeugZulassung.Infrastructure.Services;

public class PdfReportService
{
    public async Task<byte[]> ExportAlsPdfAsync(ReportPdfModel model)
    {
        await Task.CompletedTask;

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4.Landscape());
                page.Margin(2, Unit.Centimetre);
                page.PageColor(Colors.White);
                page.DefaultTextStyle(x => x.FontSize(10).FontFamily("Arial"));

                page.Header()
                    .Row(row =>
                    {
                        row.RelativeItem().Column(column =>
                        {
                            column.Item().Text("🚗 KFZ-Zulassungsservice")
                                .FontSize(20)
                                .Bold()
                                .FontColor(Colors.Blue.Medium);
                            
                            column.Item().Text(model.Titel)
                                .FontSize(16)
                                .SemiBold();
                            
                            column.Item().Text(model.Zeitraum)
                                .FontSize(10)
                                .FontColor(Colors.Grey.Medium);
                        });

                        row.ConstantItem(100).AlignRight().Text(text =>
                        {
                            text.Span("Erstellt am: ").FontSize(8);
                            text.Span(DateTime.Now.ToString("dd.MM.yyyy")).FontSize(8).SemiBold();
                        });
                    });

                page.Content()
                    .PaddingVertical(1, Unit.Centimetre)
                    .Column(column =>
                    {
                        column.Item().Text("Report-Inhalt")
                            .FontSize(12)
                            .SemiBold();
                        
                        // Hier würde der eigentliche Report-Inhalt kommen
                        // Dies ist ein Platzhalter für die Implementierung
                    });

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("Seite ");
                        text.CurrentPageNumber();
                        text.Span(" von ");
                        text.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }
}
