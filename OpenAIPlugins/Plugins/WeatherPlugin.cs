using Microsoft.SemanticKernel;
using System.ComponentModel;

namespace OpenAIPlugins.Plugins;

public class WeatherPlugin(HttpClient httpClient)
{
    [KernelFunction("get_weather_of_city")]
    [Description("Gets the current weather in a given city")]
    [return: Description("A string containing the weather in the requested city")]
    public async Task<string> GetWeatherAsync(string city)
    {
        var response = await httpClient.GetStringAsync($"https://api.weatherapi.com/v1/current.json?key=<YOUR_API_KEY>&q={city}"); // register for free on weatherapi to see this in action using your own API key
        return response;
    }
}
