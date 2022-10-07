using FamilyHubs.ServiceDirectory.Shared.Models.Api.OpenReferralOrganisations;
using System.Text.Json;

namespace FamilyHub.IdentityServerHost.Services;

public interface IApiService
{
    Task<List<OpenReferralOrganisationDto>> GetListOpenReferralOrganisations();
    Task<OpenReferralOrganisationDto> GetOpenReferralOrganisationById(string id);
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

    public async Task<OpenReferralOrganisationDto> GetOpenReferralOrganisationById(string id)
    {
        var request = new HttpRequestMessage
        {
            Method = HttpMethod.Get,
            RequestUri = new Uri(_client.BaseAddress + $"api/organizations/{id}"),

        };

        using var response = await _client.SendAsync(request);

        response.EnsureSuccessStatusCode();

        return await JsonSerializer.DeserializeAsync<OpenReferralOrganisationDto>(await response.Content.ReadAsStreamAsync(), options: new JsonSerializerOptions { PropertyNameCaseInsensitive = true }) ?? new OpenReferralOrganisationDto();

    }
}