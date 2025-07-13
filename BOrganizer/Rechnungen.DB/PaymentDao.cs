using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using DB.Core;
using Extensions.Core;
using Npgsql;
using Rechnungen.DB.Extensions;
using Rechnungen.Model.Payments;
using Rechnungen.Services.Payments;
using RepoDb;
using Rechnungen.Extensions;

namespace Rechnungen.DB;

internal class PaymentRepoDb(string connectionString)
    : BaseRepository<PaymentDto, NpgsqlConnection>(connectionString);

public class PaymentDao(
    string connectionString,
    IDbConnectionFactory connectionFactory) : IPaymentRepository
{
    private readonly PaymentRepoDb _repo = new(connectionString);

    public async Task<Payment> SaveAsync(Payment payment)
    {
        using IDbConnection conn = connectionFactory.CreateOpenConnection();
        PaymentDto dto = PaymentDto.FromModel(payment);

        if (payment.Id is null) { dto.payment_id = await conn.InsertAsync<PaymentDto, long>(dto); }
        else { await conn.UpdateAsync(dto); }

        return dto.ToModel();
    }

    public async Task<IEnumerable<Payment>> GetAllAsync()
    {
        IEnumerable<PaymentDto>? dtos = await _repo.QueryAllAsync();
        return dtos.Select(p => p.ToModel());
    }

    public async Task<Payment?> GetByIdAsync(long id)
    {
        IEnumerable<PaymentDto>? dto = await _repo.QueryAsync(p => p.payment_id == id, top: 1);
        return dto.FirstOrDefault()?.ToModel();
    }

    public async Task DeleteAsync(long id)
    {
        await _repo.DeleteAsync(id);
    }

    public async Task<Payment?> GetByInvoiceIdAsync(long invoiceId)
    {
        return (await _repo.QueryAsync(
            p => p.rechnung_id == invoiceId,
            top: 1)).FirstOrDefault()?.ToModel();
    }
}

[Table("payments")]
public class PaymentDto
{
    public long payment_id { get; set; }
    public DateTime date { get; set; }
    public decimal mwst_percent { get; set; }
    public decimal netto { get; set; }
    public decimal brutto { get; set; }
    public decimal mwst_total { get; set; }
    public string sender { get; set; } = default!;
    public string receiver { get; set; } = default!;
    public string product { get; set; } = default!;
    public string currency { get; set; } = default!;
    public string? method { get; set; }
    public string? notes { get; set; }
    public long? sender_id { get; set; }
    public long? receiver_id { get; set; }
    public long? rechnung_id { get; set; }

    public Payment ToModel() => new(
        Date: date.ToDateOnly(),
        MwstPercent: mwst_percent,
        Netto: netto,
        Brutto: brutto,
        MwstTotal: mwst_total,
        Sender: sender,
        Receiver: receiver,
        Product: product,
        Currency: currency,
        Method: method,
        Notes: notes,
        SenderId: sender_id,
        ReceiverId: receiver_id,
        RechnungId: rechnung_id
    ) { Id = payment_id };

    public static PaymentDto FromModel(Payment payment) => new()
    {
        payment_id = payment.Id ?? -1,
        date = payment.Date.ToDateTime(),
        mwst_percent = payment.MwstPercent,
        netto = payment.Netto,
        brutto = payment.Brutto,
        mwst_total = payment.MwstTotal,
        sender = payment.Sender,
        receiver = payment.Receiver,
        product = payment.Product,
        currency = payment.Currency,
        method = payment.Method,
        notes = payment.Notes,
        sender_id = payment.SenderId,
        receiver_id = payment.ReceiverId,
        rechnung_id = payment.RechnungId
    };
}