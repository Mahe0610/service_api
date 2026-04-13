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
        var ws = workbook.Worksheets.Add("Certificate");

        ws.Cell("A1").Value = "Asterix Company";
        ws.Cell("A2").Value = "Employee Certificate";
        ws.Range("A1:B1").Merge().Style.Font.SetBold().Font.SetFontSize(16);
        ws.Range("A2:B2").Merge().Style.Font.SetBold();

        ws.Cell("A4").Value = "Certificate Code";
        ws.Cell("B4").Value = record.CertificateCode;
        ws.Cell("A5").Value = "Username";
        ws.Cell("B5").Value = record.Username;
        ws.Cell("A6").Value = "Employee Name";
        ws.Cell("B6").Value = record.EmployeeName;
        ws.Cell("A7").Value = "User Type";
        ws.Cell("B7").Value = record.UserType;
        ws.Cell("A8").Value = "Age";
        ws.Cell("B8").Value = record.Age;
        ws.Cell("A9").Value = "DOB";
        ws.Cell("B9").Value = record.Dob.ToString("yyyy-MM-dd");
        ws.Cell("A10").Value = "Email";
        ws.Cell("B10").Value = record.Email;
        ws.Cell("A11").Value = "Scanner";
        ws.Cell("B11").Value = record.ScannerId;
        ws.Cell("A12").Value = "Salary";
        ws.Cell("B12").Value = record.Salary?.ToString("0.00") ?? "N/A";

        using var barcodeStream = new MemoryStream(GenerateBarcode(record.CertificateCode));
        ws.AddPicture(barcodeStream)
            .MoveTo(ws.Cell("A14"))
            .WithSize(220, 60);

        ws.Columns("A:B").AdjustToContents();

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] GeneratePdf(EmployeeRecord record)
    {
        QuestPDF.Settings.License = LicenseType.Community;
        var barcodePng = GenerateBarcode(record.CertificateCode);

        return Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.PageColor(Colors.White);
                page.Margin(30);

                page.Header().Row(row =>
                {
                    row.ConstantItem(52).Height(52)
                        .Background(Colors.Blue.Medium)
                        .AlignCenter().AlignMiddle()
                        .Text("AC").FontColor(Colors.White).Bold();
                    row.RelativeItem().PaddingLeft(12).Column(column =>
                    {
                        column.Item().Text("Asterix Company").FontSize(20).Bold();
                        column.Item().Text("Employee Certificate").FontSize(12).FontColor(Colors.Grey.Darken2);
                    });
                });

                page.Content().PaddingVertical(20).Column(column =>
                {
                    column.Spacing(8);
                    column.Item().Text($"Certificate Code: {record.CertificateCode}").Bold();
                    column.Item().Text($"Username: {record.Username}");
                    column.Item().Text($"Employee Name: {record.EmployeeName}");
                    column.Item().Text($"User Type: {record.UserType}");
                    column.Item().Text($"Age: {record.Age}");
                    column.Item().Text($"DOB: {record.Dob:yyyy-MM-dd}");
                    column.Item().Text($"Email: {record.Email}");
                    column.Item().Text($"Scanner: {record.ScannerId}");
                    column.Item().Text($"Salary: {(record.Salary.HasValue ? record.Salary.Value.ToString("0.00") : "N/A")}");
                    column.Item().PaddingTop(10).Text("Use the scanner endpoint to verify this certificate.").Italic();
                });

                page.Footer().Column(column =>
                {
                    column.Item().AlignCenter().Height(45).Image(barcodePng);
                    column.Item().AlignCenter().Text($"Generated on {DateTime.UtcNow:yyyy-MM-dd HH:mm} UTC").FontSize(9);
                });
            });
        }).GeneratePdf();
    }

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
