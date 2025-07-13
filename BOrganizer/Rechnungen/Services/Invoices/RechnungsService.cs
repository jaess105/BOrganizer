using Rechnungen.Model.General;
using Rechnungen.Model.Invoices;
using Rechnungen.Model.Payments;
using Rechnungen.Services.Payments;

namespace Rechnungen.Services.Invoices;

public interface IRechnungsNummerRepository
{
    Task<string> GetHighestNumberAsync(string kuerzel, string jahr);
    Task<RechnungsNummer> SaveWithMaxConstraintAsync(RechnungsNummer newRechnungsnummer);
    Task<RechnungsNummer?> GetByIdAsync(long rechnungsnummerId);
    Task<IEnumerable<RechnungsNummer>> SearchByNummerAsync(string query);
}

public interface ICreditRepository
{
    Task<Credit> SaveAsync(string institute, string iban, string bic, string inhaber);
    Task<IEnumerable<Credit>> GetAllAsync();
    Task<Credit?> GetByIdAsync(long creditId);
    Task<Credit> UpdateAsync(long id, string institute, string iban, string bic, string inhaber);
}

public interface IInvoiceRepository
{
    Task<Invoice> SaveAsync(Invoice invoice);
    Task<Invoice?> GetByIdAsync(long invoiceId);

    Task<Invoice> UpdateAsync(
        long invoiceId,
        Business rechnungsSteller,
        Business rechnungsEmpfaenger,
        DateOnly rechnungsDatum,
        DateOnly lieferDatum,
        InvoiceSteuersatz steuersatz,
        Credit creditInfo,
        List<Invoice.InvoiceItem> invoiceItems);

    Task<bool> ExistsAsync(long invoiceId);
    Task<IEnumerable<Invoice>> GetAllAsync();
    Task<IEnumerable<Invoice>> GetAllUnpayedAsync();
}

public interface IRechnungsService
{
    Task<RechnungsNummer> NextRechnungsNummer(string kuerzel, string jahr);

    Task<Credit> CreateCreditAsync(string institute, string iban, string bic, string inhaber);
    Task<IEnumerable<Credit>> GetCreditsAsync();
    Task<Credit?> GetCreditByIdAsync(long creditId);
    Task<Invoice> CreateInvoiceAsync(Invoice invoice);
    Task<Invoice?> GetInvoiceByIdAsync(long invoiceId);

    Task<Invoice> UpdateInvoiceAsync(long invoiceId,
        Business rechnungsSteller,
        Business rechnungsEmpfaenger,
        DateOnly rechnungsDatum,
        DateOnly lieferDatum,
        InvoiceSteuersatz steuersatz,
        Credit creditInfo,
        List<Invoice.InvoiceItem> invoiceItems);

    Task<bool> InvoiceExistsAsync(long invoiceId);
    Task<IEnumerable<Invoice>> GetInvoicesAsync();
    Task<IEnumerable<Invoice>> GetUnpayedInvoicesAsync();

    Task<Credit> UpdateCreditAsync(long id, string institute, string iban, string bic,
        string inhaber);

    Task<IEnumerable<RechnungsNummer>> SearchRechnungsNummernAsync(string s);
    Task<(IEnumerable<Invoice> invoices, Dictionary<long, Payment> payments)> GetInvoicesWithPaymentsAsync();
    Task<RechnungsNummer?> GetRechnungsNummerByIdAsync(long currentId);
}

public class RechnungsService(
    IRechnungsNummerRepository rechnungsRepo,
    ICreditRepository creditRepo,
    IInvoiceRepository invoiceRepo,
    IPaymentRepository paymentRepo) : IRechnungsService
{
    public async Task<RechnungsNummer> NextRechnungsNummer(string kuerzel, string jahr)
    {
        string highestNumber = await rechnungsRepo.GetHighestNumberAsync(kuerzel, jahr);
        if (!int.TryParse(highestNumber, out int num))
        {
            throw new ArgumentException($"{highestNumber} in the db is not a valid number!");
        }

        num++;
        string newHighestNumber = num.ToString().PadLeft(highestNumber.Length, '0');
        RechnungsNummer newRechnungsnummer = new(
            kuerzel, DateTime.Now.Year.ToString(), newHighestNumber);
        newRechnungsnummer = await rechnungsRepo.SaveWithMaxConstraintAsync(newRechnungsnummer);
        return newRechnungsnummer;
    }


    public Task<Credit> CreateCreditAsync(string institute, string iban, string bic, string inhaber)
    {
        return creditRepo.SaveAsync(institute, iban, bic, inhaber);
    }


    public Task<IEnumerable<Credit>> GetCreditsAsync()
    {
        return creditRepo.GetAllAsync();
    }

    public Task<Credit?> GetCreditByIdAsync(long creditId) => creditRepo.GetByIdAsync(creditId);

    public Task<IEnumerable<Invoice>> GetUnpayedInvoicesAsync()
    {
        return invoiceRepo.GetAllUnpayedAsync();
    }

    public Task<Credit> UpdateCreditAsync(long id, string institute, string iban, string bic, string inhaber)
    {
        return creditRepo.UpdateAsync(id, institute, iban, bic, inhaber);
    }

    public Task<IEnumerable<RechnungsNummer>> SearchRechnungsNummernAsync(string s)
    {
        return rechnungsRepo.SearchByNummerAsync(s);
    }

    public async Task<(IEnumerable<Invoice> invoices, Dictionary<long, Payment> payments)>
        GetInvoicesWithPaymentsAsync()
    {
        IEnumerable<Invoice> invoices = await invoiceRepo.GetAllAsync();
        invoices = invoices as Invoice[] ?? invoices.ToArray();
        Dictionary<long, Payment> payments = await paymentRepo.GetByInvoiceIdsAsync(invoices.Select(i => i.Id!.Value));

        return (invoices, payments);
    }

    public Task<RechnungsNummer?> GetRechnungsNummerByIdAsync(long currentId)
    {
        return rechnungsRepo.GetByIdAsync(currentId);
    }

    public Task<Invoice> CreateInvoiceAsync(Invoice invoice)
    {
        return invoiceRepo.SaveAsync(invoice);
    }

    public Task<Invoice?> GetInvoiceByIdAsync(long invoiceId)
    {
        return invoiceRepo.GetByIdAsync(invoiceId);
    }

    public Task<Invoice> UpdateInvoiceAsync(long invoiceId,
        Business rechnungsSteller,
        Business rechnungsEmpfaenger,
        DateOnly rechnungsDatum,
        DateOnly lieferDatum,
        InvoiceSteuersatz steuersatz,
        Credit creditInfo,
        List<Invoice.InvoiceItem> invoiceItems)
    {
        return invoiceRepo.UpdateAsync(
            invoiceId,
            rechnungsSteller,
            rechnungsEmpfaenger,
            rechnungsDatum,
            lieferDatum,
            steuersatz,
            creditInfo,
            invoiceItems);
    }

    public Task<bool> InvoiceExistsAsync(long invoiceId)
    {
        return invoiceRepo.ExistsAsync(invoiceId);
    }

    public Task<IEnumerable<Invoice>> GetInvoicesAsync()
    {
        return invoiceRepo.GetAllAsync();
    }
}