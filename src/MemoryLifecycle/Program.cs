using MemoryLifecycle.Gc;

<<<<<<< HEAD
var command = args.Length > 0 ? args[0].ToLowerInvariant() : "all";

switch (command)
{
    case "gc-pressure":
    case "all":
        PressureScenarios.RunAll();
        break;

    default:
        Console.WriteLine("Команды: gc-pressure, all");
        break;
=======
public static class Program
{
    public static void Main()
    {
        GcModeScenarios.RunParallelAllocation();
        GcModeScenarios.MeasureForcedGen2Pause();
    }
>>>>>>> 51de407 (feat(gc): compare workstation and server gc modes)
}
