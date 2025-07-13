using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc;
using Rechnungen.Model.General;
using Rechnungen.Model.Payments;
using Rechnungen.Services.General;
using Rechnungen.Services.Invoices;
using Rechnungen.Services.Payments;

namespace BOrganizer.InertiaRact.Controllers.Payments;

[Route("Payments")]
public class PaymentsController(
    IBusinessService businessService,
    IPaymentService paymentService,
    IRechnungsService rechnungsService) : Controller
{
    [HttpGet("Create")]
    public async Task<IActionResult> CreateGet(long? paymentId = null)
    {
        PaymentDto paymentDto = null;
        if (paymentId is not null && await paymentService.GetPaymentByIdAsync(paymentId.Value) is { } payment)
        {
            paymentDto = payment.ToDto();
        }

        return InertiaCore.Inertia.Render("Payments/PaymentForm", new
        {
            payment = paymentDto
        });
    }

    [HttpGet("Create/FromInvoice/{invoiceId:long}")]
    public async Task<IActionResult> CreateFromInvoiceGet(long invoiceId)
    {
        if (await rechnungsService.GetInvoiceByIdAsync(invoiceId) is not { } invoice) { return NotFound(); }

        Payment? payment = await paymentService.GetPaymentByInvoiceIdAsync(invoiceId);
        if (payment is null)
        {
            payment = paymentService.PaymentFromRechnung(invoice);
            payment = await paymentService.SavePaymentAsync(payment);
        }


        return RedirectToAction(nameof(CreateGet), new { paymentId = payment.Id });
    }

    [HttpPost("Create")]
    public async Task<IActionResult> CreatePost([FromForm] PaymentDto paymentDto)
    {
        if (!ModelState.IsValid || paymentDto.ToDomain() is not { } payment) { return await CreateGet(); }

        Payment saved = await paymentService.SavePaymentAsync(payment);
        return RedirectToAction(nameof(CreateGet));
    }

    [HttpGet("")]
    public async Task<IActionResult> Index()
    {
        Business? primaryBusiness = await businessService.GetPrimaryBusinessAsync()
#if DEBUG
                                    ?? await businessService.GetBusinessByIdAsync(1)
#endif
            ;
        IEnumerable<Payment> payments = await paymentService.GetPaymentsAsync();
        return InertiaCore.Inertia.Render("Payments/PaymentsIndex",
            new
            {
                payments = payments.Select(PaymentDtoConverions.ToDto),
                primaryBusiness,
            });
    }

    [HttpDelete("Delete/{paymentId:long}")]
    public async Task<IActionResult> Delete(long paymentId)
    {
        await paymentService.DeletePaymentAsync(paymentId);
        return RedirectToAction(nameof(Index));
    }
}

public class PaymentDto
{
    [HiddenInput] public long? paymentId { get; set; }

    [Required] public DateOnly date { get; set; }
    [Required] public decimal netto { get; set; }
    [Required] public decimal mwstPercent { get; set; }
    [Required] public decimal mwstEuro { get; set; }
    [Required] public decimal brutto { get; set; }

    [Required] public string sender { get; set; }
    [HiddenInput] public long? senderId { get; set; }

    [Required] public string receiver { get; set; }
    [HiddenInput] public long? receiverId { get; set; }

    [Required] public string product { get; set; }
    [Required] public string currency { get; set; }
    [Required] public string method { get; set; }

    public string? notes { get; set; }
    [HiddenInput] public long? rechnungId { get; set; }
}

internal static class PaymentDtoConverions
{
    internal static PaymentDto ToDto(this Payment self)
        => new()
        {
            paymentId = self.Id,
            date = self.Date,
            mwstPercent = self.MwstPercent,
            netto = self.Netto,
            brutto = self.Brutto,
            mwstEuro = self.MwstTotal,
            sender = self.Sender,
            receiver = self.Receiver,
            product = self.Product,
            currency = self.Currency,
            method = self.Method!,
            notes = self.Notes,
            rechnungId = self.RechnungId,
            receiverId = self.ReceiverId,
            senderId = self.SenderId,
        };

    internal static Payment ToDomain(this PaymentDto self)
        => new(
            self.date,
            self.mwstPercent,
            self.netto,
            self.brutto,
            self.mwstEuro,
            self.sender,
            self.receiver,
            self.product,
            self.currency,
            self.method,
            self.notes,
            null,
            null,
            self.rechnungId
        )
        {
            Id = self.paymentId,
        };
}