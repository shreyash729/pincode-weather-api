using System.Text.Json;
using weatherApi.Models;

namespace weatherApi.Services;

public class WeatherService
{
    private readonly HttpClient _httpClient;

    public WeatherService(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherResponse?> GetWeatherAsync(
        double latitude,
        double longitude)
    {
        var url =
            "https://api.open-meteo.com/v1/forecast" +
            $"?latitude={latitude}" +
            $"&longitude={longitude}" +
            "&current=temperature_2m,relative_humidity_2m,wind_speed_10m,weather_code";

        var response = await _httpClient.GetAsync(url);

        response.EnsureSuccessStatusCode();

        var json = await response.Content.ReadAsStringAsync();

        using var document = JsonDocument.Parse(json);

        var current = document.RootElement.GetProperty("current");

        var weatherCode = current.GetProperty("weather_code").GetInt32();

        return new WeatherResponse
        {
            Temperature = current.GetProperty("temperature_2m").GetDouble(),
            Humidity = current.GetProperty("relative_humidity_2m").GetDouble(),
            WindSpeed = current.GetProperty("wind_speed_10m").GetDouble(),
            WeatherCode = weatherCode,
            Description = GetWeatherDescription(weatherCode)
        };
    }

    private static string GetWeatherDescription(int code){
        return code switch{
            
            0 => "Clear sky",

            1 or 2 or 3 => "Mainly clear, partly cloudy, or overcast",

            45 or 48 => "Fog",

            51 or 53 or 55 => "Drizzle",

            56 or 57 => "Freezing drizzle",

            61 or 63 or 65 => "Rain",

            66 or 67 => "Freezing rain",

            71 or 73 or 75 => "Snow fall",

            77 => "Snow grains",

            80 or 81 or 82 => "Rain showers",

            85 or 86 => "Snow showers",

            95 => "Thunderstorm",

            96 or 99 => "Thunderstorm with hail",

            _ => "Unknown"
        };
    }
}