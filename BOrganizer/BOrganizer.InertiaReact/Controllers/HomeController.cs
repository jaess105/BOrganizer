using InertiaCore;
using Microsoft.AspNetCore.Mvc;
using Rechnungen.Model.General;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.General;
using Rechnungen.Services.Invoices;

namespace BOrganizer.InertiaRact.Controllers;

[Route("/")]
public class HomeController(
    IBusinessService businessService,
    IRechnungsService rechnungsService,
    ILogger<HomeController> logger) : Controller
{
    public async Task<Response> Index()
    {
        Business? primaryBusiness;
        List<Invoice> invoices;
        primaryBusiness = await businessService.GetPrimaryBusinessAsync();
        invoices = (await rechnungsService.GetInvoicesAsync()).ToList();

        var props = new
        {
            primaryBusiness,
            invoices = invoices.Select(invoice => new
            {
                Id = invoice.Id.ToString(),
                Rechnungsnummer = invoice.Rechnungsnummer.ToString(),
                ErstellungsDatum = invoice.ErstellungsDatum.ToString("o"), // ISO 8601
                RechnungsSteller = new { Name = invoice.RechnungsSteller.Name },
                RechnungsEmpfaenger = new { Name = invoice.RechnungsEmpfaenger.Name },
                invoice.GesamtBetrag
            }).ToList(),
        };
        return InertiaCore.Inertia.Render("App", props);
    }

    // public IActionResult Privacy()
    // {
    //     return View();
    // }
    //
    // [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    // public IActionResult Error()
    // {
    //     return View(new { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    // }
}