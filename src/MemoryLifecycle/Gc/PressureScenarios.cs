using System.Diagnostics;

namespace MemoryLifecycle.Gc;

public static class PressureScenarios
{
    private static readonly List<byte[]> LongLived = new();

    public static ScenarioResult RunSmallObjects(int count = 100_000)
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        long checksum = 0;
        for (var i = 0; i < count; i++)
        {
            var buffer = new byte[100];
            buffer[0] = (byte)i;
            checksum += buffer[0];
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();
        GC.KeepAlive(checksum);

        return new ScenarioResult("Мелкие объекты", stopwatch.Elapsed, after.Diff(before));
    }

    public static ScenarioResult RunLargeObjects(int count = 10)
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        var list = new List<byte[]>(count);
        for (var i = 0; i < count; i++)
        {
            list.Add(new byte[100_000]);
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();
        GC.KeepAlive(list);

        return new ScenarioResult("Крупные объекты", stopwatch.Elapsed, after.Diff(before));
    }

    public static ScenarioResult RunLongLived(int count = 20_000)
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < count; i++)
        {
            LongLived.Add(new byte[512]);
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();

        return new ScenarioResult("Удерживаемые объекты", stopwatch.Elapsed, after.Diff(before));
    }

    public static ScenarioResult RunForcedGen2()
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        GC.Collect(2, GCCollectionMode.Forced, blocking: true);

        stopwatch.Stop();
        var after = GcSnapshot.Capture();

        return new ScenarioResult("Принудительный Gen2", stopwatch.Elapsed, after.Diff(before));
    }

    public static void RunAll()
    {
        var startAllocated = GC.GetTotalAllocatedBytes(precise: true);

        GcObserver.PrintMemoryInfo("Состояние кучи до экспериментов");
        Console.WriteLine();

        Console.WriteLine("{0,-24}{1,10}{2,6}{3,6}{4,6}{5,12}{6,12}",
            "Сценарий", "Время, мс", "Gen0", "Gen1", "Gen2", "Alloc, МБ", "Heap Δ, МБ");

        var results = new[]
        {
            RunSmallObjects(),
            RunLargeObjects(),
            RunLongLived(),
            RunForcedGen2()
        };

        foreach (var r in results)
        {
            Console.WriteLine("{0,-24}{1,10:F2}{2,6}{3,6}{4,6}{5,12:F2}{6,12:F2}",
                r.Name,
                r.Elapsed.TotalMilliseconds,
                r.Delta.Gen0,
                r.Delta.Gen1,
                r.Delta.Gen2,
                r.Delta.AllocatedMegabytes,
                r.Delta.HeapDeltaMegabytes);
        }

        var total = GC.GetTotalAllocatedBytes(precise: true) - startAllocated;
        Console.WriteLine();
        Console.WriteLine($"Всего аллоцировано: {total / 1024d / 1024d:F2} МБ");
        Console.WriteLine();

        GcObserver.PrintMemoryInfo("Состояние кучи после экспериментов");
    }
}

public readonly record struct ScenarioResult(string Name, TimeSpan Elapsed, GcDelta Delta);
