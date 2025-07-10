using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Linq.Expressions;
using DB.Core;
using Npgsql;
using Rechnungen.DB.Extensions;
using Rechnungen.Model.General;
using Rechnungen.Services.General;
using RepoDb;

namespace Rechnungen.DB;

internal class BusinessRepoDb(string connectionString)
    : BaseRepository<BusinessDto, NpgsqlConnection>(connectionString);

internal class PersonRepoDb(string connectionString)
    : BaseRepository<PersonDto, NpgsqlConnection>(connectionString);

internal class JoinRepoDb(string connectionString)
    : BaseRepository<PersonBusiness, NpgsqlConnection>(connectionString);

public class BusinessDao(
    string connectionString,
    IDbConnectionFactory connectionFactory) : IBusinessRepository
{
    private readonly BusinessRepoDb _businessRepo = new(connectionString);
    private readonly PersonRepoDb _personRepo = new(connectionString);
    private readonly JoinRepoDb _joinRepo = new(connectionString);

    public async Task<Business> SaveBusinessAsync(
        Business business,
        Person? person = null,
        bool isPrimaryBusiness = false)
    {
        using IDbConnection conn = connectionFactory.CreateOpenConnection();
        using IDbTransaction tx = conn.BeginTransaction();

        try
        {
            if (isPrimaryBusiness)
            {
                await conn.ExecuteNonQueryAsync(
                    "UPDATE businesses SET is_primary = FALSE WHERE is_primary = TRUE", transaction: tx);
            }

            long? personId = null;

            if (person is not null)
            {
                // Try to get existing person
                IEnumerable<PersonDto>? existing = await conn.QueryAsync<PersonDto>(
                    p => p.first_name == person.FirstName && p.last_name == person.LastName,
                    transaction: tx);

                personId = existing.FirstOrDefault()?.id
                           ?? await conn.InsertAsync<PersonDto, long>(
                               new PersonDto
                               {
                                   first_name = person.FirstName,
                                   last_name = person.LastName
                               },
                               transaction: tx
                           );
            }

            var businessDto = new BusinessDto
            {
                name = business.Name,
                street = business.Street,
                number = business.Number,
                plz = business.Plz,
                ort = business.Ort,
                country = business.Country,
                tel = business.Tel,
                email = business.EMail,
                is_primary = isPrimaryBusiness
            };
            long businessId;
            if (business.Id is not null)
            {
                businessId = business.Id.Value;
                await conn.UpdateAsync(businessDto, transaction: tx);
            }
            else { businessId = await conn.InsertAsync<BusinessDto, long>(businessDto, transaction: tx); }


            if (personId is { } personIdLong)
            {
                await conn.InsertAsync(new
                    PersonBusiness
                    {
                        person_id = personIdLong,
                        business_id = businessId
                    }, transaction: tx);
            }

            tx.Commit();

            return business;
        }
        catch
        {
            tx.Rollback();
            throw;
        }
    }

    public async Task<Business?> GetPrimaryBusinessAsync()
    {
        if ((await _businessRepo.QueryAsync(where: b => b.is_primary == true, top: 1))
            ?.FirstOrDefault() is not { } bDto) { return null; }

        if ((await _joinRepo.QueryAsync(j => j.business_id == bDto.id, top: 1))
            ?.FirstOrDefault() is not { } bToP) { return bDto.ToBusiness(); }

        var person = (await _personRepo.QueryAsync(p => p.id == bToP.person_id))
            ?.FirstOrDefault()?.ToPerson();

        return bDto.ToBusiness(person);
    }

    public async Task<IEnumerable<Business>> GetAllAsync()
    {
        using IDbConnection conn = connectionFactory.CreateOpenConnection();
        (IEnumerable<BusinessDto>? businesses,
                IEnumerable<PersonDto>? persons,
                IEnumerable<PersonBusiness>? personBusiness) =
            await conn.QueryMultipleAsync<BusinessDto, PersonDto, PersonBusiness>(
                where1: (Expression<Func<BusinessDto, bool>>)null,
                where2: null,
                where3: null);

        Dictionary<long, BusinessDto> businessDict = businesses.ToDictionary(b => b.id, b => b);
        Dictionary<long, PersonDto> personDict = persons.ToDictionary(p => p.id, p => p);

        IEnumerable<Business> allAsync = personBusiness.DistinctBy(pb => pb.business_id)
            .Select(pb =>
                businessDict[pb.business_id].ToBusiness(personDict.GetValueOrDefault(pb.person_id)?.ToPerson()))
            .ToArray();
        return allAsync;
    }

    public async Task<Business?> GetByIdAsync(long businessId)
    {
        using IDbConnection conn = connectionFactory.CreateOpenConnection();
        (IEnumerable<BusinessDto>? businesses,
                IEnumerable<PersonDto>? persons,
                IEnumerable<PersonBusiness>? personBusiness) =
            await conn.QueryMultipleAsync<BusinessDto, PersonDto, PersonBusiness>(
                where1: b => b.id == businessId,
                where2: p => p.id == businessId,
                where3: pb => pb.business_id == businessId);

        Person? person = null;
        if (personBusiness.FirstOrDefault()?.person_id is { } personId)
        {
            person = persons.FirstOrDefault(p => p.id == personId)?.ToPerson();
        }

        var business = businesses.FirstOrDefault()?.ToBusiness(person);
        return business;
    }

    public Task UpdateAsync(Business fullBusiness, Person? person, bool isUsersBusiness)
    {
        return SaveBusinessAsync(fullBusiness, person, isUsersBusiness);
    }
}

[Table("persons")]
public class PersonDto
{
    public long id { get; set; }
    public string first_name { get; set; } = default!;
    public string last_name { get; set; } = default!;

    public Person ToPerson() => new(id, first_name, last_name);
}

[Table("businesses")]
public class BusinessDto
{
    public long id { get; set; }
    public string name { get; set; } = default!;
    public string street { get; set; } = default!;
    public string number { get; set; } = default!;
    public string plz { get; set; } = default!;
    public string ort { get; set; } = default!;
    public string? country { get; set; }
    public string? tel { get; set; }
    public string? email { get; set; }
    public bool is_primary { get; set; } = false;

    public List<long> person_ids { get; set; } = new(); // Associated Person IDs

    public Business ToBusiness(Person? person = null)
        => new(id, person, name, street, number, plz, ort,
            Country: country, Tel: tel, EMail: email);
}

[Table("person_businesses")]
public class PersonBusiness
{
    public long person_id { get; set; }
    public long business_id { get; set; }
}