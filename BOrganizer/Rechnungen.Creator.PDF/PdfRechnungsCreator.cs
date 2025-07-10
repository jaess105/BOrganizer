using Extensions.Core;
using QuestPDF;
using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using Rechnungen.Model.Invoices;

namespace Rechnungen.Creator.PDF;

public class PdfRechnungsCreator : IRechnungsCreator
{
    private static PdfRechnungsCreator? _rechnungsCreator = null;

    private PdfRechnungsCreator()
    {
    }

    public static IRechnungsCreator Init()
    {
        if (_rechnungsCreator is not null) { return _rechnungsCreator; }

        Settings.License = LicenseType.Community;
        _rechnungsCreator = new PdfRechnungsCreator();
        return _rechnungsCreator;
    }

    public Stream CreateRechnung(Invoice invoice)
    {
        var stream = new MemoryStream();
        Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(30);
                page.DefaultTextStyle(x => x.FontSize(10));

                // HEADER
                page.Header().Column(header =>
                {
                    header.Item().Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(invoice.RechnungsEmpfaenger.Name).Bold();
                            col.Item().Text(invoice.RechnungsEmpfaenger.Address);
                        });

                        row.ConstantItem(200).Column(col =>
                        {
                            // col.Item().Width(50).AlignRight().Image(Logo);

                            col.Item().PaddingTop(20).Text(invoice.RechnungsSteller.Name).AlignRight();
                            col.Item().Text(invoice.RechnungsSteller.Address).AlignRight();


                            col.Item().PaddingTop(20).Text($"Rechnungsnummer: {invoice.Rechnungsnummer}").AlignRight();
                            col.Item().Text($"Rechnungsdatum: {invoice.ErstellungsDatum:dd.MM.yyyy}")
                                .AlignRight();
                            col.Item().Text($"Lieferdatum: {invoice.LieferDatum:dd.MM.yyyy}")
                                .AlignRight();
                        });
                    });
                });

                // TABLE
                page.Content().PaddingTop(20).Column(col =>
                {
                    col.Item().PaddingTop(15).Row(row => row.RelativeItem().Text("Rechnung").FontSize(18).Bold());
                    col.Item().PaddingTop(20).Row(row =>
                        row.RelativeItem()
                            .Text(
                                $"Sehr geehrte/r {invoice.RechnungsEmpfaenger.Person?.FullName ?? invoice.RechnungsEmpfaenger.Name},"));
                    col.Item().PaddingTop(5).Row(row => row.RelativeItem().Text(
                        "vielen Dank für Ihren Auftrag. Vereinbarungsgemäß berechnen wir Ihnen hiermit folgende Leistungen:"));

                    col.Item().PaddingTop(20).Table(table =>
                    {
                        table.ColumnsDefinition(cols =>
                        {
                            cols.ConstantColumn(40); // Pos
                            cols.ConstantColumn(40); // Menge
                            cols.ConstantColumn(60); // Einheit
                            cols.RelativeColumn(3); // Beschreibung
                            cols.ConstantColumn(70); // Einzelpreis
                            cols.ConstantColumn(70); // Gesamtpreis
                        });

                        table.Header(header =>
                        {
                            header.Cell().Element(HeaderStyle).Text("Pos");
                            header.Cell().Element(HeaderStyle).Text("Anzahl");
                            header.Cell().Element(HeaderStyle).Text("Einheit");
                            header.Cell().Element(HeaderStyle).Text("Bezeichnung");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Einzelpreis");
                            header.Cell().Element(HeaderStyle).AlignRight().Text("Gesamtpreis");
                        });

                        var pos = 1;
                        foreach ((int i, Invoice.InvoiceItem item) in invoice.Items.Enumerate())
                        {
                            Func<IContainer, IContainer> bodyStyle = BodyStyle(i);
                            table.Cell().Element(bodyStyle).Padding(5).Text(pos++.ToString());
                            table.Cell().Element(bodyStyle).Padding(5).Text(item.Quantity.ToString());
                            table.Cell().Element(bodyStyle).Padding(5).Text(""); // item.Einheit);
                            table.Cell().Element(bodyStyle).Padding(5).Text(item.Beschreibung);
                            table.Cell().Element(bodyStyle).Padding(5).AlignRight().Text($"{item.UnitPrice:0.00} €");
                            table.Cell().Element(bodyStyle).Padding(5).AlignRight().Text($"{item.Total:0.00} €");
                        }

                        decimal subtotal = invoice.Items.Sum(i => i.Total);
                        decimal tax = subtotal * invoice.SteuerAusweisung.InProzent;
                        decimal total = subtotal + tax;

                        // Blank rows
                        for (var i = 0; i < 2; i++) { table.Cell().ColumnSpan(6).Text(" "); }

                        // Summary
                        table.Cell().ColumnSpan(5).AlignRight().Text("Nettopreis:");
                        table.Cell().AlignRight().Text($"{subtotal:0.00} €");

                        table.Cell().ColumnSpan(5).AlignRight()
                            .Text($"Zzgl. {invoice.SteuerAusweisung.InProzent * 100:0}% USt:");
                        table.Cell().AlignRight().Text($"{tax:0.00} €");

                        table.Cell().ColumnSpan(5).AlignRight().Text("Rechnungsbetrag:").Bold();
                        table.Cell().AlignRight().Text($"{total:0.00} €").Bold();
                    });
                });

                // FOOTER
                page.Footer().Column(footer =>
                {
                    footer.Item().PaddingTop(20)
                        .Text(
                            "Bitte überweisen Sie den Rechnungsbetrag innerhalb von 14 Tagen auf unser unten genanntes Konto.");
                    footer.Item().Text("Für weitere Fragen stehen wir Ihnen gerne zur Verfügung.");

                    footer.Item().PaddingTop(15).Text("Mit freundlichen Grüßen");
                    footer.Item().Text(invoice.RechnungsSteller.Person?.FullName
                                       ?? invoice.RechnungsSteller.Name);

                    footer.Item().PaddingTop(20).Row(row =>
                    {
                        row.RelativeItem().Column(col =>
                        {
                            col.Item().Text(invoice.RechnungsSteller.Name);
                            col.Item().Text(invoice.RechnungsSteller.Address);
                            col.Item().Text($"Tel: {invoice.RechnungsSteller.Tel}");
                            col.Item().Text($"E-mail: {invoice.RechnungsSteller.EMail}");
                        });

                        if (invoice.RechnungsStellerCredit is { } credit)
                        {
                            row.RelativeItem().Column(col =>
                            {
                                col.Item().Text($"Kreditinstitut: {credit.Institute}");
                                col.Item().Text($"IBAN: {credit.IbanNotation()}");
                                col.Item().Text($"BIC: {credit.BIC}");
                                col.Item().Text($"Kto. Inh.: {credit.Inhaber}");
                            });
                        }

                        row.RelativeItem().Column(col =>
                        {
                            if (invoice.UStId is { } uStId) { col.Item().Text($"USt-ID: {uStId}"); }

                            if (invoice.HRB is { } hrb) { col.Item().Text($"HRB: {hrb}"); }

                            if (invoice.Amtsgericht is { } amtsgericht)
                            {
                                col.Item().Text($"Amtsgericht: {amtsgericht}");
                            }

                            if (invoice.Geschaeftsfuehrer is { } geschaeftsfuehrer)
                            {
                                col.Item().Text($"Geschäftsführer: {geschaeftsfuehrer}");
                            }

                            if (invoice.Webseite is { } webseite) { col.Item().Text($"Webseite: {webseite}"); }
                        });
                    });
                });
            });
        }).GeneratePdf(stream);

        return stream;
    }

    static IContainer HeaderStyle(IContainer container)
    {
        // Colors.Grey.Lighten2;
        return container
            .Background(Colors.Blue.Darken2)
            .DefaultTextStyle(x => x.FontColor(Colors.White).Bold())
            .PaddingVertical(4)
            .PaddingHorizontal(4);
    }

    static Func<IContainer, IContainer> BodyStyle(int i)
    {
        Color color = i % 2 == 0
            ? Colors.Blue.Lighten5
            : Colors.Blue.Lighten4;

        return container => container.Background(color);
    }
}