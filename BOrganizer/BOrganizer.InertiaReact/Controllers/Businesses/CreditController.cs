using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.General;
using Rechnungen.Services.Invoices;

namespace BOrganizer.InertiaRact.Controllers.Businesses;

[Route("Businesses/Credit")]
public class CreditController(
    IBusinessService businessService,
    IRechnungsService rechnungsService) : Controller
{
    [HttpGet("Create")]
    public IActionResult CreateGet(long? id)
    {
        return InertiaCore.Inertia.Render("Businesses/CreditForm");
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreatePost([FromForm] CreditFormDto formDto)
    {
        if (!ModelState.IsValid) { return CreateGet(formDto.Id); }

        if (formDto.Id is not null)
        {
            await rechnungsService.UpdateCreditAsync(
                formDto.Id.Value,
                formDto.Institute,
                formDto.IBAN,
                formDto.BIC,
                formDto.Inhaber);
        }
        else
        {
            Credit credit =
                await rechnungsService.CreateCreditAsync(formDto.Institute, formDto.IBAN, formDto.BIC, formDto.Inhaber);
            formDto.Id = credit.Id;
        }

        return RedirectToAction(nameof(CreateGet), new { id = formDto.Id });
    }
}

public class CreditFormDto
{
    [HiddenInput] public long? Id { get; set; }

    [Required(ErrorMessage = "Institut ist erforderlich.")]
    public string Institute { get; set; }

    [Required(ErrorMessage = "IBAN ist erforderlich.")]
    public string IBAN { get; set; }

    [Required(ErrorMessage = "BIC ist erforderlich.")]
    public string BIC { get; set; }

    [Required(ErrorMessage = "Kontoinhaber ist erforderlich.")]
    public string Inhaber { get; set; }
}