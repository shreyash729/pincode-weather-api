namespace weatherApi.Models;

public class WeatherResponse
{
    public double Temperature { get; set; }

    public double Humidity { get; set; }

    public double WindSpeed { get; set; }

    public int WeatherCode { get; set; }

    public string Description { get; set; } = "";
}