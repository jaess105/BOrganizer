using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rechnungen.Services.Invoices;

namespace BOrganizer.Pages.Pages.Businesses;

public class CreditCreation(
    IRechnungsService rechnungsService) : PageModel
{
    [BindProperty] public string Institute { get; set; }
    [BindProperty] public string IBAN { get; set; }
    [BindProperty] public string BIC { get; set; }
    [BindProperty] public string Inhaber { get; set; }

    public void OnGet()
    {
    }

    public async Task<IActionResult> OnPost()
    {
        if (!ModelState.IsValid) { return Page(); }

        await rechnungsService.CreateCreditAsync(Institute, IBAN, BIC, Inhaber);

        return RedirectToPage("/Index");
    }
}