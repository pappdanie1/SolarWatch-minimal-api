namespace SolarWatch_MinimalApi.Services.Api;

public interface IGeoCodingApi
{
    Task<string> GetCity(string city);
}