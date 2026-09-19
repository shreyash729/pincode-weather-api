using System.Text.Json;
using weatherApi.Models;

namespace weatherApi.Services;

public class OlaMapsService
{
    private readonly HttpClient _httpClient;
    private readonly IConfiguration _configuration;

    public OlaMapsService(
        HttpClient httpClient,
        IConfiguration configuration)
    {
        _httpClient = httpClient;
        _configuration = configuration;
    }

    public async Task<OlaPrediction?> GetLocationFromPincodeAsync(string pincode)
    {
        var apiKey = _configuration["OlaMaps:ApiKey"];

        var url =
            $"https://api.olamaps.io/places/v1/autocomplete" +
            $"?input={Uri.EscapeDataString(pincode)}" +
            $"&api_key={Uri.EscapeDataString(apiKey ?? "")}";

        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        var olaResponse =
            JsonSerializer.Deserialize<OlaPlacesResponse>(
                json,
                new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

        if (olaResponse?.Predictions == null ||
            olaResponse.Predictions.Count == 0){
            return null;
        }

        // 1. Prefer an exact postal-code result
        var postalCodeResult = olaResponse.Predictions
            .FirstOrDefault(p =>
                p.Types.Contains("postal_code") &&
                p.Geometry?.Location != null);

        if (postalCodeResult != null)
        {
            return postalCodeResult;
        }

        // 2. Fallback: use the first result with valid coordinates
        var firstValidResult = olaResponse.Predictions
            .FirstOrDefault(p =>
                p.Geometry?.Location != null &&
                p.Geometry.Location.Lat != 0 &&
                p.Geometry.Location.Lng != 0);

        return firstValidResult;
    }
}