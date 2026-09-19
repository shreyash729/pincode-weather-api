using weatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient<OlaMapsService>();
builder.Services.AddHttpClient<WeatherService>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.MapGet("/api/weather", async (
    string pincode,
    OlaMapsService olaMapsService,
    WeatherService weatherService) =>
{
    var location =
        await olaMapsService.GetLocationFromPincodeAsync(pincode);

    if (location == null)
    {
        return Results.BadRequest(new
        {
            error = "Please enter a valid PIN code."
        });
    }

    var latitude = location.Geometry.Location.Lat;
    var longitude = location.Geometry.Location.Lng;

    var weather =
        await weatherService.GetWeatherAsync(latitude, longitude);

    if (weather == null)
    {
        return Results.Problem(
            "Unable to retrieve weather information.");
    }

    return Results.Ok(new
    {
        pincode,
        location = location.Description,
        latitude,
        longitude,
        weather
    });
});

app.Run();