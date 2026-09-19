namespace weatherApi.Models;

public class OlaPlacesResponse
{
    public List<OlaPrediction> Predictions { get; set; } = new();
}

public class OlaPrediction
{
    public string Description { get; set; } = "";

    public List<string> Types { get; set; } = new();

    public OlaGeometry Geometry { get; set; } = new();
}

public class OlaGeometry
{
    public OlaLocation Location { get; set; } = new();
}

public class OlaLocation
{
    public double Lat { get; set; }

    public double Lng { get; set; }
}