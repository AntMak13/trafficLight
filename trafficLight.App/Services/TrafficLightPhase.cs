using trafficLight.App.Models;

namespace trafficLight.App.Services;

public class TrafficLightPhase
{
    public TrafficLightColor CowColor { get; }
    public TrafficLightColor SheepColor { get; }
    public int TimeOfLight { get; }

    public TrafficLightPhase(
        TrafficLightColor cowColor,
        TrafficLightColor sheepColor,
        int timeOfLight
    )
    {
        CowColor = cowColor;
        SheepColor = sheepColor;
        TimeOfLight = timeOfLight;
    }
}