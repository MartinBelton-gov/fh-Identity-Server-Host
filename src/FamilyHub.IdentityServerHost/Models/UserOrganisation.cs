namespace FamilyHub.IdentityServerHost.Models;

public class UserOrganisation
{
    public string Id { get; set; } = default!;
    public string UserId { get; set; } = default!;
    public string OrganisationId { get; set; } = default!;
    public string Description { get; set; } = default!;
}
