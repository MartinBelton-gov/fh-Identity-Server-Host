using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using System.Text.Json;

namespace FamilyHub.IdentityServerHost.Services;

public interface IApiService
{
    Task<List<OpenReferralOrganisationDto>> GetListOpenReferralOrganisations();
}

public class ApiService : IApiService
{
    protected readonly HttpClient _client;

    public ApiService(HttpClient client)
    {
        _client = client;
    }

    public async Task<List<OpenReferralOrganisationDto>> GetListOpenReferralOrganisations()
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + "api/organizations"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<List<OpenReferralOrganisationDto>>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new List<OpenReferralOrganisationDto>();

    }
}