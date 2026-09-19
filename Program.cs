using System.Threading.RateLimiting;
using Microsoft.AspNetCore.RateLimiting;
using weatherApi.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

builder.Services.AddHttpClient<OlaMapsService>();
builder.Services.AddHttpClient<WeatherService>();

builder.Services.AddRateLimiter(options =>
{
    options.GlobalLimiter =
        PartitionedRateLimiter.Create<HttpContext, string>(httpContext =>
            RateLimitPartition.GetFixedWindowLimiter(
                partitionKey:
                    httpContext.Connection.RemoteIpAddress?.ToString()
                    ?? "unknown",
                factory: _ => new FixedWindowRateLimiterOptions
                {
                    PermitLimit = 10,
                    Window = TimeSpan.FromMinutes(1),
                    QueueLimit = 0,
                    AutoReplenishment = true
                }));

    options.OnRejected = async (context, cancellationToken) =>
    {
        context.HttpContext.Response.StatusCode =
            StatusCodes.Status429TooManyRequests;

        context.HttpContext.Response.Headers.RetryAfter = "60";

        await context.HttpContext.Response.WriteAsJsonAsync(
            new
            {
                error = "Too many requests.",
                message = "You have exceeded the rate limit. Please try again in 60 seconds.",
                retryAfterSeconds = 60
            },
            cancellationToken);
    };
});

var app = builder.Build();

app.UseRateLimiter();

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