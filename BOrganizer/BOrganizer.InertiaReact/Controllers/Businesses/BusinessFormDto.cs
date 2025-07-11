using Rechnungen.Model.General;

namespace BOrganizer.InertiaRact.Controllers.Businesses;

public class BusinessForm
{
    public long? Id { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string Plz { get; set; }
    public string Ort { get; set; }
    public string? Tel { get; set; }
    public string? EMail { get; set; }
    public string? Country { get; set; }
    public string? Steuernummer { get; set; }
    public long? PersonId { get; set; }
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public int IsUsersBusiness { get; set; }

    public bool IsUsersBusinessBool() => IsUsersBusiness > 0;
}

public static class FormExtensions
{
    // --- Person ---
    public static Person ToPerson(this BusinessForm form) => new(form.PersonId, form.FirstName, form.LastName);

    // --- Business ---
    public static Business ToBusiness(this BusinessForm form)
        => new(
            form.Id,
            form.ToPerson(),
            form.Name,
            form.Street,
            form.Number,
            form.Plz,
            form.Ort,
            form.Tel,
            form.EMail,
            form.Country,
            form.Steuernummer
        );

    public static BusinessForm ToBusinessForm(this Business business)
        => new()
        {
            Id = business.Id,
            PersonId = business.Person?.Id,
            FirstName = business.Person?.FirstName ?? "",
            LastName = business.Person?.LastName ?? "",
            Name = business.Name,
            Street = business.Street,
            Number = business.Number,
            Plz = business.Plz,
            Ort = business.Ort,
            Tel = business.Tel,
            EMail = business.EMail,
            Country = business.Country,
            Steuernummer = business.Steuernummer
        };
}