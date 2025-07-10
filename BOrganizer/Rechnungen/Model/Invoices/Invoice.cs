using System.Collections.Immutable;
using Rechnungen.Model.General;

namespace Rechnungen.Model.Invoices;

/// <param name="RechnungsSteller">
/// Deine Geschäftsadresse 
/// </param>
/// <param name="RechnungsEmpfaenger">
///  die Adresse des Rechnungsempfängers
/// </param>
/// <param name="Steuernummer">
/// Deine Steuernummer oder deine Umsatzsteueridentifikationsnummer (notwendig für Kund*innen im EU-Ausland)
/// </param>
/// <param name="ErstellungsDatum">
/// Das Datum, an dem du die Rechnung ausgestellt hast
/// </param>
/// <param name="Rechnungsnummer">
/// Eine fortlaufende Rechnungsnummer
/// </param>
/// <param name="SteuerAusweisung">
/// Angabe des Steuersatzes und die Höhe des Steuerbetrags
/// </param>
/// <param name="Zahlungsziel">
/// Zahlungsziel
/// </param>
/// <param name="AngabeZurSteuerbefreiung">
/// Angabe, ob eine Steuerbefreiung vorliegt (z. B. Kleinunternehmerregelung)
/// </param>
public record Invoice(
    Business RechnungsSteller,
    Business RechnungsEmpfaenger,
    DateOnly ErstellungsDatum,
    DateOnly LieferDatum,
    RechnungsNummer Rechnungsnummer,
    InvoiceSteuersatz SteuerAusweisung,
    string Zahlungsziel,
    string AngabeZurSteuerbefreiung,
    ImmutableArray<Invoice.InvoiceItem> Items,
    Credit RechnungsStellerCredit,
    string? UStId = null,
    string? HRB = null,
    string? Amtsgericht = null,
    string? Geschaeftsfuehrer = null,
    string? Webseite = null
)
{
    public required long? Id { get; init; }
    public decimal GesamtBetrag => Items.Sum(i => i.Rechnungsbetrag);

    /// <param name="Beschreibung">
    /// Der Umfang der bestellten Produkte oder erbrachten Leistungen Leistungs- oder Lieferzeitpunkt
    /// </param>
    /// <param name="Rechnungsbetrag">
    /// Der Rechnungsbetrag samt Nettobetrag
    /// </param>
    public record InvoiceItem(
        string Beschreibung,
        uint Quantity,
        decimal UnitPrice
    )
    {
        public decimal? HardTotal { get; set; }
        public decimal Total => HardTotal ?? UnitPrice * Quantity;
        public decimal Rechnungsbetrag => Total;
    }
}