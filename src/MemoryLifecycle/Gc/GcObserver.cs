namespace MemoryLifecycle.Gc;

public static class GcObserver
{
    public static void PrintMemoryInfo(string label)
    {
        var info = GC.GetGCMemoryInfo();

        Console.WriteLine($"{label}:");
        Console.WriteLine($"  HeapSizeBytes:       {ToMegabytes(info.HeapSizeBytes):F2} МБ");
        Console.WriteLine($"  TotalCommittedBytes: {ToMegabytes(info.TotalCommittedBytes):F2} МБ");
        Console.WriteLine($"  MemoryLoadBytes:     {ToMegabytes(info.MemoryLoadBytes):F2} МБ");
        Console.WriteLine($"  PauseTimePercentage: {info.PauseTimePercentage:F2} %");
    }

    public static double ToMegabytes(long bytes) => bytes / 1024d / 1024d;
}
