using SolarWatch_MinimalApi.Services.Api;
using SolarWatch_MVC.Services.Api;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<IGeoCodingApi, GeocodingApi>();
builder.Services.AddSingleton<ISunApi, SunApi>();
builder.Services.AddSingleton<IJsonProcessor, JSonProcessor>();

var app = builder.Build();

app.MapGet("/", () => "For City info: /city/{city}, For sunset sunrise times: /solar-watch/{city}");

app.MapGet("/city/{city}", async (string city, IGeoCodingApi geoCodingApi, IJsonProcessor jsonProcessor) =>
{
    if (string.IsNullOrEmpty(city) || city.Length < 3)
    {
        return Results.BadRequest("City name must be at least 3 characters long.");
    }
    var data = await geoCodingApi.GetCity(city);
    var cityInfo = jsonProcessor.ProcessCity(data);
    if (cityInfo == null)
    {
        return Results.NotFound("city not found");
    }
    return Results.Ok(cityInfo);
});

app.MapGet("/solar-watch/{city}", async (string city, IGeoCodingApi geoCodingApi, ISunApi sunApi, IJsonProcessor jsonProcessor) =>
{
    if (string.IsNullOrEmpty(city) || city.Length < 3)
    {
        return Results.BadRequest("City name must be at least 3 characters long.");
    }
    var cityData = await geoCodingApi.GetCity(city);
    var cityInfo = jsonProcessor.ProcessCity(cityData);
    
    if (cityInfo == null)
    {
        return Results.NotFound("city not found");
    }

    var sunData = await sunApi.GetSunriseSunset(cityInfo.Latitude, cityInfo.Longitude);
    var sunsetSunrise = jsonProcessor.ProcessSunriseSunset(sunData, cityInfo);
    
    return Results.Ok(sunsetSunrise);
});

app.Run();