using trafficLight.App.Controllers;
using trafficLight.App.Services;

class Program
{
    static async Task Main()
    {
        var cowLight = new TrafficLight("Cows");
        var sheepLight = new TrafficLight("Sheep");
        var goatLight = new TrafficLight("Goat");

        var controller = new TrafficLightController(cowLight, sheepLight, goatLight);
        controller.StartInputListener();
        await controller.StartAsync();
    }
}