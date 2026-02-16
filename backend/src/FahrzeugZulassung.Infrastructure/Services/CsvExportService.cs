using CsvHelper;
using CsvHelper.Configuration;
using System.Globalization;
using System.Text;

namespace FahrzeugZulassung.Infrastructure.Services;

public class CsvExportService
{
    public async Task<byte[]> ExportAlsCsvAsync<T>(IEnumerable<T> daten, string reportName)
    {
        using var memoryStream = new MemoryStream();
        using var streamWriter = new StreamWriter(memoryStream, new UTF8Encoding(true)); // UTF-8 mit BOM für Excel
        
        var config = new CsvConfiguration(new CultureInfo("de-DE"))
        {
            Delimiter = ";", // Semikolon für deutsche Excel-Versionen
            HasHeaderRecord = true
        };

        using var csv = new CsvWriter(streamWriter, config);
        
        await csv.WriteRecordsAsync(daten);
        await streamWriter.FlushAsync();
        
        return memoryStream.ToArray();
    }
}
