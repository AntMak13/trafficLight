using trafficLight.App.Models;

namespace trafficLight.App.Services;

public class TrafficLight
{
    public string Name { get; }

    public TrafficLight(string name)
    {
        Name = name;
    }

    public void SetColor(TrafficLightColor color)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} | {Name}: {color}");
    }
}