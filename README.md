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

## Сборка и запуск

Все команды выполняются из корня репозитория.

```bash
dotnet build src/MemoryLifecycle/MemoryLifecycle.csproj -c Release

dotnet run --project src/MemoryLifecycle -c Release -- gc-pressure
dotnet run --project src/MemoryLifecycle -c Release -- gc-modes
dotnet run --project src/MemoryLifecycle -c Release -- resources
dotnet run --project src/MemoryLifecycle -c Release -- dispose
dotnet run --project src/MemoryLifecycle -c Release -- all
```

Запуск в режиме Server GC (bash):

```bash
DOTNET_gcServer=1 dotnet run --project src/MemoryLifecycle -c Release --no-build -- gc-modes
```

## Результаты

> Таблицы заполняются значениями, полученными на своей машине.

### КТ №40. Поколения и давление на кучу

| Сценарий | Gen0 | Gen1 | Gen2 | Alloc, МБ |
|---|---:|---:|---:|---:|
| Мелкие объекты | | | | |
| Крупные объекты | | | | |
| Удерживаемые объекты | | | | |

### КТ №41. Workstation и Server GC

| Режим | Server GC | Время, мс | Gen0 | Gen2 |
|---|---|---:|---:|---:|
| Workstation | false | | | |
| Server | true | | | |

## Выводы

_Заполняется после выполнения каждого этапа (см. также `docs/`)._

## История коммитов

| № | Сообщение | Содержание |
|---|---|---|
| 0 | `chore: initialize .NET 10 console project` | решение, проект, .gitignore, README |
| 1 | `feat(gc): add generations and pressure measurement` | КТ №40 |
| 2 | `feat(gc): compare workstation and server gc modes` | КТ №41 |
| 3 | `feat(resources): add finalizer and IDisposable examples` | КТ №42 |
| 4 | `feat(dispose): implement full dispose pattern and IAsyncDisposable` | КТ №43 |
| 5 | `docs: add measurement results and conclusions` | README, docs, выводы |

Полная история: `git log --oneline --graph`.

## Автор

Фамилия Имя, группа ____.
