using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Rechnungen.Model.General;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.General;
using Rechnungen.Services.Invoices;

namespace BOrganizer.Pages.Pages.Rechnungen;

public class RechnungCreation(
    IBusinessService businessService,
    IRechnungsService rechnungsService) : PageModel
{
    // For edit
    [BindProperty(SupportsGet = true)] public long? InvoiceId { get; set; }

    [Required] [BindProperty] public long? RechnungsStellerId { get; set; }
    [Required] [BindProperty] public long? RechnungsEmpfaengerId { get; set; }
    [Required] [BindProperty] public DateOnly RechnungsDatum { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    [Required] [BindProperty] public DateOnly LieferDatum { get; set; } = DateOnly.FromDateTime(DateTime.Now);
    [Required] [BindProperty] public InvoiceSteuersatzId? SteuersatzId { get; set; } = InvoiceSteuersatzId.Standard;
    [Required] [BindProperty] public long? CreditId { get; set; }
    [BindProperty] public List<Invoice.InvoiceItem> InvoiceItems { get; set; } = [];
    public RechnungsNummer? RechnungsNummer { get; set; }

    public List<Business> BusinessOptions { get; set; } = [];
    public List<InvoiceSteuersatz> SteuersatzOptions { get; set; } = [];
    public List<Credit> CreditOptions { get; set; } = [];

    public async Task OnGetAsync()
    {
        await LoadOptions();

        if (InvoiceId is not null)
        {
            // Load existing invoice for editing
            if (await rechnungsService.GetInvoiceByIdAsync(InvoiceId.Value) is not { } invoice) { return; }

            RechnungsStellerId = invoice.RechnungsSteller.Id;
            RechnungsEmpfaengerId = invoice.RechnungsEmpfaenger.Id;
            RechnungsDatum = invoice.ErstellungsDatum;
            LieferDatum = invoice.LieferDatum;
            SteuersatzId = invoice.SteuerAusweisung.Id;
            CreditId = invoice.RechnungsStellerCredit.Id;
            InvoiceItems = invoice.Items.ToList();
            RechnungsNummer = invoice.Rechnungsnummer;
        }
    }


    public async Task<IActionResult> OnPostAsync()
    {
        const string steuernummer = "mep";
        if (!ModelState.IsValid)
        {
            await LoadOptions();
            return Page();
        }

        Business? rechnungsSteller = await businessService.GetBusinessByIdAsync(RechnungsStellerId!.Value);
        Business? rechnungsEmpfaenger = await businessService.GetBusinessByIdAsync(RechnungsEmpfaengerId!.Value);
        Credit? creditInfo = await rechnungsService.GetCreditByIdAsync(CreditId!.Value);
        InvoiceSteuersatz steuersatz = InvoiceSteuersatz.ById(SteuersatzId!.Value);

        if (InvoiceId is null)
        {
            RechnungsNummer rechnungsnummer = await rechnungsService.NextRechnungsNummer(
                "PzE", DateTime.Now.Year.ToString());

            var invoice = new Invoice(
                rechnungsSteller,
                rechnungsEmpfaenger,
                RechnungsDatum,
                LieferDatum,
                rechnungsnummer,
                steuersatz,
                creditInfo.Short,
                "Ne du",
                [..InvoiceItems],
                creditInfo) { Id = null };

            await rechnungsService.CreateInvoiceAsync(invoice);
        }
        else
        {
            if (!await rechnungsService.InvoiceExistsAsync(InvoiceId.Value)) { return NotFound(); }

            await rechnungsService.UpdateInvoiceAsync(
                InvoiceId.Value,
                rechnungsSteller,
                rechnungsEmpfaenger,
                RechnungsDatum,
                LieferDatum,
                steuersatz,
                creditInfo,
                InvoiceItems);
        }

        return RedirectToPage("/Index");
    }


    private async Task LoadOptions()
    {
        BusinessOptions = [..await businessService.GetBusinessesAsync()];
        SteuersatzOptions = [..InvoiceSteuersatz.Steuersaetze];
        CreditOptions = [..await rechnungsService.GetCreditsAsync()];
    }
}