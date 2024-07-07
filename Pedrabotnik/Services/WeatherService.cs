namespace Pedrabotnik.Services;

public class WeatherService
{
    public async Task<string> GetCurrentWeather(string city)
    {
        // Здесь вы можете реализовать логику получения текущей погоды для указанного города
        // Например, вы можете вызвать API погоды и вернуть текущую температуру

        return $"Текущая погода в городе {city}: +20°C";
    }
}
