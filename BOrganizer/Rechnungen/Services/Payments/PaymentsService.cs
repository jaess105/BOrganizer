using Rechnungen.Model.Invoices;
using Rechnungen.Model.Payments;
using Rechnungen.Services.Invoices;

namespace Rechnungen.Services.Payments;

public interface IPaymentRepository
{
    Task<Payment> SaveAsync(Payment payment);
    Task<IEnumerable<Payment>> GetAllAsync();
    Task<Payment?> GetByIdAsync(long id);
    Task DeleteAsync(long id);
    Task<Payment?> GetByInvoiceIdAsync(long invoiceId);
}

public interface IPaymentService
{
    Payment PaymentFromRechnung(Invoice invoice);
    Task<Payment> CreatePaymentAsync(Payment payment);
    Task<Payment> SavePaymentAsync(Payment payment);
    Task<Payment> UpdatePaymentAsync(Payment payment);
    Task<Payment?> GetPaymentByIdAsync(long id);
    Task<IEnumerable<Payment>> GetPaymentsAsync();
    Task DeletePaymentAsync(long paymentId);
    Task<Payment?> GetPaymentByInvoiceIdAsync(long invoiceId);
}

public class PaymentService(
    IPaymentRepository paymentRepo,
    IInvoiceRepository invoiceRepo) : IPaymentService
{
    public Payment PaymentFromRechnung(Invoice invoice)
    {
        decimal mwstPercent = invoice.SteuerAusweisung.InProzent;
        decimal netto = invoice.GesamtNetto;
        decimal brutto = invoice.GesamtBrutto;
        decimal mwstTotal = brutto - netto;

        return new(
            Date: invoice.ErstellungsDatum,
            MwstPercent: mwstPercent,
            Netto: netto,
            Brutto: brutto,
            MwstTotal: mwstTotal,
            Sender: invoice.RechnungsEmpfaenger.Name,
            Receiver: invoice.RechnungsSteller.Name,
            Product: "Zahlung für Rechnung",
            Currency: "EUR",
            Method: "BankTransfer",
            Notes: $"Automatisch generiert aus Rechnung {invoice.Rechnungsnummer}",
            SenderId: invoice.RechnungsEmpfaenger.Id,
            ReceiverId: invoice.RechnungsSteller.Id,
            RechnungId: invoice.Id
        ) { Id = null };
    }

    public Task<Payment> CreatePaymentAsync(Payment payment)
    {
        return SavePaymentAsync(payment);
    }

    public Task<Payment> SavePaymentAsync(Payment payment)
    {
        return paymentRepo.SaveAsync(payment);
    }

    public Task<Payment> UpdatePaymentAsync(Payment payment)
    {
        return paymentRepo.SaveAsync(payment);
    }

    public Task<Payment?> GetPaymentByIdAsync(long id)
    {
        return paymentRepo.GetByIdAsync(id);
    }

    public Task<IEnumerable<Payment>> GetPaymentsAsync()
    {
        return paymentRepo.GetAllAsync();
    }

    public Task DeletePaymentAsync(long paymentId)
    {
        return paymentRepo.DeleteAsync(paymentId);
    }

    public Task<Payment?> GetPaymentByInvoiceIdAsync(long invoiceId)
    {
       return paymentRepo.GetByInvoiceIdAsync(invoiceId); 
    }
}