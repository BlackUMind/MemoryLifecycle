using MemoryLifecycle.Gc;

// Точка входа и диспетчер команд проекта MemoryLifecycle.
// Команды добавляются по мере выполнения этапов (КТ №40–43).

var command = args.Length > 0 ? args[0].ToLowerInvariant() : "help";

switch (command)
{
    case "gc-pressure": // КТ №40
        PressureScenarios.RunAll();
        break;

    case "gc-modes":    // КТ №41
    case "resources":   // КТ №42
    case "dispose":     // КТ №43
        Console.WriteLine($"Команда '{command}' ещё не реализована.");
        break;

    case "all":
        PressureScenarios.RunAll();
        break;

    default:
        PrintHelp();
        break;
}

static void PrintHelp()
{
    Console.WriteLine("MemoryLifecycle — исследование GC и времени жизни объектов в .NET 10");
    Console.WriteLine();
    Console.WriteLine("Использование: dotnet run --project src/MemoryLifecycle -c Release -- <команда>");
    Console.WriteLine();
    Console.WriteLine("  gc-pressure  КТ №40. Поколения и давление на кучу");
    Console.WriteLine("  gc-modes     КТ №41. Workstation GC и Server GC");
    Console.WriteLine("  resources    КТ №42. Финализатор, IDisposable, using");
    Console.WriteLine("  dispose      КТ №43. Паттерн Dispose, IAsyncDisposable");
    Console.WriteLine("  all          Все сценарии подряд");
}
