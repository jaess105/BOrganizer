namespace Rechnungen.Model.General;

public record Business(
    long? Id,
    Person? Person,
    string Name,
    string Street,
    string Number,
    string Plz,
    string Ort,
    string? Tel = null,
    string? EMail = null,
    string? Country = null,
    string? Steuernummer = null)
{
    public Business(Person? Person, string Name, string Street, string Number, string Plz, string Ort,
        string? Country = null, string? Tel = null, string? EMail = null,
        string? Steuernummer = null
    ) : this(null, Person, Name, Street, Number, Plz, Ort, Country, Tel, EMail, Steuernummer)
    {
    }

    public Business(string Name, string Street, string Number, string Plz, string Ort,
        string? Country = null, string? Tel = null, string? EMail = null,
        string? Steuernummer = null
    ) : this(null, null, Name, Street, Number, Plz, Ort, Country, Tel, EMail, Steuernummer)
    {
    }

    public string Address
    {
        get
        {
            string countryStr = Country is not null ? $"\n{Country}" : "";
            return $"{Street} {Number}\n{Plz} {Ort}{countryStr}";
        }
    }
}