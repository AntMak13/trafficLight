namespace trafficLight.App.Models;

public class TrafficLightState
{
    public TrafficLightColor Color { get; }
    public int TimeOfLight { get;  }

    public TrafficLightState(TrafficLightColor color, int timeOfLight)
    {
        Color = color;
        TimeOfLight = timeOfLight;
    }
}