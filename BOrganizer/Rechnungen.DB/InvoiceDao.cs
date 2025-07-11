using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using DB.Core;
using Npgsql;
using Rechnungen.DB.Extensions;
using Rechnungen.Model.General;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.General;
using Rechnungen.Services.Invoices;
using RepoDb;

namespace Rechnungen.DB;

internal class InvoiceRepository(string connectionString) :
    BaseRepository<InvoiceDto, NpgsqlConnection>(connectionString);

internal class InvoiceItemRepository(string connectionString) :
    BaseRepository<InvoiceItemDto, NpgsqlConnection>(connectionString);

public class InvoiceDao(
    string connectionString,
    IBusinessRepository businessRepo,
    ICreditRepository creditRepo,
    IRechnungsNummerRepository rechnungsNummerRepo,
    IDbConnectionFactory connectionFactory) : IInvoiceRepository
{
    private readonly InvoiceRepository _repo = new(connectionString);
    private readonly InvoiceItemRepository _repoItem = new(connectionString);

    public async Task<Invoice> SaveAsync(Invoice invoice)
    {
        InvoiceDto dto = invoice.ToDto();
        long? id = await _repo.InsertAsync<long>(dto);

        List<InvoiceItemDto> items = invoice.Items.Select(i => i.ToDto(id.Value)).ToList();
        await _repoItem.InsertAllAsync(items);

        return (await GetByIdAsync(id!.Value))!;
    }

    public async Task<Invoice?> GetByIdAsync(long invoiceId)
    {
        if ((await _repo.QueryAsync(invoiceId))?.FirstOrDefault() is not { } dto) { return null; }

        IEnumerable<InvoiceItemDto>? items = await _repoItem.QueryAsync(e => e.invoice_id == invoiceId);
        Business? business1 = await businessRepo.GetByIdAsync(dto.rechnungs_steller_id);
        Business? business2 = await businessRepo.GetByIdAsync(dto.rechnungs_empfaenger_id);
        Credit? credit = await creditRepo.GetByIdAsync(dto.credit_accounts_id);
        RechnungsNummer? rechnungsNummer = await rechnungsNummerRepo.GetByIdAsync(dto.rechnungsnummer_id);
        InvoiceSteuersatz steuersatz = InvoiceSteuersatz.ById(dto.steuersatz_id);


        return dto.ToDomain(
            business1!,
            business2!,
            credit!,
            rechnungsNummer!,
            items.Select(i => i.ToDomain()).ToList(),
            steuersatz);
    }

    public async Task<Invoice> UpdateAsync(long invoiceId,
        Business rechnungsSteller,
        Business rechnungsEmpfaenger,
        DateOnly rechnungsDatum,
        DateOnly lieferDatum,
        InvoiceSteuersatz steuersatz,
        Credit creditInfo,
        List<Invoice.InvoiceItem> invoiceItems)
    {
        if ((await _repo.QueryAsync(invoiceId))?.FirstOrDefault() is not { } dto)
        {
            throw new InvalidOperationException("Invoice not found");
        }

        dto.rechnungs_steller_id = rechnungsSteller.Id.Value;
        dto.rechnungs_empfaenger_id = rechnungsEmpfaenger.Id.Value;
        dto.erstellungs_datum = rechnungsDatum.ToDateTime(TimeOnly.MinValue);
        dto.liefer_datum = lieferDatum.ToDateTime(TimeOnly.MinValue);
        dto.credit_accounts_id = creditInfo.Id.Value;
        dto.steuersatz_id = steuersatz.Id;

        await _repo.UpdateAsync(dto);

        await _repoItem.DeleteAsync(e => e.invoice_id == invoiceId); // Replace all items
        await _repoItem.InsertAllAsync(invoiceItems.Select(i => i.ToDto(invoiceId)));

        RechnungsNummer? rechnungsNummer = await rechnungsNummerRepo.GetByIdAsync(dto.rechnungsnummer_id);
        
        return dto.ToDomain(
            rechnungsSteller,
            rechnungsEmpfaenger,
            creditInfo,
            rechnungsNummer!,
            invoiceItems,
            steuersatz);
    }

    public async Task<bool> ExistsAsync(long invoiceId)
    {
        return await _repo.ExistsAsync(e => e.invoice_id == invoiceId);
    }

    public async Task<IEnumerable<Invoice>> GetAllAsync()
    {
        using IDbConnection con = connectionFactory.CreateOpenConnection();

        IEnumerable<InvoiceDto> invoices = await _repo.QueryAllAsync();

        (HashSet<long> businessIds, HashSet<long> creditIds, HashSet<long> rechnungsNummerIds) = invoices.Aggregate(
            (new HashSet<long>(), new HashSet<long>(), new HashSet<long>()),
            (agg, i) =>
            {
                agg.Item1.Add(i.rechnungs_steller_id);
                agg.Item1.Add(i.rechnungs_empfaenger_id);
                agg.Item2.Add(i.credit_accounts_id);
                agg.Item3.Add(i.rechnungsnummer_id);
                return agg;
            });

        (IEnumerable<BusinessDto>? businesses,
                IEnumerable<CreditDto>? creditInfos,
                IEnumerable<RechnungsNummerDto>? rechnungsNummern,
                IEnumerable<InvoiceItemDto>? invoiceItems) =
            await con.QueryMultipleAsync<BusinessDto, CreditDto, RechnungsNummerDto, InvoiceItemDto>(
                where1: b => businessIds.Contains(b.id),
                where2: c => creditIds.Contains(c.id),
                // if -1 is not present, an error is thrown even though it makes no difference
                where3: rn => rechnungsNummerIds.Contains(rn.id ?? -1),
                where4: null);


        Dictionary<long, Business> businessesDict = businesses.ToDictionary(b => b.id, b => b.ToBusiness());
        Dictionary<long, Credit> creditInfoDict = creditInfos.ToDictionary(ci => ci.id, ci => ci.ToDomain());
        Dictionary<long, RechnungsNummer> rechnungsNummernDict =
            rechnungsNummern.ToDictionary(rn => rn.id!.Value, rn => rn.ToDomain());
        Dictionary<long, List<Invoice.InvoiceItem>> invoiceItemsDict = invoiceItems
            .GroupBy(ii => ii.invoice_id)
            .ToDictionary(
                ii => ii.Key,
                iis => iis.Select(ii => ii.ToDomain()).ToList());

        return invoices.Select(i =>
        {
            Business steller = businessesDict[i.rechnungs_steller_id];
            Business empfaenger = businessesDict[i.rechnungs_empfaenger_id];
            Credit credit = creditInfoDict[i.credit_accounts_id];
            RechnungsNummer rechnungsNummer = rechnungsNummernDict[i.rechnungsnummer_id];
            List<Invoice.InvoiceItem> items =
                invoiceItemsDict.GetValueOrDefault(i.invoice_id!.Value, []);
            InvoiceSteuersatz invoiceSteuersatz = InvoiceSteuersatz.ById(i.steuersatz_id);
            return i.ToDomain(
                steller,
                empfaenger,
                credit,
                rechnungsNummer,
                items: items,
                steuersatz: invoiceSteuersatz
            );
        });
    }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[Table("invoices")]
internal class InvoiceDto
{
    public long? invoice_id { get; set; }
    public long rechnungs_steller_id { get; set; }
    public long rechnungs_empfaenger_id { get; set; }
    public DateTime erstellungs_datum { get; set; }
    public DateTime liefer_datum { get; set; }

    public long rechnungsnummer_id { get; set; }

    public InvoiceSteuersatzId steuersatz_id { get; set; }
    public long credit_accounts_id { get; set; }

    public string angabe_zur_steuerbefreiung { get; set; } = null!;

    public List<long> invoice_items_id { get; set; } = null!;

    public string? ustid { get; set; }
    public string? hrb { get; set; }
    public string? amtsgreicht { get; set; }
    public string? geschaeftsfuehrer { get; set; }
    public string? webseite { get; set; }
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[Table("invoice_items")]
public class InvoiceItemDto
{
    public long invoice_id { get; set; }
    public string beschreibung { get; set; } = null!;
    public long quantity { get; set; }
    public decimal unit_price { get; set; }
}

internal static class InvoiceMappingExtensions
{
    internal static InvoiceDto ToDto(this Invoice invoice)
        => new()
        {
            invoice_id = invoice.Id,
            rechnungs_steller_id = invoice.RechnungsSteller.Id.Value,
            rechnungs_empfaenger_id = invoice.RechnungsEmpfaenger.Id.Value,
            erstellungs_datum = invoice.ErstellungsDatum.ToDateTime(TimeOnly.MinValue),
            liefer_datum = invoice.LieferDatum.ToDateTime(TimeOnly.MinValue),
            rechnungsnummer_id = invoice.Rechnungsnummer.Id.Value,
            steuersatz_id = invoice.SteuerAusweisung.Id,
            credit_accounts_id = invoice.RechnungsStellerCredit.Id.Value,
            angabe_zur_steuerbefreiung = invoice.AngabeZurSteuerbefreiung,
            invoice_items_id = [], // Not stored directly — handled separately
            ustid = invoice.UStId,
            hrb = invoice.HRB,
            amtsgreicht = invoice.Amtsgericht,
            geschaeftsfuehrer = invoice.Geschaeftsfuehrer,
            webseite = invoice.Webseite
        };

    internal static Invoice ToDomain(this InvoiceDto dto,
        Business steller, Business empfaenger, Credit credit,
        RechnungsNummer rechnungsNummer,
        List<Invoice.InvoiceItem> items, InvoiceSteuersatz steuersatz)
        => new(
            steller,
            empfaenger,
            DateOnly.FromDateTime(dto.erstellungs_datum),
            DateOnly.FromDateTime(dto.liefer_datum),
            rechnungsNummer,
            steuersatz,
            credit.Short,
            dto.angabe_zur_steuerbefreiung,
            [..items],
            credit,
            dto.ustid,
            dto.hrb,
            dto.amtsgreicht,
            dto.geschaeftsfuehrer,
            dto.webseite
        )
        {
            Id = dto.invoice_id!.Value
        };

    internal static InvoiceItemDto ToDto(this Invoice.InvoiceItem item, long invoiceId)
        => new()
        {
            invoice_id = invoiceId,
            beschreibung = item.Beschreibung,
            quantity = item.Quantity,
            unit_price = item.UnitPrice
        };

    internal static Invoice.InvoiceItem ToDomain(this InvoiceItemDto dto)
        => new(dto.beschreibung, (uint)dto.quantity, dto.unit_price);
}