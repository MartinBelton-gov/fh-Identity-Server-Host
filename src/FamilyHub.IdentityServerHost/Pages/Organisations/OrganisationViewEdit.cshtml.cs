using FamilyHub.IdentityServerHost.Services;
using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace FamilyHub.IdentityServerHost.Pages.Organisations;

public class OrganisationViewEditModel : PageModel
{
    private readonly IApiService _apiService;

    public OrganisationViewEditModel(IApiService apiService)
    {
        _apiService = apiService;
    }

    [BindProperty]
    public OrganisationModel Organisation { get; set; } = new OrganisationModel();
    public class OrganisationModel
    {
        public string Id { get; set; } = default!;

        [Required]
        [StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
        public string? Name { get; set; } = default!;

        public string? Description { get; set; }

        public string? Logo { get; set; }

        public string? Uri { get; set; }

        public string? Url { get; set; }
    }
    public async Task OnGet(string id)
    {
        if (!string.IsNullOrEmpty(id))
        {
            var apiOrganisation = await _apiService.GetOpenReferralOrganisationById(id);
            if (apiOrganisation != null)
            {
                Organisation.Id = apiOrganisation.Id;
                Organisation.Name = apiOrganisation.Name;
                Organisation.Description = apiOrganisation.Description;
                Organisation.Logo = apiOrganisation.Logo;
                Organisation.Uri = apiOrganisation.Uri;
                Organisation.Url = apiOrganisation.Url;

            }
        }
    }
}
