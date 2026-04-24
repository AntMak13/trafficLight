using trafficLight.App.Models;
using trafficLight.App.Services;

namespace trafficLight.App.Controllers;

public class TrafficLightController
{
    private readonly TrafficLight _cowLight;
    private readonly TrafficLight _sheepLight;
    private readonly List<TrafficLightPhase> _phases;
    private bool _flapIsOpen;

    public TrafficLightController(
        TrafficLight cowLight, 
        TrafficLight sheepLight,
        List<TrafficLightPhase> phases)
    {
        _cowLight = cowLight;
        _sheepLight = sheepLight;
        _phases = phases;
    }

    private void UpdateFlap(TrafficLightColor color)
    {
        switch (color)
        {
            case TrafficLightColor.Green:
                _flapIsOpen = true;
                break;
            case TrafficLightColor.Red:
                _flapIsOpen = false;
                break;
            case TrafficLightColor.Yellow:
                break;
        }
    }
    
    private string GetFlapSymbol()
    {
        return _flapIsOpen ? "О" : "З";
    }

    public async Task StartAsync()
    {
        while (true)
        {
            foreach (var phase in _phases)
            {
                UpdateFlap(phase.CowColor);
                
                _cowLight.SetColor(phase.CowColor);
                _sheepLight.SetColor(phase.SheepColor);
                
                Console.WriteLine($"Flap: {GetFlapSymbol()}");
                
                await Task.Delay(phase.TimeOfLight * 1000);
            }
        }
    }
}