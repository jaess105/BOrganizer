using Microsoft.AspNetCore.Mvc;
using Rechnungen;
using Rechnungen.Services.Invoices;

namespace BOrganizer.InertiaRact.Controllers.Api;

[Route("Api/Rechnung/Creation/Pdf")]
public class RechnungsController(
    IRechnungsService rechnungsService,
    IRechnungsCreator rechnungsCreator) : Controller
{
    [HttpGet("{invoiceId:long}")]
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
}