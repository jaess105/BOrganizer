using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using DB.Core;
using Npgsql;
using Rechnungen.DB.Extensions;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.Invoices;
using RepoDb;

namespace Rechnungen.DB;

internal class CreditRepository(string connectionString) :
    BaseRepository<CreditDto, NpgsqlConnection>(connectionString);

public class CreditDao(
    string connectionString,
    IDbConnectionFactory connectionFactory) : ICreditRepository
{
    private readonly CreditRepository _repo = new(connectionString);

    public async Task<Credit> SaveAsync(string institute, string iban, string bic, string inhaber)
    {
        CreditDto dto = new()
        {
            institute = institute,
            iban = iban,
            bic = bic,
            inhaber = inhaber
        };
        await _repo.InsertAsync(dto);
        return dto.ToDomain();
    }

    public async Task<IEnumerable<Credit>> GetAllAsync()
        => (await _repo.QueryAllAsync()).Select(dto => dto.ToDomain());

    public async Task<Credit?> GetByIdAsync(long creditId)
    {
        return (await _repo.QueryAsync(c => c.id == creditId))
            .Select(dto => dto.ToDomain())
            .FirstOrDefault();
    }

    public async Task<Credit> UpdateAsync(long id, string institute, string iban, string bic, string inhaber)
    {
        using IDbConnection conn = connectionFactory.CreateOpenConnection();
        await conn.UpdateAsync(new CreditDto
        {
            id = id,
            institute = institute,
            bic = bic,
            iban = iban,
            inhaber = inhaber
        });

        return (await GetByIdAsync(id))!;
    }
}

[Table("credit_accounts")]
internal class CreditDto
{
    public long id { get; set; }
    public string institute { get; set; } = default!;
    public string iban { get; set; } = default!;
    public string bic { get; set; } = default!;
    public string inhaber { get; set; } = default!;


    public static CreditDto FromDomain(Credit credit) => new()
    {
        institute = credit.Institute,
        iban = credit.IBAN,
        bic = credit.BIC,
        inhaber = credit.Inhaber
    };

    public Credit ToDomain() => new(id, institute, iban, bic, inhaber);
}