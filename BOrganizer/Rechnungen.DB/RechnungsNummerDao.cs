using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Diagnostics.CodeAnalysis;
using DB.Core;
using Npgsql;
using Rechnungen.DB.Extensions;
using Rechnungen.Model.General;
using Rechnungen.Model.Invoices;
using Rechnungen.Services.Invoices;
using RepoDb;

namespace Rechnungen.DB;

internal class RechnungsNummerRepoDb(string connectionString)
    : BaseRepository<RechnungsNummerDto, NpgsqlConnection>(connectionString);

public class RechnungsNummerDao(
    string connectionString,
    IDbConnectionFactory connectionFactory) : IRechnungsNummerRepository
{
    private readonly RechnungsNummerRepoDb _repo = new(connectionString);

    public async Task<string> GetHighestNumberAsync(string kuerzel, string? jahr = null)
    {
        jahr ??= DateTime.Now.Year.ToString();

        using IDbConnection connection = connectionFactory.CreateOpenConnection();

        RechnungsNummerDto? dto = await GetHighestDto(connection, kuerzel, jahr);

        return dto?.number ?? "000000";
    }

    public async Task<RechnungsNummer> SaveWithMaxConstraintAsync(RechnungsNummer newRechnungsnummer)
    {
        using IDbConnection connection = connectionFactory.CreateOpenConnection();
        using IDbTransaction tx = connection.BeginTransaction();

        (long? id, string kuerzel, string jahr, string nummer) = newRechnungsnummer;

        RechnungsNummerDto? lastDto = await GetHighestDto(connection, kuerzel, jahr, tx);

        if (lastDto is not null && string.Compare(nummer, lastDto.number, StringComparison.Ordinal) <= 0)
        {
            throw new InvalidOperationException(
                $"New number '{nummer}' must be greater than existing '{lastDto}'");
        }

        if (await connection.InsertAsync(newRechnungsnummer.ToDto(), transaction: tx) is not long longId)
        {
            throw new InvalidOperationException(
                $"New number '{nummer}' must did not get a long id from the db!");
        }

        tx.Commit();

        if ((await _repo.QueryAsync(r => r.id == longId))?.FirstOrDefault() is not { } rechnungsNummer)
        {
            throw new InvalidOperationException($"Could not retrieve the rechnungsnummer for {nummer}");
        }


        return rechnungsNummer.ToDomain();
    }

    public async Task<RechnungsNummer?> GetByIdAsync(long rechnungsnummerId)
        => (await _repo.QueryAsync(rechnungsnummerId))?.FirstOrDefault()?.ToDomain();


    private async Task<RechnungsNummerDto?> GetHighestDto(
        IDbConnection connection, string kuerzel, string jahr, IDbTransaction? transaction = null)
        => (await connection
            .QueryAsync<RechnungsNummerDto>(
                e => e.kuerzel == kuerzel && e.year == jahr,
                orderBy: [OrderField.Descending<RechnungsNummerDto>(e => e.created_at)],
                top: 1,
                transaction: transaction)).FirstOrDefault();
}

[SuppressMessage("ReSharper", "InconsistentNaming")]
[Table("rechnungsnummer")]
internal class RechnungsNummerDto
{
    public required long? id { get; set; }
    public string kuerzel { get; set; } = null!;
    public string year { get; set; } = null!;
    public string number { get; set; } = null!;
    public DateTime created_at { get; set; } = DateTime.UtcNow;
}

internal static class RechnungsNummerMapper
{
    public static RechnungsNummerDto ToDto(this RechnungsNummer rn) =>
        new()
        {
            id = rn.Id,
            kuerzel = rn.Kuerzel,
            year = rn.Jahr,
            number = rn.Nummer,
            created_at = DateTime.UtcNow
        };

    public static RechnungsNummer ToDomain(this RechnungsNummerDto dto)
        => new(dto.id, dto.kuerzel, dto.year, dto.number);
}