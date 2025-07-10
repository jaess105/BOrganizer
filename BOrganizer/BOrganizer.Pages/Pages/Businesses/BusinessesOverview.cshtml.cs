using Microsoft.AspNetCore.Mvc.RazorPages;
using Rechnungen.Model.General;
using Rechnungen.Services.General;

namespace BOrganizer.Pages.Pages.Businesses;

public class BusinessesOverview(
    IBusinessService businessService) : PageModel
{
    public List<Business> Businesses { get; set; } = [];

    public async Task OnGet()
    {
        await Load();
    }


    private async Task Load()
    {
        Businesses = [..await businessService.GetBusinessesAsync()];
    }
}