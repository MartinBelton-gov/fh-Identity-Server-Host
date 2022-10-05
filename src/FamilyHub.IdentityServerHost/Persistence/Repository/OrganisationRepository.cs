using FamilyHub.IdentityServerHost.Models.Entities;

namespace FamilyHub.IdentityServerHost.Persistence.Repository;

public interface IOrganisationRepository
{
    Task AddUserOrganisationAsync(UserOrganisation userOrganisation, CancellationToken cancellationToken = new CancellationToken());
}

public class OrganisationRepository : IOrganisationRepository
{
    private readonly IApplicationDbContext _applicationDbContext;
    private readonly ILogger<OrganisationRepository> _logger;
    public OrganisationRepository(IApplicationDbContext applicationDbContext, ILogger<OrganisationRepository> logger)
    {
        _applicationDbContext = applicationDbContext;
        _logger = logger;
    }
    public async Task AddUserOrganisationAsync(UserOrganisation userOrganisation, CancellationToken cancellationToken = new CancellationToken())
    {
        try
        {
            _applicationDbContext.UserOrganisations.Add(userOrganisation);
            await _applicationDbContext.SaveChangesAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "An error occurred creating organisation. {exceptionMessage}", ex.Message);
            throw new Exception(ex.Message, ex);
        }
        
    }

}
