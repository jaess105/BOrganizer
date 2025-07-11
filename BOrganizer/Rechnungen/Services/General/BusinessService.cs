using Rechnungen.Model.General;

namespace Rechnungen.Services.General;

public interface IBusinessRepository
{
    Task<Business> SaveBusinessAsync(
        Business business,
        Person? person = null,
        bool isPrimaryBusiness = false);

    Task<Business?> GetPrimaryBusinessAsync();
    Task<IEnumerable<Business>> GetAllAsync();
    Task<Business?> GetByIdAsync(long businessId);
    Task UpdateAsync(Business fullBusiness, Person? person, bool isUsersBusiness);
    Task<bool> IsPrimaryBusinessAsync(long businessId);
}

public interface IBusinessService
{
    Task<Business> CreateBusinessAsync(Business fullBusiness, Person? person = null, bool isUsersBusiness = false);
    Task<Business?> GetPrimaryBusinessAsync();
    Task<IEnumerable<Business>> GetBusinessesAsync();
    Task<Business?> GetBusinessByIdAsync(long businessId);
    Task UpdateBusinessAsync(Business fullBusiness, Person? person, bool isUsersBusiness);
    Task<bool> IsPrimaryBusinessAsync(long? businessId);
}

public class BusinessService(IBusinessRepository businessRepo) : IBusinessService
{
    public Task<Business> CreateBusinessAsync(
        Business fullBusiness,
        Person? person = null,
        bool isUsersBusiness = false)
        => businessRepo.SaveBusinessAsync(fullBusiness, person, isUsersBusiness);

    public Task<Business?> GetPrimaryBusinessAsync() => businessRepo.GetPrimaryBusinessAsync();

    public Task<IEnumerable<Business>> GetBusinessesAsync() => businessRepo.GetAllAsync();
    public Task<Business?> GetBusinessByIdAsync(long businessId) => businessRepo.GetByIdAsync(businessId);

    public async Task UpdateBusinessAsync(Business fullBusiness, Person? person, bool isUsersBusiness)
    {
        await businessRepo.UpdateAsync(fullBusiness, person, isUsersBusiness);
    }

    public Task<bool> IsPrimaryBusinessAsync(long? businessId)
    {
        if (businessId is null) { return Task.FromResult(false); }

        return businessRepo.IsPrimaryBusinessAsync(businessId.Value);
    }
}