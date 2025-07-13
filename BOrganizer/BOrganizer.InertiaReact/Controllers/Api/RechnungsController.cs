using Microsoft.AspNetCore.Mvc;
using Rechnungen;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.Invoices;

namespace BOrganizer.InertiaRact.Controllers.Api;

[Route("Api/Rechnung")]
public class RechnungsController(
    IRechnungsService rechnungsService,
    IRechnungsCreator rechnungsCreator) : Controller
{
    [HttpGet("Creation/Pdf/{invoiceId:long}")]
    public async Task<IActionResult> OnPostDownload(
        long? invoiceId,
        bool download = false)
    {
        if (invoiceId is null
            || !await rechnungsService.InvoiceExistsAsync(invoiceId.Value)
            || await rechnungsService.GetInvoiceByIdAsync(invoiceId.Value) is not { } invoice) { return NotFound(); }

        Stream invoicePdfStream = rechnungsCreator.CreateRechnung(
            invoice);

        invoicePdfStream.Position = 0;

        var pdfName = $"{invoice.Rechnungsnummer}.pdf";
        string disposition = download ? "attachment" : "inline";
        Response.Headers.Append("Content-Disposition", $"{disposition}; filename={pdfName}");
        return File(invoicePdfStream, "application/pdf");
    }


    [HttpGet("Search")]
    public async Task<IActionResult> Search(string query)
    {
        if (string.IsNullOrWhiteSpace(query)) { return Ok(new List<object>()); }

        IEnumerable<RechnungsNummer> rechnungsNummern = await rechnungsService.SearchRechnungsNummernAsync(query);

        return Ok(rechnungsNummern.Select(rn => new
        {
            rn.Id,
            label = rn.ToString(),
        }));
    }
}