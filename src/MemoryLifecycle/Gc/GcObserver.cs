namespace MemoryLifecycle.Gc;

/// <summary>
/// Вспомогательный вывод состояния кучи по данным GC.GetGCMemoryInfo().
/// </summary>
public static class GcObserver
{
    public static void PrintMemoryInfo(string label)
    {
        // GetGCMemoryInfo описывает кучу на момент последней завершённой сборки.
        // Если сборок ещё не было, HeapSizeBytes и TotalCommittedBytes равны 0 — это не ошибка.
        var info = GC.GetGCMemoryInfo();

        Console.WriteLine($"{label}:");
        Console.WriteLine($"  HeapSizeBytes:       {ToMegabytes(info.HeapSizeBytes):F2} МБ");
        Console.WriteLine($"  TotalCommittedBytes: {ToMegabytes(info.TotalCommittedBytes):F2} МБ");
        Console.WriteLine($"  MemoryLoadBytes:     {ToMegabytes(info.MemoryLoadBytes):F2} МБ");
        Console.WriteLine($"  PauseTimePercentage: {info.PauseTimePercentage:F2} %");
        Console.WriteLine($"  Режим GC:            {(System.Runtime.GCSettings.IsServerGC ? "Server" : "Workstation")}");
    }

    public static double ToMegabytes(long bytes) => bytes / 1024d / 1024d;
}
