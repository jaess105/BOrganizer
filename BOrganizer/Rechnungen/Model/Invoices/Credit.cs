using System.Text;

namespace Rechnungen.Model.Invoices;

public record Credit(
    long? Id,
    string Institute,
    string IBAN,
    string BIC,
    string Inhaber)
{
    public Credit(string Institute, string IBAN, string BIC, string Inhaber)
        : this(null, Institute, IBAN, BIC, Inhaber)
    {
    }

    public string Short => $"{Institute}: {IBAN}";

    public string IbanNotation()
    {
        string together = string.Join("", IBAN.Split(" "));
        return string.Join(" ",
            Enumerable.Range(0, together.Length)
                .Select(i => together[i..Math.Min(i + 4, together.Length)]));
    }
}