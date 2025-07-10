using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rechnungen.Model.General;
using Rechnungen.Services.General;

namespace BOrganizer.Pages.Pages.Businesses;

public class BusinessCreation(IBusinessService businessService) : PageModel
{
    [BindProperty(SupportsGet = true)] public long? BusinessId { get; set; }
    [BindProperty] public string? FirstName { get; set; }

    [BindProperty] public string? LastName { get; set; }

    [BindProperty] public BusinessForm? Business { get; set; }

    [BindProperty] public bool IsUsersBusiness { get; set; } = false;


    public async Task<IActionResult> OnGet()
    {
        if (BusinessId is null) { return Page(); }

        Business? business = await businessService.GetBusinessByIdAsync(BusinessId.Value);
        if (business is null) { return NotFound(); }

        Business = business.ToBusinessForm();

        if (Business.Person is not null)
        {
            FirstName = Business.Person.FirstName;
            LastName = Business.Person.LastName;
        }

        return Page();
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) { return Page(); }

        // Combine person info if provided
        Person? person = null;
        if (!string.IsNullOrWhiteSpace(FirstName) && !string.IsNullOrWhiteSpace(LastName))
        {
            person = new Person(FirstName.Trim(), LastName.Trim());
        }

        Business fullBusiness = Business.ToBusiness() with { Person = person };

        if (Business.Id is null)
        {
            await businessService.CreateBusinessAsync(fullBusiness, person, IsUsersBusiness);
            TempData["Success"] = "Business created!";
        }
        else
        {
            await businessService.UpdateBusinessAsync(fullBusiness, person, IsUsersBusiness);
            TempData["Success"] = "Business updated!";
        }

        return RedirectToPage("/Index");
    }
}

public class BusinessForm
{
    public long? Id { get; set; }
    public PersonForm? Person { get; set; }
    public string Name { get; set; }
    public string Street { get; set; }
    public string Number { get; set; }
    public string Plz { get; set; }
    public string Ort { get; set; }
    public string? Tel { get; set; }
    public string? EMail { get; set; }
    public string? Country { get; set; }
    public string? Steuernummer { get; set; }


    public class PersonForm
    {
        public long? Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
    }
}

public static class FormExtensions
{
    // --- Person ---
    public static Person ToPerson(this BusinessForm.PersonForm form) => new(form.Id, form.FirstName, form.LastName);

    public static BusinessForm.PersonForm ToPersonForm(this Person person)
        => new()
        {
            Id = person.Id,
            FirstName = person.FirstName,
            LastName = person.LastName
        };

    // --- Business ---
    public static Business ToBusiness(this BusinessForm form)
        => new(
            form.Id,
            form.Person?.ToPerson(),
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
            Person = business.Person?.ToPersonForm(),
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