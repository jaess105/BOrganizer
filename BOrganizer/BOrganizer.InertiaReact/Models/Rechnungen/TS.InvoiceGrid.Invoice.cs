using Rechnungen.Model.Invoices;

namespace BOrganizer.Inertia.Models.Rechnungen;

public class TsInvoiceGridInvoice
{
    public string Id { get; set; }
    public RechnungsNummer Rechnungsnummer { get; set; }
    public string ErstellungsDatum { get; set; }
    public TsBusiness RechnungsSteller { get; set; }
    public TsBusiness RechnungsEmpfaenger { get; set; }
    public decimal GesamtBetrag { get; set; }


    public static TsInvoiceGridInvoice FromInvoice(Invoice invoice)
    {
        return new()
        {
            Id = invoice.Id!.ToString(),
            Rechnungsnummer = invoice.Rechnungsnummer,
            ErstellungsDatum = invoice.ErstellungsDatum.ToString("o"), // ISO 8601
            RechnungsSteller = new TsBusiness { Name = invoice.RechnungsSteller.Name },
            RechnungsEmpfaenger = new TsBusiness { Name = invoice.RechnungsEmpfaenger.Name },
            GesamtBetrag = invoice.GesamtBrutto
        };
    }
}