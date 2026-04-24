using trafficLight.App.Services;

namespace trafficLight.App.Controllers;

public class TrafficLightController
{
    private readonly TrafficLight _cowLight;
    private readonly TrafficLight _sheepLight;
    private readonly List<TrafficLightPhase> _phases;

    public TrafficLightController(
        TrafficLight cowLight, 
        TrafficLight sheepLight,
        List<TrafficLightPhase> phases)
    {
        _cowLight = cowLight;
        _sheepLight = sheepLight;
        _phases = phases;
    }

    public async Task StartAsync()
    {
        while (true)
        {
            foreach (var phase in _phases)
            {
                _cowLight.SetColor(phase.CowColor);
                _sheepLight.SetColor(phase.SheepColor);
                
                await Task.Delay(phase.TimeOfLight * 1000);
            }
        }
    }
}