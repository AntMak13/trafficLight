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
    private bool _started;
    private bool _wolvesMode;
    private CancellationTokenSource _cts = new();

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

    private void InputLoop()
    {
        while (true)
        {
            var key = Console.ReadKey(true).KeyChar;
            var k = char.ToLower(key);

            if (k == 'w')
            {
                ToggleWolvesMode();
                continue;
            }

            if (string.IsNullOrEmpty(_currentPet))
            {
                switch (k)
                {
                    case 'c' : _currentPet = "Cow"; break;
                    case 's' : _currentPet = "Sheep"; break;
                    case 'g' : _currentPet = "Goat"; break;
                    default:
                        Console.WriteLine($"Unknown key {k}");
                        continue;
                }
            }
            
            HandleRequest(key);
        }
    }

    private void HandleRequest(char key)
    {
        var k = char.ToLower(key);
        
        switch (k)
        {
            case 'w':
                ToggleWolvesMode();
                return;
            case 'c':
            case 's':
            case 'g':
                if (_wolvesMode)
                {
                    Console.WriteLine("Incorrect");
                    return;
                }
                break;
        }

        switch (k)
        {
            case 'c': Request("Cow"); break;
            case 's': Request("Sheep"); break;
            case 'g': Request("Goat"); break;
        }
    }

    private void ToggleWolvesMode()
    {
        _wolvesMode = !_wolvesMode;
        
        Console.WriteLine(_wolvesMode ? "Wolves mode ON" : "Wolves mode OFF");

        _cts.Cancel();
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
        
        // PrintState();
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
        return _flaps[pet] ? "Оw" : "C";
    }

    public async Task StartAsync()
    {
        Console.WriteLine("Let`s start! (c - Cows, s - Sheep, g - Goats)\n");
        
        Task.Run(InputLoop);
        
        Task.Run(StatePrinterLoopAsync);

        while (true)
        {
            if (_wolvesMode)
            {
                await EnterWolvesModeAsync();
                continue;
            }

            if (string.IsNullOrEmpty(_currentPet))
            {
                await Task.Delay(50);
                continue;
            }
            
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

        await SafeDelay(15000);
    }

    private async Task SwitchToRequestedAsync()
    {
        var from = _currentPet;
        var to = _requestedPet;
        
        ApplyTransitionState(from, to);
        await SafeDelay(5000);
        
        _currentPet = to;
        _requestedPet = null;

        ApplyState(to, TrafficLightColor.Green);
        
        await SafeDelay(15000);
    }

    private async Task EnterWolvesModeAsync()
    {
        var current = _currentPet;
        
        ApplyTransitionState(current, current);
        await SafeDelay(5000);

        ApplyAllRed();

        while (_wolvesMode)
        {
            await SafeDelay(100);
        }
        
        // ApplyState(_currentPet, TrafficLightColor.Green);
        await ExitWolvesModeAsync();
    }

    private async Task ExitWolvesModeAsync()
    {
        var pet = _currentPet;
        
        _cowLight.SetColor(pet == "Cow" ? TrafficLightColor.Yellow :  TrafficLightColor.Red);
        _sheepLight.SetColor(pet == "Sheep" ? TrafficLightColor.Yellow :  TrafficLightColor.Red);
        _goatLight.SetColor(pet == "Goat" ? TrafficLightColor.Yellow :  TrafficLightColor.Red);
        
        // PrintState();

        await SafeDelay(5000);
        
        ApplyState(pet, TrafficLightColor.Green);
    }
    
    private void ApplyState(string activePet, TrafficLightColor color)
    {
        SetLights(activePet, color);
        
        RecalculateFlaps();
        
        // PrintState();
    }
    
    private async Task SafeDelay(int ms)
    {
        try
        {
            await Task.Delay(ms, _cts.Token);
        }
        catch (TaskCanceledException)
        {
            _cts = new CancellationTokenSource();
        }
    }

    private async Task StatePrinterLoopAsync()
    {
        while (true)
        {
            PrintState();
            await Task.Delay(1000);
        }
    }
    
    private void ApplyAllRed()
    {
        _cowLight.SetColor(TrafficLightColor.Red);
        _sheepLight.SetColor(TrafficLightColor.Red);
        _goatLight.SetColor(TrafficLightColor.Red);
        
        RecalculateFlaps();
        // PrintState();
    }

    private void SetLights(string activePet, TrafficLightColor activeColor)
    {
        _cowLight.SetColor(activePet == "Cow" ? activeColor : TrafficLightColor.Red);
        _sheepLight.SetColor(activePet == "Sheep" ? activeColor : TrafficLightColor.Red);
        _goatLight.SetColor(activePet == "Goat" ? activeColor : TrafficLightColor.Red);
    }
}