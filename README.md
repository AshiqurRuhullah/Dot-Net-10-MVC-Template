# EmployeeManagement — ASP.NET Core MVC Layered Architecture Template (.NET 10)

[![.NET](https://img.shields.io/badge/.NET-10.0%20LTS-512BD4?logo=dotnet&logoColor=white)](https://dotnet.microsoft.com/download/dotnet/10.0)
[![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91?logo=dotnet&logoColor=white)](https://learn.microsoft.com/aspnet/core/mvc/overview)
[![EF Core](https://img.shields.io/badge/EF%20Core-10.x-68217A)](https://learn.microsoft.com/ef/core/)
[![SQL Server](https://img.shields.io/badge/SQL%20Server-LocalDB%20%2F%20Express-CC2927?logo=microsoftsqlserver&logoColor=white)](https://www.microsoft.com/sql-server/sql-server-downloads)
[![License: MIT](https://img.shields.io/badge/License-MIT-green.svg)](LICENSE)

A clean, 4-project **layered architecture boilerplate** for ASP.NET Core MVC on .NET 10 with Entity Framework Core (code-first) and SQL Server. Ships with a complete **Employee CRUD** as a working reference implementation so you can clone, rename, and start building.

Use this repository as a GitHub template: click **Use this template** → **Create a new repository**.

---

## Table of Contents

- [Why this template](#why-this-template)
- [Architecture](#architecture)
- [Tech stack](#tech-stack)
- [Solution structure](#solution-structure)
- [Getting started](#getting-started)
- [Database setup](#database-setup)
- [Run and test](#run-and-test)
- [How to add a new entity](#how-to-add-a-new-entity)
- [Conventions](#conventions)
- [Troubleshooting](#troubleshooting)
- [Roadmap](#roadmap)
- [Contributing](#contributing)
- [License](#license)
- [Author](#author)

---

## Why this template

| Problem with a single-project MVC app | How this template solves it |
|---|---|
| Controllers talk to `DbContext` directly | Controllers call a **service interface**; data access is hidden behind a **repository interface** |
| Business rules scattered across controllers | Rules live in one place: the **Business** layer (e.g. unique email) |
| Hard to unit test | Every layer depends on an interface, so repositories and services can be mocked |
| Hard to swap the database | Only the **DataAccess** project references EF Core and SQL Server |

Each layer calls **only the layer directly below it**. Nothing skips a layer.

---

## Architecture

```
 Browser
   │  HTTP
   ▼
┌───────────────────────────────┐       ┌──────────────────────────────┐
│ EmployeeManagement.Web        │       │ EmployeeManagement.Entities  │
│ Presentation layer            │──────▶│ POCO entity classes          │
│ Controllers, Razor views,     │ uses  │ Depends on nothing           │
│ Program.cs, appsettings.json  │       │ Referenced by all layers     │
└───────────────────────────────┘       └──────────────────────────────┘
   │  IEmployeeService                                  ▲
   ▼                                                    │
┌───────────────────────────────┐                       │
│ EmployeeManagement.Business   │───────────────────────┤
│ Business layer                │                       │
│ Services + business rules     │                       │
│ No SQL, no HTTP               │                       │
└───────────────────────────────┘                       │
   │  IEmployeeRepository                               │
   ▼                                                    │
┌───────────────────────────────┐                       │
│ EmployeeManagement.DataAccess │───────────────────────┘
│ Data access layer             │
│ AppDbContext, repositories,   │
│ EF Core migrations            │
└───────────────────────────────┘
   │  SQL (connection string)
   ▼
 SQL Server — EmployeeManagementDb (dbo.Employees)
```

### Project references

| Project | References |
|---|---|
| `EmployeeManagement.Entities` | — |
| `EmployeeManagement.DataAccess` | `Entities` |
| `EmployeeManagement.Business` | `Entities`, `DataAccess` |
| `EmployeeManagement.Web` | `Entities`, `DataAccess`, `Business` |

> `Web` references `DataAccess` **only** so `Program.cs` can register `AppDbContext` and the repository. Controllers never call a repository directly.

### Dependency injection (`Program.cs`)

```csharp
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

builder.Services.AddScoped<IEmployeeRepository, EmployeeRepository>();
builder.Services.AddScoped<IEmployeeService, EmployeeService>();
```

---

## Tech stack

| Component | Choice | Notes |
|---|---|---|
| Runtime | .NET 10 (LTS) | .NET 8 (LTS) also works — match the EF Core major version |
| Web framework | ASP.NET Core MVC | Razor views, tag helpers, Bootstrap 5 (default template) |
| ORM | Entity Framework Core 10.x | Code-first with migrations |
| Database | SQL Server (LocalDB by default) | SQL Server Express / full SQL Server supported via connection string |
| IDE | Visual Studio 2026 | Visual Studio 2022 for .NET 8 |

NuGet packages:

| Project | Package |
|---|---|
| `EmployeeManagement.DataAccess` | `Microsoft.EntityFrameworkCore.SqlServer` |
| `EmployeeManagement.Web` | `Microsoft.EntityFrameworkCore.Tools` |

---

## Solution structure

```
EmployeeManagement.sln
│
├── EmployeeManagement.Entities/
│   └── Employee.cs                        # Entity with data annotations
│
├── EmployeeManagement.DataAccess/
│   ├── Data/
│   │   └── AppDbContext.cs                # DbSet<Employee>, unique email index, decimal precision
│   ├── Repositories/
│   │   ├── IEmployeeRepository.cs
│   │   └── EmployeeRepository.cs          # GetAll, GetById, EmailExists, Add, Update, Delete
│   └── Migrations/                        # Generated by Add-Migration
│
├── EmployeeManagement.Business/
│   └── Services/
│       ├── IEmployeeService.cs
│       └── EmployeeService.cs             # Business rules (unique email), returns error message or null
│
└── EmployeeManagement.Web/                # Startup project
    ├── appsettings.json                   # DefaultConnection
    ├── Program.cs                         # DI registration
    ├── Controllers/
    │   └── EmployeesController.cs         # Index, Details, Create, Edit, Delete
    └── Views/
        ├── _ViewImports.cshtml            # @using EmployeeManagement.Entities
        ├── Shared/_Layout.cshtml          # "Employees" nav link
        └── Employees/
            ├── Index.cshtml
            ├── _EmployeeForm.cshtml       # Shared partial for Create/Edit
            ├── Create.cshtml
            ├── Edit.cshtml
            ├── Details.cshtml
            └── Delete.cshtml
```

---

## Getting started

### Prerequisites

- **Visual Studio 2026** with the **ASP.NET and web development** workload
  (or Visual Studio 2022 + .NET 8 SDK)
- **SQL Server Express LocalDB** (installed with the workload — tick it under *Individual components*)
  *or* SQL Server Express / SQL Server with **SQL Server Management Studio** (optional)

### 1. Clone

```bash
git clone https://github.com/<your-username>/<repo-name>.git
cd <repo-name>
```

### 2. Open the solution

Open `EmployeeManagement.sln` in Visual Studio. Confirm **EmployeeManagement.Web** is the startup project (shown in bold; otherwise right-click → *Set as Startup Project*).

### 3. Configure the connection string

Edit `EmployeeManagement.Web/appsettings.json`. Backslashes are doubled because the file is JSON.

**LocalDB (default — no changes needed):**

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=(localdb)\\MSSQLLocalDB;Database=EmployeeManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true"
}
```

**SQL Server Express (Windows auth):**

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EmployeeManagementDb;Trusted_Connection=True;TrustServerCertificate=True"
```

**SQL login:**

```json
"DefaultConnection": "Server=localhost\\SQLEXPRESS;Database=EmployeeManagementDb;User Id=YOUR_USER;Password=YOUR_PASSWORD;TrustServerCertificate=True"
```

> `TrustServerCertificate=True` is for local development only. Never commit real production credentials — use user secrets or environment variables.

### 4. Restore and build

```
Build → Build Solution   (Ctrl+Shift+B)
```

---

## Database setup

The database is created from the C# model — no hand-written SQL.

1. Open **Tools → NuGet Package Manager → Package Manager Console**.
2. Set **Default project** to `EmployeeManagement.DataAccess`.
3. Run:

```powershell
Add-Migration InitialCreate
Update-Database
```

Or name both projects explicitly:

```powershell
Add-Migration InitialCreate -Project EmployeeManagement.DataAccess -StartupProject EmployeeManagement.Web
Update-Database -Project EmployeeManagement.DataAccess -StartupProject EmployeeManagement.Web
```

Using the .NET CLI instead:

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add InitialCreate --project EmployeeManagement.DataAccess --startup-project EmployeeManagement.Web
dotnet ef database update --project EmployeeManagement.DataAccess --startup-project EmployeeManagement.Web
```

This creates `EmployeeManagementDb`, the `dbo.Employees` table and `__EFMigrationsHistory`.

Verify in **View → SQL Server Object Explorer** → `(localdb)\MSSQLLocalDB` → Databases → `EmployeeManagementDb` → Tables.

### Expected schema — `dbo.Employees`

| Column | Type | Nullable |
|---|---|---|
| `Id` | `int` identity, PK | No |
| `FullName` | `nvarchar(100)` | No |
| `Email` | `nvarchar(150)`, unique index | No |
| `Phone` | `nvarchar(20)` | Yes |
| `Department` | `nvarchar(100)` | No |
| `Designation` | `nvarchar(100)` | No |
| `Salary` | `decimal(18,2)` | No |
| `JoiningDate` | `datetime2` | No |
| `IsActive` | `bit` | No |

---

## Run and test

Press **Ctrl+F5** (or **F5** with the debugger). Accept the development SSL certificate prompt on first run, then click **Employees** in the top menu.

| Route | Verb | Action | Result |
|---|---|---|---|
| `/Employees` | GET | `Index` | Employee list |
| `/Employees/Details/{id}` | GET | `Details` | Read-only view |
| `/Employees/Create` | GET / POST | `Create` | Form → validate → insert |
| `/Employees/Edit/{id}` | GET / POST | `Edit` | Form → validate → update |
| `/Employees/Delete/{id}` | GET / POST | `Delete` / `DeleteConfirmed` | Confirm → delete |

### Smoke test checklist

- [ ] Empty list shows *"No employees yet"*
- [ ] Create with valid data → success message, one row
- [ ] Submit empty form → required-field errors, nothing saved
- [ ] Duplicate email → *"Another employee already uses this email."*
- [ ] Details page shows all fields
- [ ] Edit → updated values on the list
- [ ] Delete → confirmation page → row removed
- [ ] Row visible in SQL Server Object Explorer

All POST actions are protected with `[ValidateAntiForgeryToken]`.

---

## How to add a new entity

Example: adding `Department`.

1. **Entities** — create `Department.cs` with data annotations.
2. **DataAccess** — add `DbSet<Department>` to `AppDbContext`; create `IDepartmentRepository` / `DepartmentRepository`.
3. **Business** — create `IDepartmentService` / `DepartmentService` with any rules.
4. **Web** — register both in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();
   builder.Services.AddScoped<IDepartmentService, DepartmentService>();
   ```
5. **Web** — add `DepartmentsController` and `Views/Departments/*.cshtml`.
6. Migrate:
   ```powershell
   Add-Migration AddDepartment
   Update-Database
   ```

---

## Conventions

- **Services return `string?`** — `null` on success, an error message when a business rule fails. The controller maps it to `ModelState`.
- **Repositories return `Task`** and call `SaveChangesAsync()` themselves. Nothing is persisted until then.
- **Read queries use `AsNoTracking()`** for list operations.
- **Scoped lifetime** for `DbContext`, repositories and services — one instance per HTTP request.
- **Namespaces** mirror folders: `EmployeeManagement.DataAccess.Repositories`, `EmployeeManagement.Business.Services`, etc.
- Renaming the solution? Update every namespace to match.

---

## Troubleshooting

| Symptom | Likely cause | Fix |
|---|---|---|
| `Add-Migration` is not recognized | Tools package missing or not loaded | Install `Microsoft.EntityFrameworkCore.Tools` in `Web`, restart VS |
| No DbContext was found | Wrong *Default project* in PMC | Set it to `EmployeeManagement.DataAccess` |
| Unable to create a DbContext | Wrong startup project or `AddDbContext` missing | Set `Web` as startup; check `Program.cs` |
| NU1902 vulnerability warning stops `Add-Migration` | Transitive package flagged | Update EF Core packages to the latest stable, or temporarily set `<NuGetAudit>false</NuGetAudit>` in the `.csproj` |
| Migrations assembly error | Migrations expected in a different project | Pass `-Project EmployeeManagement.DataAccess` explicitly |
| A network-related or instance-specific error | Wrong `Server=` or SQL Server not running | Check instance name; start the `SQL Server (SQLEXPRESS)` service |
| Certificate chain not trusted | Self-signed cert on local server | Add `TrustServerCertificate=True` |
| Cannot open database / invalid object name `Employees` | Migration not applied | Run `Update-Database` |
| Unable to resolve service `IEmployeeService` | DI registration missing | Add `AddScoped` lines to `Program.cs` |
| The view "Index" was not found | Folder/file naming | Must be `Views/Employees/Index.cshtml` |
| 404 at root URL | Default route points to `Home` | Navigate to `/Employees` or change the default route in `Program.cs` |
| HTTP 400 on form submit | Anti-forgery token missing | Use `<form asp-action="..." method="post">` tag helper |

---

## Roadmap

- [ ] Unit test project (xUnit + Moq) for `Business` and `DataAccess`
- [ ] Pagination, search and sorting on the list page
- [ ] ViewModels / DTOs instead of passing entities to views
- [ ] Global exception handling and logging (Serilog)
- [ ] Authentication (ASP.NET Core Identity)
- [ ] Docker Compose with SQL Server container
- [ ] GitHub Actions CI (build + test)

---

## Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "Add your feature"`
4. Push the branch: `git push origin feature/your-feature`
5. Open a pull request

---

## License

This project is licensed under the MIT License — see the [LICENSE](LICENSE) file for details.

---

## Author

**H.N. ASHIQUR RUHULLAH**
Software Engineer
ERA Infotech Limited, Dhaka, Bangladesh

[![LinkedIn](https://img.shields.io/badge/LinkedIn-0A66C2?logo=linkedin&logoColor=white)](https://www.linkedin.com/in/ashiqurruhullah)
[![GitHub](https://img.shields.io/badge/GitHub-181717?logo=github&logoColor=white)](https://github.com/AshiqurRuhullah)
[![Email](https://img.shields.io/badge/Email-D14836?logo=gmail&logoColor=white)](mailto:YOUR-EMAIL@example.com)

If this template saved you time, consider giving it a ⭐.
