using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace BOrganizer.Pages.Pages.Rechnungen;

public class PdfView : PageModel
{
    [BindProperty(SupportsGet = true)] public int InvoiceId { get; set; }

    public void OnGet(int invoiceId)
    {
        InvoiceId = invoiceId;
    }
}