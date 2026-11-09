# MemoryLifecycle

Консольный проект на .NET 10 для исследования сборки мусора
и управления временем жизни объектов.

Контрольная точка по дисциплине «Язык программирования C#»:
КТ №40–43 (GC, режимы сборки, финализаторы и IDisposable, паттерн Dispose и IAsyncDisposable).

## Стек и окружение

- .NET 10 SDK (проверка: `dotnet --version`)
- C# 14, включённые nullable-ссылочные типы
- Git 2.40+, GitHub
- Дополнительные пакеты не требуются

## Структура

```
MemoryLifecycle/
├── .gitignore
├── README.md
├── MemoryLifecycle.sln
├── docs/
│   ├── 01-gc.md
│   ├── 02-gc-modes.md
│   ├── 03-disposable.md
│   └── 04-dispose-pattern.md
└── src/
    └── MemoryLifecycle/
        ├── MemoryLifecycle.csproj
        ├── Program.cs
        ├── Gc/
        │   ├── GcSnapshot.cs
        │   ├── GcObserver.cs
        │   ├── PressureScenarios.cs
        │   └── GcModeScenarios.cs
        ├── Resources/
        │   ├── FakeResource.cs
        │   ├── ManagedResource.cs
        │   ├── LogFile.cs
        │   ├── DatabaseConnection.cs
        │   └── FileBuffer.cs
        └── Demos/
            ├── ResourceDemos.cs
            └── DisposeDemos.cs
```

- `Gc` — снимки счётчиков и сценарии нагрузки;
- `Resources` — классы, управляющие ресурсами;
- `Demos` — консольные сценарии;
- `docs` — отчёты по темам.

## Запуск

```bash
dotnet run --project src/MemoryLifecycle -c Release -- gc-pressure
```

## Результаты

### КТ №40. Поколения и давление на кучу

Вариант 2 «Измерение давления».

Условия запуска: ОС ____, CPU ____, .NET ____, Workstation GC, Release.

| Сценарий | Время, мс | Gen0 | Gen1 | Gen2 | Alloc, МБ | Heap Δ, МБ |
|---|---:|---:|---:|---:|---:|---:|
| Мелкие объекты (100 000 × 100 Б) | | | | | | |
| Крупные объекты (10 × 100 000 Б) | | | | | | |
| Удерживаемые объекты (20 000 × 512 Б) | | | | | | |
| Принудительный Gen2 | | | | | | |

Выводы:

1. Gen0 нагружают мелкие короткоживущие объекты: они умирают молодыми,
   сборки Gen0 дешёвые, Gen2 не растёт.
2. Крупные объекты (от 85 000 байт) попадают в LOH, минуя Gen0. LOH относится к Gen2,
   но счётчик Gen2 растёт только при сборке, а не при выделении памяти.
3. Долгоживущие объекты из статического списка переживают сборки и уходят в Gen2,
   их вклад в `HeapSizeBytes` виден после следующей сборки.

Подробнее — [docs/01-gc.md](docs/01-gc.md).

## История коммитов

`git log --oneline`

## Автор

