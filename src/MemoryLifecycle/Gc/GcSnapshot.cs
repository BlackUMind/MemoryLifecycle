namespace MemoryLifecycle.Gc;

/// <summary>
/// Снимок счётчиков сборщика мусора и управляемой кучи в один момент времени.
/// </summary>
public readonly record struct GcSnapshot(
    int Gen0,
    int Gen1,
    int Gen2,
    long AllocatedBytes,
    long HeapSizeBytes)
{
    /// <summary>
    /// Фиксирует текущие значения счётчиков.
    /// </summary>
    public static GcSnapshot Capture() => new(
        GC.CollectionCount(0),
        GC.CollectionCount(1),
        GC.CollectionCount(2),
        // precise: true — точное значение, а не приблизительное,
        // которое может запаздывать на размер текущего allocation context.
        GC.GetTotalAllocatedBytes(precise: true),
        // Размер кучи по данным ПОСЛЕДНЕЙ завершённой сборки (не "прямо сейчас").
        GC.GetGCMemoryInfo().HeapSizeBytes);

    /// <summary>
    /// Прирост показателей относительно более раннего снимка.
    /// </summary>
    public GcDelta Diff(GcSnapshot previous) => new(
        Gen0 - previous.Gen0,
        Gen1 - previous.Gen1,
        Gen2 - previous.Gen2,
        AllocatedBytes - previous.AllocatedBytes,
        HeapSizeBytes - previous.HeapSizeBytes);
}

/// <summary>
/// Прирост показателей между двумя снимками.
/// </summary>
public readonly record struct GcDelta(
    int Gen0,
    int Gen1,
    int Gen2,
    long AllocatedBytes,
    long HeapSizeDelta)
{
    public double AllocatedMegabytes => AllocatedBytes / 1024d / 1024d;
    public double HeapDeltaMegabytes => HeapSizeDelta / 1024d / 1024d;
}
