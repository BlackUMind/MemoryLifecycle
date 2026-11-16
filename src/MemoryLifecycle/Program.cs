using MemoryLifecycle.Demos;
using MemoryLifecycle.Gc;

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
}

public static partial class Program 
{
    public static void Main()
    {
        //GcModeScenarios.RunParallelAllocation();
        //GcModeScenarios.MeasureForcedGen2Pause();


        ResourceDemos.RunFinalizerDemo();
        ResourceDemos.RunUsingDemo();
        ResourceDemos.RunForgottenDisposeDemo();
        ResourceDemos.RunLogFileDemo("memory-lifecycle.log");
    }
}
