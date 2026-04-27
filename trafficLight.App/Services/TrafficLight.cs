using trafficLight.App.Models;

namespace trafficLight.App.Services;

public class TrafficLight
{
    public string Name { get; }
    public TrafficLightColor Color { get; private set; }

    public TrafficLight(string name)
    {
        Name = name;
        Color = TrafficLightColor.Red;
    }

    public void SetColor(TrafficLightColor color)
    {
        Color = color;
        
        // Console.WriteLine($"{DateTime.Now:HH:mm:ss} | {Name}: {color}");
    }
}