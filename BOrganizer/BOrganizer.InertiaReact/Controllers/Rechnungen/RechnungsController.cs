using System.Collections.Immutable;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Rechnungen.Model.General;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.General;
using Rechnungen.Services.Invoices;

namespace BOrganizer.InertiaRact.Controllers.Rechnungen;

[Route("Rechnungen")]
public class RechnungsController(
    IBusinessService businessService,
    IRechnungsService rechnungsService
) : Controller
{
    [HttpGet("Create")]
    public async Task<IActionResult> CreateGet(long? invoiceId)
    {
        IEnumerable<Business> businesses = await businessService.GetBusinessesAsync();
        IEnumerable<Credit> credits = await rechnungsService.GetCreditsAsync();
        ImmutableArray<InvoiceSteuersatz> steuersaetze = InvoiceSteuersatz.Steuersaetze;

        InvoiceDto? invoiceDto = null;
        if (invoiceId is not null)
        {
            // Load existing invoice for editing
            if (await rechnungsService.GetInvoiceByIdAsync(invoiceId.Value) is { } invoice)
            {
                invoiceDto = invoice.ToInvoiceDto();
            }
            //
            // RechnungsStellerId = invoice.RechnungsSteller.Id;
            // RechnungsEmpfaengerId = invoice.RechnungsEmpfaenger.Id;
            // RechnungsDatum = invoice.ErstellungsDatum;
            // LieferDatum = invoice.LieferDatum;
            // SteuersatzId = invoice.SteuerAusweisung.Id;
            // CreditId = invoice.RechnungsStellerCredit.Id;
            // InvoiceItems = invoice.Items.ToList();
            // RechnungsNummer = invoice.Rechnungsnummer;
        }

        return InertiaCore.Inertia.Render("Rechnungen/RechnungsForm", new
        {
            businesses,
            credits,
            steuersaetze,
            invoiceDto
        });
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreatePost([FromForm] InvoiceDto dto)
    {
        if (!ModelState.IsValid) { return await CreateGet(null); }

        Business? rechnungsSteller = await businessService.GetBusinessByIdAsync(dto.RechnungsStellerId);
        Business? rechnungsEmpfaenger = await businessService.GetBusinessByIdAsync(dto.RechnungsEmpfaengerId);
        Credit? creditInfo = await rechnungsService.GetCreditByIdAsync(dto.CreditId);
        InvoiceSteuersatz steuersatz = InvoiceSteuersatz.ById(dto.SteuersatzId);

        DateOnly rechnungsDatum = DateOnly.FromDateTime(dto.RechnungsDatum);
        DateOnly lieferDatum = DateOnly.FromDateTime(dto.LieferDatum);

        IEnumerable<Invoice.InvoiceItem> invoiceItems =
            dto.InvoiceItems.Select(i => new Invoice.InvoiceItem(i.Beschreibung, i.Quantity, i.UnitPrice));

        if (dto.InvoiceId is null)
        {
            RechnungsNummer rechnungsnummer = await rechnungsService.NextRechnungsNummer(
                "PzE", DateTime.Now.Year.ToString());

            var invoice = new Invoice(
                rechnungsSteller,
                rechnungsEmpfaenger,
                rechnungsDatum,
                lieferDatum,
                rechnungsnummer,
                steuersatz,
                creditInfo!.Short,
                "Ne du",
                [..invoiceItems],
                creditInfo) { Id = null };

            await rechnungsService.CreateInvoiceAsync(invoice);
        }
        else
        {
            if (!await rechnungsService.InvoiceExistsAsync(dto.InvoiceId.Value)) { return NotFound(); }

            await rechnungsService.UpdateInvoiceAsync(
                dto.InvoiceId.Value,
                rechnungsSteller,
                rechnungsEmpfaenger,
                rechnungsDatum,
                lieferDatum,
                steuersatz,
                creditInfo,
                [..invoiceItems]);
        }


        return RedirectToAction(nameof(CreateGet));
    }
}

public class InvoiceDto
{
    [HiddenInput] public long? InvoiceId { get; set; }
    public string? RechnungsNummer { get; set; }

    [Required] public long RechnungsStellerId { get; set; }
    [Required] public long RechnungsEmpfaengerId { get; set; }
    [Required] public DateTime RechnungsDatum { get; set; } = DateTime.Now;
    [Required] public DateTime LieferDatum { get; set; } = DateTime.Now;
    [Required] public InvoiceSteuersatzId SteuersatzId { get; set; } = InvoiceSteuersatzId.Standard;
    [Required] public long CreditId { get; set; }
    [Required] public List<InvoiceItemDto> InvoiceItems { get; set; } = [];


    public class InvoiceItemDto
    {
        [HiddenInput] public long? Id { get; set; }
        [Required] public string Beschreibung { get; set; }
        [Required] public uint Quantity { get; set; }
        [Required] public decimal UnitPrice { get; set; }
    }
}

public static class InvoiceDtoExtensions
{
    public static InvoiceDto ToInvoiceDto(this Invoice invoice)
        => new()
        {
            InvoiceId = invoice.Id,
            RechnungsNummer = invoice.Rechnungsnummer.ToString(),
            RechnungsStellerId = invoice.RechnungsSteller.Id.Value,
            RechnungsEmpfaengerId = invoice.RechnungsEmpfaenger.Id.Value,
            RechnungsDatum = invoice.ErstellungsDatum.ToDateTime(TimeOnly.MinValue),
            LieferDatum = invoice.LieferDatum.ToDateTime(TimeOnly.MinValue),
            SteuersatzId = invoice.SteuerAusweisung.Id,
            CreditId = invoice.RechnungsStellerCredit.Id.Value,
            InvoiceItems = invoice.Items.Select(ToInvoiceItemDto).ToList(),
        };

    public static InvoiceDto.InvoiceItemDto ToInvoiceItemDto(this Invoice.InvoiceItem invoice)
        => new()
        {
            Beschreibung = invoice.Beschreibung,
            Quantity = invoice.Quantity,
            UnitPrice = invoice.UnitPrice,
        };
}