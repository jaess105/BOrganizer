using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rechnungen.Model.General;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.General;
using Rechnungen.Services.Invoices;

namespace BOrganizer.Pages.Pages;

public class IndexModel(
    IBusinessService businessService,
    IRechnungsService rechnungsService,
    ILogger<IndexModel> logger) : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    [BindProperty] public Business? PrimaryBusiness { get; set; }
    public List<Invoice> Invoices { get; set; }

    public async Task OnGet()
    {
        PrimaryBusiness = await businessService.GetPrimaryBusinessAsync();
        Invoices = (await rechnungsService.GetInvoicesAsync()).ToList();
    }
}