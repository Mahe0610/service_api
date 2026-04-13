using ClosedXML.Excel;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using ServiceApi.Models;
using ZXing;
using ZXing.Common;
using ZXing.SkiaSharp;

namespace ServiceApi.Services;

public class CertificateService
{
    public byte[] GenerateExcel(EmployeeRecord record)
    {
        using var workbook = new XLWorkbook();
        var ws = workbook.Worksheets.Add("EmployeeExport");

        ws.Cell("A1").Value = "User Name";
        ws.Cell("B1").Value = "Email";
        ws.Cell("C1").Value = "Salary";
        ws.Cell("D1").Value = "Barcode";
        ws.Range("A1:D1").Style.Font.SetBold().Fill.SetBackgroundColor(XLColor.LightGray);

        ws.Cell("A2").Value = record.EmployeeName;
        ws.Cell("B2").Value = record.Email;
        ws.Cell("C2").Value = record.Salary?.ToString("0.00") ?? "N/A";

        using var barcodeStream = new MemoryStream(GenerateBarcode(record.CertificateCode));
        ws.AddPicture(barcodeStream)
            .MoveTo(ws.Cell("D2"))
            .WithSize(220, 60);

        ws.Columns("A:D").AdjustToContents();
        ws.Row(2).Height = 50;

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePdf(EmployeeRecord record)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        QuestPDF.Settings.EnableDebugging = true;

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.Margin(24);

                page.Header().Text("Employee Export").Bold().FontSize(18);

                page.Content().PaddingTop(20).Column(column =>
                {
                    column.Item().Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(3);
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderCellStyle).Text("User Name");
                            header.Cell().Element(HeaderCellStyle).Text("Email");
                            header.Cell().Element(HeaderCellStyle).Text("Salary");
                            header.Cell().Element(HeaderCellStyle).Text("Barcode");
                        });

                        table.Cell().Element(BodyCellStyle).Text(record.EmployeeName);
                        table.Cell().Element(BodyCellStyle).Text(record.Email);
                        table.Cell().Element(BodyCellStyle).Text(record.Salary?.ToString("0.00") ?? "N/A");
                        table.Cell().Element(BodyCellStyle).Height(60).Image(GenerateBarcode(record.CertificateCode)).FitArea();
                    });
                });

                page.Footer().AlignRight().Text($"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(9);
            });
        }).GeneratePdf();
    }

    private static IContainer HeaderCellStyle(IContainer container) =>
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6).Background(Colors.Grey.Lighten3);

    private static IContainer BodyCellStyle(IContainer container) =>
        container.Border(1).BorderColor(Colors.Grey.Lighten2).Padding(6);

    private static byte[] GenerateBarcode(string text)
    {
        var writer = new BarcodeWriter<SkiaSharp.SKBitmap>
        {
            Format = BarcodeFormat.CODE_128,
            Options = new EncodingOptions
            {
                Height = 50,
                Width = 260,
                Margin = 2
            },
            Renderer = new SKBitmapRenderer()
        };

        using var bitmap = writer.Write(text);
        using var image = SkiaSharp.SKImage.FromBitmap(bitmap);
        using var data = image.Encode(SkiaSharp.SKEncodedImageFormat.Png, 100);
        return data.ToArray();
    }
}
