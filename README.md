<div align="center">

# Equipment Inventory System

**A desktop application for full-cycle equipment asset management**

![Platform](https://img.shields.io/badge/platform-Windows-0078D7?style=flat-square&logo=windows)
![Framework](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)
![UI](https://img.shields.io/badge/UI-WPF-68217A?style=flat-square)
![Database](https://img.shields.io/badge/database-SQLite-003B57?style=flat-square&logo=sqlite)
![Pattern](https://img.shields.io/badge/pattern-MVVM%20%2B%20Repository-4CAF50?style=flat-square)
![License](https://img.shields.io/badge/license-MIT-blue?style=flat-square)

</div>

---

## Overview

**Equipment Inventory System** is a Windows desktop application that provides centralized control over an organization's physical assets. It covers the complete asset lifecycle — from initial registration and assignment to periodic inventory audits and movement tracking.

The system is designed for IT departments, facility managers, and operations teams that need a reliable, offline-capable tool to maintain asset accountability without the overhead of a web-based solution.

---

## Core Capabilities

- **Equipment registry** — full CRUD with status tracking, room assignment, and responsible-employee linkage
- **Employee & room directories** — searchable reference data used across all modules
- **Inventory checks** — structured audits with per-item results (found / not found / requires clarification)
- **Movement history** — immutable audit log automatically updated on every relocation or reassignment
- **Reports & CSV export** — four report types covering the complete asset picture
- **Automatic database initialization** — schema and seed data created on first launch; no manual setup required

---

## Technology Stack

| Layer | Technology |
|---|---|
| Platform | Windows (WPF, .NET 8) |
| UI Framework | WPF (XAML) |
| Database | SQLite via `Microsoft.Data.Sqlite` |
| Architecture | MVVM + Repository pattern |
| Language | C# 12, Nullable enabled |

---

## Project Structure

```
EquipmentInventorySystem/
├── EquipmentInventorySystem.sln
├── docs/
│   └── functional-description.md
└── EquipmentInventorySystem/
    ├── App.xaml / App.xaml.cs        ← startup & DB initialization
    ├── MainWindow.xaml               ← shell with sidebar navigation
    ├── Models/                       ← domain entities & display rows (17 files)
    ├── ViewModels/                   ← 7 ViewModels (one per section)
    ├── Views/                        ← pages (UserControls) + modal windows
    ├── Services/                     ← business logic layer
    ├── Data/
    │   ├── DatabaseHelper.cs
    │   ├── DatabaseInitializer.cs
    │   ├── Repositories/             ← 5 repositories
    │   └── Scripts/
    │       ├── init.sql              ← schema definition
    │       └── seed.sql              ← demo data
    └── EquipmentInventorySystem.csproj
```

---

## Modules

### Equipment
Maintains the central asset registry. Each record includes name, inventory number, serial number, status (in use / in storage / decommissioned / under repair), room, responsible employee, and notes. Full add / edit / delete / search support.

### Employees
Reference directory of personnel. Stores name, position, and department. Employees can be assigned as responsible parties for one or more assets.

### Rooms
Reference directory of physical locations. Each room has a number, name, and optional description. Used to track where each asset is currently located.

### Inventory Checks
Structured audit module. Create a check with a date and responsible employee, then record a result (found / not found / requires clarification) for every asset in the registry. All historical checks remain available for review.

### Movement History
Read-only audit log. Automatically captures every change of room or responsible employee — whether made through the equipment form or the dedicated Transfer dialog. Provides a full accountability chain.

### Reports
Four built-in reports with CSV export:
- **All equipment** — complete registry with current status
- **By room** — asset count per location
- **By employee** — asset count per responsible person
- **Inventory results** — detailed outcome of any selected audit

---

## Screenshots

> Screenshots will be added in a future update. To preview the UI, clone the repository and run the application — demo data is loaded automatically on first launch.

Planned captures:
- Dashboard (home screen with statistics)
- Equipment list and add/edit dialog
- Transfer equipment dialog
- Inventory check creation and results view
- Movement history journal
- Reports with CSV export

---

## Getting Started

### Prerequisites

- Windows 10 / 11
- [.NET 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0) (or SDK for development)

### Run from source

```bash
git clone https://github.com/Saitama4722/EquipmentInventorySystem.git
cd EquipmentInventorySystem
dotnet run --project EquipmentInventorySystem/EquipmentInventorySystem.csproj
```

### Build Release

```bash
dotnet build --configuration Release
```

The compiled executable will be placed in `EquipmentInventorySystem/bin/Release/net8.0-windows/`.

---

## Database

The application uses **SQLite** as its embedded database engine. On first launch, `DatabaseInitializer` automatically:

1. Creates `inventory.db` in the application directory
2. Executes `init.sql` to build the schema
3. Executes `seed.sql` to populate demo data

No external database server, no connection strings, no manual configuration. The database file is excluded from version control via `.gitignore`; each installation maintains its own local data.

---

## Contact

Questions, feedback, or collaboration inquiries:

**Telegram:** [@VadikQA](https://t.me/VadikQA)

---

<div align="center">

*WPF · C# · SQLite · .NET 8 · desktop application · inventory management · equipment inventory system · asset tracking · inventory checks · equipment management · reporting system · Windows desktop app*

</div>

---
---

<div align="center">

# Система учёта оборудования

**Настольное приложение для управления материально-техническими активами**

</div>

---

## Обзор

**Equipment Inventory System** — настольное приложение для Windows, обеспечивающее централизованный учёт физических активов организации. Система охватывает полный жизненный цикл оборудования: от регистрации и назначения до проведения инвентаризаций и отслеживания перемещений.

Решение ориентировано на ИТ-отделы, хозяйственные службы и подразделения обеспечения, которым нужен надёжный автономный инструмент для учёта имущества без зависимости от веб-инфраструктуры.

---

## Ключевые возможности

- **Реестр оборудования** — полный CRUD со статусами, привязкой к помещению и ответственному сотруднику
- **Справочники сотрудников и помещений** — поддерживают поиск, используются во всех модулях
- **Инвентаризации** — структурированные проверки с результатом по каждой позиции
- **История перемещений** — неизменяемый журнал, обновляемый автоматически
- **Отчёты и экспорт в CSV** — четыре вида сводных отчётов
- **Автоматическая инициализация БД** — схема и демонстрационные данные создаются при первом запуске

---

## Стек технологий

| Уровень | Технология |
|---|---|
| Платформа | Windows (WPF, .NET 8) |
| Интерфейс | WPF (XAML) |
| База данных | SQLite (`Microsoft.Data.Sqlite`) |
| Архитектура | MVVM + Repository |
| Язык | C# 12, Nullable enabled |

---

## Структура проекта

```
EquipmentInventorySystem/
├── EquipmentInventorySystem.sln
├── docs/
│   └── functional-description.md
└── EquipmentInventorySystem/
    ├── App.xaml / App.xaml.cs        ← точка входа и инициализация БД
    ├── MainWindow.xaml               ← оболочка с боковой навигацией
    ├── Models/                       ← доменные модели и строки отображения
    ├── ViewModels/                   ← 7 ViewModel (по одному на раздел)
    ├── Views/                        ← страницы (UserControl) и диалоги
    ├── Services/                     ← слой бизнес-логики
    ├── Data/
    │   ├── DatabaseHelper.cs
    │   ├── DatabaseInitializer.cs
    │   ├── Repositories/             ← 5 репозиториев
    │   └── Scripts/
    │       ├── init.sql              ← схема БД
    │       └── seed.sql              ← демонстрационные данные
    └── EquipmentInventorySystem.csproj
```

---

## Модули системы

### Оборудование
Центральный реестр активов. Каждая запись содержит наименование, инвентарный номер, серийный номер, статус (в эксплуатации / на хранении / списывается / в ремонте), помещение, ответственного сотрудника и примечания. Поддерживаются добавление, редактирование, удаление и поиск.

### Сотрудники
Справочник персонала. Хранит ФИО, должность и отдел. Сотрудник может быть назначен ответственным за одну или несколько единиц оборудования.

### Помещения
Справочник мест размещения. Каждое помещение имеет номер, наименование и описание. Используется для учёта фактического местонахождения оборудования.

### Инвентаризации
Модуль плановых проверок. Позволяет создать инвентаризацию с датой и ответственным, а затем зафиксировать результат проверки (обнаружено / не обнаружено / требует уточнения) для каждой единицы реестра. Все проведённые инвентаризации доступны для просмотра.

### История перемещений
Журнал доступен только для чтения. Автоматически регистрирует каждое изменение помещения или ответственного — как при редактировании карточки оборудования, так и через диалог «Перемещение». Обеспечивает полный аудиторский след.

### Отчёты
Четыре встроенных отчёта с экспортом в CSV:
- **Всё оборудование** — полный реестр с текущим статусом
- **По помещениям** — количество активов в каждом помещении
- **По сотрудникам** — количество активов на каждого ответственного
- **Результаты инвентаризации** — детальные итоги выбранной проверки

---

## Скриншоты

> Скриншоты будут добавлены в следующем обновлении. Для предварительного просмотра интерфейса — клонируйте репозиторий и запустите приложение: при первом запуске автоматически загружаются демонстрационные данные.

---

## Быстрый старт

### Требования

- Windows 10 / 11
- [.NET 8 Runtime](https://dotnet.microsoft.com/download/dotnet/8.0)

### Запуск из исходного кода

```bash
git clone https://github.com/Saitama4722/EquipmentInventorySystem.git
cd EquipmentInventorySystem
dotnet run --project EquipmentInventorySystem/EquipmentInventorySystem.csproj
```

### Сборка Release-версии

```bash
dotnet build --configuration Release
```

Скомпилированный исполняемый файл будет находиться в `EquipmentInventorySystem/bin/Release/net8.0-windows/`.

---

## База данных

Приложение использует встроенную СУБД **SQLite**. При первом запуске `DatabaseInitializer` автоматически:

1. Создаёт файл `inventory.db` в директории приложения
2. Выполняет `init.sql` для построения схемы
3. Выполняет `seed.sql` для наполнения демонстрационными данными

Внешний сервер БД не требуется. Файл базы данных исключён из системы контроля версий через `.gitignore`.

---

## Практические задачи, решаемые системой

1. Централизованное хранение сведений об оборудовании организации
2. Контроль местонахождения и персональной ответственности за каждую единицу имущества
3. Документирование перемещений для целей внутреннего аудита
4. Проведение инвентаризаций с фиксацией результатов проверки
5. Формирование сводной отчётности для руководства и бухгалтерии

---

## Контакты

По вопросам и предложениям:

**Telegram:** [@VadikQA](https://t.me/VadikQA)

---

<div align="center">

*WPF · C# · SQLite · .NET 8 · настольное приложение · учёт оборудования · система инвентаризации · отслеживание активов · управление имуществом · Windows*

</div>
