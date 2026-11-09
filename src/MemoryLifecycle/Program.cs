using MemoryLifecycle.Gc;

var command = args.Length > 0 ? args[0].ToLowerInvariant() : "";

switch (command)
{
    case "gc-pressure":
    case "all":
        PressureScenarios.RunAll();
        break;

    default:
        Console.WriteLine("Команды: gc-pressure, all");
        break;
}
