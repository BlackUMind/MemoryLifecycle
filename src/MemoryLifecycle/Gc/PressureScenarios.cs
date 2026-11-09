using System.Diagnostics;

namespace MemoryLifecycle.Gc;

/// <summary>
/// КТ №40, вариант 2 «Измерение давления».
/// Три сценария с разным характером нагрузки на управляемую кучу
/// и контрольная принудительная сборка Gen2.
/// Счётчики снимаются до и после КАЖДОГО сценария, поэтому эффекты не смешиваются.
/// </summary>
public static class PressureScenarios
{
    private const int SmallObjectCount = 100_000;
    private const int SmallObjectSize = 100;

    private const int LargeObjectCount = 10;
    private const int LargeObjectSize = 100_000; // больше порога LOH (85 000 байт)

    private const int LongLivedCount = 20_000;
    private const int LongLivedSize = 512;

    /// <summary>
    /// Долгоживущие данные: статический список является корнем GC,
    /// поэтому всё, что в него попало, остаётся достижимым до конца процесса.
    /// </summary>
    private static readonly List<byte[]> LongLivedStorage = new();

    /// <summary>
    /// Сценарий 1. Много мелких короткоживущих объектов (byte[100] × 100 000).
    ///
    /// Почему растёт Gen0: каждый массив размещается в Gen0 и сразу становится мусором.
    /// Когда исчерпывается бюджет Gen0, запускается дешёвая эфемерная сборка;
    /// почти все объекты к этому моменту мертвы, поэтому до Gen1/Gen2 никто не доживает.
    /// Почему Gen2 почти не меняется: выживших нет, продвигать в старшие поколения нечего
    /// (гипотеза о смертности молодых объектов).
    /// </summary>
    public static ScenarioResult RunSmallObjects()
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        // checksum не даёт JIT выбросить аллокации как неиспользуемые.
        long checksum = 0;
        for (var i = 0; i < SmallObjectCount; i++)
        {
            var buffer = new byte[SmallObjectSize];
            buffer[0] = (byte)i;
            checksum += buffer[0];
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();
        GC.KeepAlive(checksum);

        return new ScenarioResult(
            $"Мелкие объекты ({SmallObjectCount:N0} × {SmallObjectSize} Б)",
            stopwatch.Elapsed, after.Diff(before));
    }

    /// <summary>
    /// Сценарий 2. Несколько крупных объектов (byte[100 000] × 10).
    ///
    /// Почему Gen0 не растёт: объекты ≥ 85 000 байт сразу размещаются в LOH,
    /// минуя Gen0, и не расходуют его бюджет.
    /// Почему Gen2 может не измениться: LOH логически относится к Gen2, но счётчик
    /// CollectionCount(2) растёт только при СБОРКЕ, а не при аллокации.
    /// ~1 МБ аллокаций обычно не исчерпывает бюджет LOH, поэтому сборка не запускается.
    /// При этом объём аллокаций заметен: 10 объектов дают почти столько же байт,
    /// сколько 10 000 мелких.
    /// </summary>
    public static ScenarioResult RunLargeObjects()
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        var retained = new List<byte[]>(LargeObjectCount);
        for (var i = 0; i < LargeObjectCount; i++)
        {
            retained.Add(new byte[LargeObjectSize]);
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();
        GC.KeepAlive(retained);

        return new ScenarioResult(
            $"Крупные объекты ({LargeObjectCount} × {LargeObjectSize:N0} Б)",
            stopwatch.Elapsed, after.Diff(before));
    }

    /// <summary>
    /// Сценарий 3. Удержание долгоживущих объектов в статическом списке (byte[512] × 20 000).
    ///
    /// Gen0 может вырасти: объекты, как и в сценарии 1, рождаются в Gen0 и расходуют
    /// его бюджет (~10 МБ). Разница в том, что при эфемерной сборке они ЖИВЫ — сборщик
    /// не отбрасывает их, а продвигает в Gen1, поэтому такая сборка дороже.
    /// Почему Gen2 обычно не меняется сразу: объекты сначала живут в Gen0/Gen1 и попадают
    /// в Gen2 только после того, как переживут две сборки. Рост Gen2 и HeapSizeBytes
    /// виден позже — после следующей сборки (см. сценарий 4).
    /// </summary>
    public static ScenarioResult RunLongLived()
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        for (var i = 0; i < LongLivedCount; i++)
        {
            LongLivedStorage.Add(new byte[LongLivedSize]);
        }

        stopwatch.Stop();
        var after = GcSnapshot.Capture();

        return new ScenarioResult(
            $"Удерживаемые объекты ({LongLivedCount:N0} × {LongLivedSize} Б)",
            stopwatch.Elapsed, after.Diff(before));
    }

    /// <summary>
    /// Сценарий 4 (контрольный). Принудительная блокирующая сборка Gen2.
    ///
    /// Полная сборка учитывается и как сборка младших поколений, поэтому все три
    /// счётчика растут на единицу. Аллокаций нет, зато HeapSizeBytes обновляется:
    /// теперь в нём учтены удерживаемые объекты из сценария 3.
    /// В рабочем коде GC.Collect не вызывают — здесь он нужен только для наблюдения.
    /// </summary>
    public static ScenarioResult RunForcedGen2()
    {
        var before = GcSnapshot.Capture();
        var stopwatch = Stopwatch.StartNew();

        GC.Collect(2, GCCollectionMode.Forced, blocking: true);

        stopwatch.Stop();
        var after = GcSnapshot.Capture();

        return new ScenarioResult("Принудительный Gen2 (blocking)",
            stopwatch.Elapsed, after.Diff(before));
    }

    public static void RunAll()
    {
        // Шаг 1 задания: зафиксировать общий объём аллокаций в начале работы.
        var allocatedAtStart = GC.GetTotalAllocatedBytes(precise: true);

        Console.WriteLine("КТ №40. Сценарии давления на GC (вариант 2)");
        Console.WriteLine();
        GcObserver.PrintMemoryInfo("Состояние кучи до экспериментов");
        Console.WriteLine();

        var line = new string('-', 96);
        Console.WriteLine(line);
        Console.WriteLine(
            "{0,-38}{1,10}{2,6}{3,6}{4,6}{5,12}{6,12}",
            "Сценарий", "Время, мс", "Gen0", "Gen1", "Gen2", "Alloc, МБ", "Heap Δ, МБ");
        Console.WriteLine(line);

        // Каждый сценарий — отдельный метод со своими снимками «до/после».
        var results = new[]
        {
            RunSmallObjects(),
            RunLargeObjects(),
            RunLongLived(),
            RunForcedGen2()
        };

        foreach (var result in results)
        {
            Console.WriteLine(
                "{0,-38}{1,10:F2}{2,6}{3,6}{4,6}{5,12:F2}{6,12:F2}",
                result.Name,
                result.Elapsed.TotalMilliseconds,
                result.Delta.Gen0,
                result.Delta.Gen1,
                result.Delta.Gen2,
                result.Delta.AllocatedMegabytes,
                result.Delta.HeapDeltaMegabytes);
        }

        Console.WriteLine(line);

        var allocatedTotal = GC.GetTotalAllocatedBytes(precise: true) - allocatedAtStart;
        Console.WriteLine($"Всего аллоцировано с начала работы: {GcObserver.ToMegabytes(allocatedTotal):F2} МБ");
        Console.WriteLine($"Удерживается в статическом списке:  {LongLivedStorage.Count:N0} объектов");
        Console.WriteLine();

        GcObserver.PrintMemoryInfo("Состояние кучи после экспериментов");
        Console.WriteLine();
        Console.WriteLine(
            "Значения зависят от машины, режима GC и момента запуска. " +
            "Важна не абсолютная величина, а соотношение счётчиков.");
    }
}

/// <summary>
/// Результат одного сценария: название, длительность и прирост счётчиков.
/// </summary>
public readonly record struct ScenarioResult(string Name, TimeSpan Elapsed, GcDelta Delta);
