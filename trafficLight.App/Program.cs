using trafficLight.App.Controllers;
using trafficLight.App.Models;
using trafficLight.App.Services;

class Program
{
    static async Task Main()
    {
        var cowLight = new TrafficLight("Cows");
        var sheepLight = new TrafficLight("Sheep");
        var phases = new List<TrafficLightPhase>
        {
            new TrafficLightPhase(TrafficLightColor.Green, TrafficLightColor.Red, 15),
            new TrafficLightPhase(TrafficLightColor.Yellow, TrafficLightColor.Yellow, 5),
            new TrafficLightPhase(TrafficLightColor.Red, TrafficLightColor.Green, 15),
            new TrafficLightPhase(TrafficLightColor.Yellow, TrafficLightColor.Yellow, 5),
        };

        var controller = new TrafficLightController(cowLight, sheepLight, phases);
        await controller.StartAsync();
    }
}