using trafficLight.App.Models;
using trafficLight.App.Services;

namespace trafficLight.App.Controllers;

public class TrafficLightController
{
    private readonly TrafficLight _cowLight;
    private readonly TrafficLight _sheepLight;
    private readonly TrafficLight _goatLight;
    private string? _requestedPet;
    private string _currentPet;

    private readonly Dictionary<string, bool> _flaps = new()
    {
        ["Cow"] = false,
        ["Sheep"] = false,
        ["Goat"] = false
    };

    public TrafficLightController(
        TrafficLight cowLight, 
        TrafficLight sheepLight,
        TrafficLight goatLight)
    {
        _cowLight = cowLight;
        _sheepLight = sheepLight;
        _goatLight = goatLight;
    }

    public void StartInputListener()
    {
        Task.Run(() =>
            {
                while (true)
                {
                    var key = Console.ReadKey(true).KeyChar;

                    switch (char.ToLower(key))
                    {
                        case 'c':
                            Request("Cow");
                            break;
                        case 's':
                            Request("Sheep");
                            break;
                        case 'g':
                            Request("Goat");
                            break;
                    }
                }
            }
        );
    }

    private void Request(string pet)
    {
        Console.WriteLine($"[INPUT] Request from {pet}");
        _requestedPet = pet;
    }

    private void PrintState()
    {
        PrintPet("Cow", _cowLight.Color);
        PrintPet("Sheep", _sheepLight.Color);
        PrintPet("Goat", _goatLight.Color);
    }

    private void PrintPet(string pet, TrafficLightColor color)
    {
        Console.WriteLine($"{DateTime.Now:HH:mm:ss} | {pet}: {color} | Flap: {GetFlapSymbol(pet)}");
    }

    private void ApplyState(string activePet, TrafficLightColor color)
    {
        SetLights(activePet, color);
        
        RecalculateFlaps();
        
        PrintState();
    }

    private void RecalculateFlaps()
    {
        _flaps["Cow"] = _cowLight.Color == TrafficLightColor.Green;
        _flaps["Sheep"] = _sheepLight.Color == TrafficLightColor.Green;
        _flaps["Goat"] = _goatLight.Color == TrafficLightColor.Green;
    }

    private void ApplyTransitionState(string from, string to)
    {
        _cowLight.SetColor(GetTransitionColor("Cow", from, to));
        _sheepLight.SetColor(GetTransitionColor("Sheep", from, to));
        _goatLight.SetColor(GetTransitionColor("Goat", from, to));
        
        PrintState();
    }

    private TrafficLightColor GetTransitionColor(string pet, string from, string to)
    {
        if (pet == from || pet == to)
        {
            return TrafficLightColor.Yellow;
        }
        
        return TrafficLightColor.Red;
    }
    
    private string GetFlapSymbol(string pet)
    {
        return _flaps[pet] ? "О" : "З";
    }

    public async Task StartAsync()
    {
        _currentPet = AskStartPet();
        
        Console.WriteLine($"Started with: {_currentPet}");

        StartInputListener();
        
        while (true)
        {
            await RunCurrentPetAsync();

            if (_requestedPet != null && _requestedPet != _currentPet)
            {
                await SwitchToRequestedAsync();
            }
        }
    }

    private async Task RunCurrentPetAsync()
    {
        Console.WriteLine($"Active: {_currentPet}");
        
        ApplyState(_currentPet, TrafficLightColor.Green);
        
        await Task.Delay(15000);
    }

    private async Task SwitchToRequestedAsync()
    {
        var from = _currentPet;
        var to = _requestedPet;
        
        ApplyTransitionState(from, to);
        await Task.Delay(5000);
        
        _currentPet = to;
        _requestedPet = null;

        ApplyState(to, TrafficLightColor.Green);
        
        await Task.Delay(15000);
    }

    private void SetLights(string activePet, TrafficLightColor activeColor)
    {
        _cowLight.SetColor(activePet == "Cow" ? activeColor : TrafficLightColor.Red);
        _sheepLight.SetColor(activePet == "Sheep" ? activeColor : TrafficLightColor.Red);
        _goatLight.SetColor(activePet == "Goat" ? activeColor : TrafficLightColor.Red);
    }
    
    
    public string AskStartPet()
    {
        while (true)
        {
            Console.WriteLine("Let's start with (c - Cow, s - Sheep, g - Goat)");
            
            var key = Console.ReadKey(true).KeyChar;

            switch (char.ToLower(key))
            {
                case 'c':
                    return "Cow";
                case 's':
                    return "Sheep";
                case 'g':
                    return "Goat";
            }
            
            Console.WriteLine("Incorrect input. Try again.");
        }
    }
}