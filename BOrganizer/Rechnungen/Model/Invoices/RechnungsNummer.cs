namespace Rechnungen.Model.Invoices;

public record RechnungsNummer(
    long? Id,
    string Kuerzel,
    string Jahr,
    string Nummer)
{
    public RechnungsNummer(
        string Kuerzel,
        string Jahr,
        string Nummer) : this(null, Kuerzel, Jahr, Nummer)
    {
    }

    public override string ToString() => $"{Kuerzel}-{Jahr}-{Nummer}";
}