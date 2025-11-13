using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;

namespace GqeberhaPharmacy.Services
{
    public class PdfService
    {
        public byte[] GenerateStockTakePdf(string pharmacyName, List<(string MedicationName, string DosageForm, int QuantityOnHand, int Schedule, string Supplier)> medications, string groupBy = "dosage")
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.Content().Column(column =>
                    {
                        column.Item().Text(text =>
                        {
                            text.AlignCenter();
                            text.Span($"{pharmacyName} - Stock Take Report").Bold().FontSize(16);
                        });

                        column.Item().PaddingVertical(10).Text($"Generated: {DateTime.Now:dd/MM/yyyy HH:mm}").FontSize(10);

                        column.Item().PaddingVertical(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Medication").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Dosage Form").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Qty on Hand").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Schedule").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Supplier").FontColor(Colors.White);
                            });

                            foreach (var med in medications)
                            {
                                table.Cell().Padding(5).Text(med.MedicationName);
                                table.Cell().Padding(5).Text(med.DosageForm);
                                table.Cell().Padding(5).Text(med.QuantityOnHand.ToString());
                                table.Cell().Padding(5).Text(med.Schedule.ToString());
                                table.Cell().Padding(5).Text(med.Supplier);
                            }
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GenerateCustomerReportPdf(string customerName, List<(DateTime Date, string Medication, int Quantity, decimal Price)> dispensedItems)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.Content().Column(column =>
                    {
                        column.Item().Text(text =>
                        {
                            text.AlignCenter();
                            text.Span($"Dispensed Prescriptions Report - {customerName}").Bold().FontSize(16);
                        });

                        column.Item().PaddingVertical(10).Text($"Report Period: {DateTime.Now.AddMonths(-1):dd/MM/yyyy} to {DateTime.Now:dd/MM/yyyy}").FontSize(10);

                        column.Item().PaddingVertical(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Date").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Medication").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Quantity").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Price").FontColor(Colors.White);
                            });

                            foreach (var item in dispensedItems)
                            {
                                table.Cell().Padding(5).Text(item.Date.ToString("dd/MM/yyyy"));
                                table.Cell().Padding(5).Text(item.Medication);
                                table.Cell().Padding(5).Text(item.Quantity.ToString());
                                table.Cell().Padding(5).Text($"R{item.Price:F2}");
                            }
                        });

                        var total = dispensedItems.Sum(x => x.Price);
                        column.Item().PaddingTop(20).AlignRight().Text($"Total Amount: R{total:F2}").Bold();
                    });
                });
            });

            return document.GeneratePdf();
        }

        public byte[] GeneratePharmacistReportPdf(string pharmacistName, List<(DateTime Date, string Medication, int Quantity, string Schedule)> dispensedItems)
        {
            QuestPDF.Settings.License = LicenseType.Community;

            var document = Document.Create(container =>
            {
                container.Page(page =>
                {
                    page.Size(PageSizes.A4);
                    page.Margin(20);
                    page.Content().Column(column =>
                    {
                        column.Item().Text(text =>
                        {
                            text.AlignCenter();
                            text.Span($"Dispense Report - {pharmacistName}").Bold().FontSize(16);
                        });

                        column.Item().PaddingVertical(10).Text($"Report Period: {DateTime.Now.AddMonths(-1):dd/MM/yyyy} to {DateTime.Now:dd/MM/yyyy}").FontSize(10);

                        column.Item().PaddingVertical(20).Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(3);
                                columns.RelativeColumn(2);
                                columns.RelativeColumn(2);
                            });

                            table.Header(header =>
                            {
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Date").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Medication").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Quantity").FontColor(Colors.White);
                                header.Cell().Background(Colors.Grey.Darken1).Padding(5).Text("Schedule").FontColor(Colors.White);
                            });

                            foreach (var item in dispensedItems)
                            {
                                table.Cell().Padding(5).Text(item.Date.ToString("dd/MM/yyyy"));
                                table.Cell().Padding(5).Text(item.Medication);
                                table.Cell().Padding(5).Text(item.Quantity.ToString());
                                table.Cell().Padding(5).Text(item.Schedule);
                            }
                        });
                    });
                });
            });

            return document.GeneratePdf();
        }
    }
}
