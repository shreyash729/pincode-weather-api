# PIN Code Weather API 🌦️

A simple ASP.NET Core Web API that takes an Indian PIN code, finds its location using **Ola Maps**, and retrieves the current weather for that location using **Open-Meteo**.

The project is built with **C# and .NET 10** and is deployed to **Azure App Service** using **GitHub Actions**.

## Features

* 🔎 PIN code based location lookup
* 📍 Latitude and longitude from Ola Maps
* 🌤️ Current weather information
* 🌡️ Temperature
* 💧 Relative humidity
* 💨 Wind speed
* ⛈️ Weather condition/description
* 🔐 API key stored securely using configuration
* ☁️ Azure App Service deployment
* 🚀 Automatic deployment through GitHub Actions

## Tech Stack

* **Language:** C#
* **Framework:** ASP.NET Core
* **Runtime:** .NET 10
* **Location API:** Ola Maps Places API
* **Weather API:** Open-Meteo
* **Hosting:** Azure App Service
* **CI/CD:** GitHub Actions

## How It Works

The API follows this flow:

```text
PIN Code
   │
   ▼
Ola Maps API
   │
   ├── Location
   ├── Latitude
   └── Longitude
          │
          ▼
     Open-Meteo API
          │
          ▼
     Weather Data
          │
          ▼
      JSON Response
```

For the Ola Maps response, the API first looks for a result of type `postal_code`.

If an exact `postal_code` result isn't available, it falls back to the first result containing valid latitude and longitude coordinates.

## API Endpoint

### Get Weather by PIN Code

```http
GET /api/weather?pincode={PIN_CODE}
```

### Example

```http
GET /api/weather?pincode=205001
```

### Example Response

```json
{
  "pincode": "205001",
  "location": "Mainpuri, Mainpuri District, Uttar Pradesh, 205001, India",
  "latitude": 27.253706,
  "longitude": 79.010732,
  "weather": {
    "temperature": 28.4,
    "humidity": 65,
    "windSpeed": 8.2,
    "weatherCode": 1,
    "description": "Mainly clear, partly cloudy, or overcast"
  }
}
```

Weather values will change depending on the current conditions.

## Project Structure

```text
weatherApi/
│
├── Models/
│   ├── OlaPlacesResponse.cs
│   └── WeatherResponse.cs
│
├── Services/
│   ├── OlaMapsService.cs
│   └── WeatherService.cs
│
├── Properties/
│
├── Program.cs
├── appsettings.json
├── appsettings.Development.json
├── weatherApi.csproj
├── weatherApi.http
└── README.md
```

## Configuration

The Ola Maps API requires an API key.

The application reads the key using:

```text
OlaMaps:ApiKey
```

### Local Development

For local development, use .NET User Secrets instead of putting the API key directly into `appsettings.json`.

First, make sure your project contains a `UserSecretsId` in `weatherApi.csproj`.

Then run:

```bash
dotnet user-secrets set "OlaMaps:ApiKey" "YOUR_OLA_MAPS_API_KEY"
```

You can verify the configured secret with:

```bash
dotnet user-secrets list
```

The actual API key should **not** be committed to GitHub.

Your `appsettings.json` can contain:

```json
{
  "OlaMaps": {
    "ApiKey": ""
  }
}
```

### Azure

For Azure App Service, configure the application setting:

```text
OlaMaps__ApiKey
```

with your actual Ola Maps API key.

ASP.NET Core maps:

```text
OlaMaps__ApiKey
```

to:

```text
OlaMaps:ApiKey
```

This keeps the API key outside the source code.

## Run Locally

Make sure .NET 10 SDK is installed.

Clone the repository:

```bash
git clone https://github.com/shreyash729/pincode-weather-api.git
```

Move into the project directory:

```bash
cd weatherApi
```

Restore dependencies:

```bash
dotnet restore
```

Run the application:

```bash
dotnet run
```

The terminal will display the local URL.

Then call:

```text
/api/weather?pincode=205001
```

For example:

```text
https://localhost:xxxx/api/weather?pincode=205001
```

## Build the Project

To build the project:

```bash
dotnet build
```

To publish the application:

```bash
dotnet publish -c Release
```

## Deployment

The application is deployed to **Azure App Service**.

The deployment process is:

```text
GitHub
   │
   │ Push to main
   ▼
GitHub Actions
   │
   ├── Restore
   ├── Build
   ├── Publish
   └── Deploy
          │
          ▼
    Azure App Service
```

A GitHub Actions workflow is used for automatic deployment whenever changes are pushed to the `main` branch.

The Ola Maps API key is configured separately in Azure App Service and is not stored in the repository.

## Error Handling

If Ola Maps cannot provide a usable location, the API returns an error instead of attempting to request weather data with invalid coordinates.

The API also handles the case where an exact `postal_code` result is not returned by Ola Maps by falling back to the first prediction containing valid coordinates.

## Future Improvements

Possible future improvements include:

* Add Swagger/OpenAPI documentation
* Add stronger PIN code validation
* Add structured logging
* Add automated unit and integration tests
* Add caching for repeated PIN code requests
* Improve weather-code descriptions
* Add rate limiting
* Add a frontend application
* Add custom domain support
* Add monitoring and Application Insights

## License

This project is currently intended for learning and personal development.
