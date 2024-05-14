namespace SolarWatch_MinimalApi.Services.Api;

public interface ISunApi
{
    Task<string> GetSunriseSunset(double lat, double lon);
}