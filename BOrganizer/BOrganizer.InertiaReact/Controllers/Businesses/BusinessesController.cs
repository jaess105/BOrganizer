using Extensions.Core;
using Microsoft.AspNetCore.Mvc;
using Rechnungen.Model.General;
using Rechnungen.Services.General;

namespace BOrganizer.InertiaRact.Controllers.Businesses;

[Route("Businesses/Create")]
public class BusinessesController(IBusinessService businessService) : Controller
{
    [HttpGet("")]
    public async Task<IActionResult> CreateGet(long? businessId)
    {
        Business? business = businessId.HasValue
            ? await businessService.GetBusinessByIdAsync(businessId.Value)
            : null;

        var form = business?.ToBusinessForm();

        return InertiaCore.Inertia.Render("Businesses/BusinessForm", form);
    }

    [HttpPost("")]
    public async Task<IActionResult> CreatePost(
        [FromForm] BusinessForm? form)
    {
        if (!ModelState.IsValid) { return await CreateGet(form?.Id); }


        Person? person = null;
        if (form!.FirstName.IsNotNullOrEmpty() && form.LastName.IsNotNullOrEmpty()) { person = form.ToPerson(); }

        Business fullBusiness = form.ToBusiness() with { Person = person };

        if (fullBusiness.Id is null)
        {
            await businessService.CreateBusinessAsync(fullBusiness, person, form.IsUsersBusinessBool());
            TempData["Success"] = "Business created!";
        }
        else
        {
            await businessService.UpdateBusinessAsync(fullBusiness, person, form.IsUsersBusinessBool());
            TempData["Success"] = "Business updated!";
        }

        return RedirectToAction(nameof(CreateGet));
    }
}