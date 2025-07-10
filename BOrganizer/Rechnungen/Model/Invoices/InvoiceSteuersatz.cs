using System.Collections.Immutable;

namespace Rechnungen.Model.Invoices;

public enum InvoiceSteuersatzId
{
    Standard,
    Ermaessigt
}

public class InvoiceSteuersatz
{
    public static readonly InvoiceSteuersatz Standard = new("Standard", 0.19M, InvoiceSteuersatzId.Standard);
    public static readonly InvoiceSteuersatz Ermaessigt = new("Ermäßigt", 0.07M, InvoiceSteuersatzId.Ermaessigt);

    public static ImmutableArray<InvoiceSteuersatz> Steuersaetze => [Standard, Ermaessigt];

    public InvoiceSteuersatzId Id { get; }

    /// <summary>
    /// Beschreibung des Steuersatzes z.B. nicht ermäßigter Steuersatz und ermäßigter Steuersatz
    /// </summary>
    public string SteuerSatz { get; }

    /// <summary>Der Steuersatz in Prozent, sprich .07 oder .19;</summary>
    public decimal InProzent { get; }

    private InvoiceSteuersatz(string steuerSatz, decimal inProzent, InvoiceSteuersatzId id)
    {
        SteuerSatz = steuerSatz;
        InProzent = inProzent;
        Id = id;
    }

    public void Deconstruct(out string steuerSatz, out decimal inProzent, out InvoiceSteuersatzId id)
    {
        steuerSatz = SteuerSatz;
        inProzent = InProzent;
        id = Id;
    }

    public static InvoiceSteuersatz ById(InvoiceSteuersatzId id)
    {
        return id switch
        {
            InvoiceSteuersatzId.Standard => Standard,
            InvoiceSteuersatzId.Ermaessigt => Ermaessigt,
            _ => throw new ArgumentOutOfRangeException(nameof(id), id, null)
        };
    }

    public static InvoiceSteuersatz ById(int steuersatzId)
    {
        if (Enum.IsDefined(typeof(InvoiceSteuersatzId), steuersatzId))
        {
            return ById((InvoiceSteuersatzId)steuersatzId);
        }

        throw new ArgumentOutOfRangeException(nameof(steuersatzId), steuersatzId, null);
    }

    public override bool Equals(object? obj)
    {
        if (obj is not InvoiceSteuersatz other)
            return false;

        return SteuerSatz == other.SteuerSatz && InProzent == other.InProzent;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(SteuerSatz, InProzent);
    }

    public override string ToString()
    {
        return $"{SteuerSatz} ({InProzent:P0})";
    }
}