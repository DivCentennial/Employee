# Microservices Framework Creation Guide

## Overview
This guide will help you create a complete microservices framework following the layered architecture pattern used in your Employee microservice project.

## Project Structure
```
YourProject/
├── ApiService/                 # Starting point (Web API)
├── Business/                   # Business logic layer
├── DataCarrier/               # Entity models (for API responses)
├── DataModel/                 # Data transfer objects (for API requests)
├── Repository/                # Data access layer
├── Repository.Contracts/      # Repository interfaces
└── Test/                      # Unit and integration tests
```

## Step-by-Step Implementation

### Step 1: Create Solution and Projects

1. **Open Visual Studio** and create a new solution
2. **Create the following projects** (in this order):

#### 1.1 DataCarrier Project
- **Project Type**: Class Library (.NET 8.0)
- **Name**: `YourProject.DataCarrier`
- **Purpose**: Contains entity classes that represent your database table structure

#### 1.2 DataModel Project
- **Project Type**: Class Library (.NET 8.0)
- **Name**: `YourProject.DataModel`
- **Purpose**: Contains DTOs (Data Transfer Objects) for API requests

#### 1.3 Repository.Contracts Project
- **Project Type**: Class Library (.NET 8.0)
- **Name**: `YourProject.Repository.Contracts`
- **Purpose**: Contains interfaces for repositories and database context

#### 1.4 Repository Project
- **Project Type**: Class Library (.NET 8.0)
- **Name**: `YourProject.Repository`
- **Purpose**: Contains repository implementations and database access logic

#### 1.5 Business Project
- **Project Type**: Class Library (.NET 8.0)
- **Name**: `YourProject.Business`
- **Purpose**: Contains business logic and service implementations

#### 1.6 ApiService Project
- **Project Type**: ASP.NET Core Web API (.NET 8.0)
- **Name**: `YourProject.ApiService`
- **Purpose**: Contains controllers and API endpoints

#### 1.7 Test Project
- **Project Type**: xUnit Test Project (.NET 8.0)
- **Name**: `YourProject.Test`
- **Purpose**: Contains unit and integration tests

### Step 2: Set Up Project References

Add the following project references:

**ApiService** → References:
- Business
- Business.Contracts
- DataCarrier
- DataModel

**Business** → References:
- Business.Contracts
- DataCarrier
- DataModel
- Repository.Contracts

**Repository** → References:
- Repository.Contracts
- DataModel

**Repository.Contracts** → References:
- DataModel

**Test** → References:
- All other projects

### Step 3: Install Required NuGet Packages

#### For ApiService Project:
```xml
<PackageReference Include="Microsoft.AspNetCore.OpenApi" Version="8.0.0" />
<PackageReference Include="Swashbuckle.AspNetCore" Version="6.4.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Serilog.Sinks.File" Version="5.0.0" />
<PackageReference Include="Serilog.Sinks.Console" Version="5.0.0" />
```

#### For Repository Project:
```xml
<PackageReference Include="System.Data.SqlClient" Version="4.8.5" />
<PackageReference Include="Microsoft.Extensions.Configuration" Version="8.0.0" />
<PackageReference Include="Microsoft.Extensions.Logging" Version="8.0.0" />
```

### Step 4: Create Data Models

#### 4.1 DataCarrier/EmployeeEntity.cs
```csharp
using System;

namespace YourProject.DataCarrier
{
    public class EmployeeEntity
    {
        public int Empid { get; set; }
        public string? Ename { get; set; }
        public int Dept_ID { get; set; }
    }
}
```

#### 4.2 DataModel/EmployeeDT.cs
```csharp
using System;

namespace YourProject.DataModel
{
    public class EmployeeDT
    {
        public int Empid { get; set; }
        public string? Ename { get; set; }
        public int Dept_ID { get; set; }
    }
}
```

### Step 5: Create Repository Contracts

#### 5.1 Repository.Contracts/Repositories/IEmployeeRepository.cs
```csharp
using YourProject.DataModel;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace YourProject.Repository.Contracts.Repositories
{
    public interface IEmployeeRepository
    {
        Task<IEnumerable<EmployeeDT>> GetAllEmployeesAsync();
        Task<EmployeeDT?> GetEmployeeByIdAsync(int id);
        Task<int> CreateEmployeeAsync(EmployeeDT emp);
        Task UpdateEmployeeAsync(EmployeeDT emp);
        Task DeleteEmployeeAsync(int id);
    }
}
```

#### 5.2 Repository.Contracts/DbContext/IAdoDbContext.cs
```csharp
using System;

namespace YourProject.Repository.Contracts.DbContext
{
    public interface IAdoDbContext
    {
        // Add your database context methods here
        // For now, this can be empty or contain basic database operations
    }
}
```

### Step 6: Create SQL Named Queries

#### 6.1 Repository/SQLNamedQueries.cs
```csharp
using System;

namespace YourProject.Repository
{
    internal static class SQLNamedQueries
    {
        // Employee related stored procedures
        public const string GetEmp = "getEmp";
        public const string GetEmployeeById = "getEmpById";
        public const string InsertEmployee = "insertEmployee";
        public const string UpdateEmployee = "updateEmployee";
        public const string DeleteEmployee = "deleteEmployee";
    }
}
```

### Step 7: Create Repository Implementation

#### 7.1 Repository/Repositories/EmployeeRepository.cs
```csharp
using YourProject.DataModel;
using YourProject.Repository.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace YourProject.Repository.Repositories
{
    public class EmployeeRepository : IEmployeeRepository
    {
        private readonly string _connectionString;

        public EmployeeRepository(string connectionString)
        {
            _connectionString = connectionString;
        }

        public async Task<IEnumerable<EmployeeDT>> GetAllEmployeesAsync()
        {
            var employees = new List<EmployeeDT>();

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(SQLNamedQueries.GetEmp, conn);
            cmd.CommandType = CommandType.StoredProcedure;

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                employees.Add(new EmployeeDT
                {
                    Empid = reader.GetInt32(reader.GetOrdinal("Empid")),
                    Ename = reader.GetString(reader.GetOrdinal("Ename")),
                    Dept_ID = reader.GetInt32(reader.GetOrdinal("DeptID"))
                });
            }

            return employees;
        }

        public async Task<EmployeeDT?> GetEmployeeByIdAsync(int id)
        {
            EmployeeDT? employee = null;

            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(SQLNamedQueries.GetEmployeeById, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@EmpID", id);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                employee = new EmployeeDT
                {
                    Empid = reader.GetInt32(reader.GetOrdinal("Empid")),
                    Ename = reader.GetString(reader.GetOrdinal("Emp_name")),
                    Dept_ID = reader.GetInt32(reader.GetOrdinal("DeptID"))
                };
            }

            return employee;
        }

        public async Task<int> CreateEmployeeAsync(EmployeeDT emp)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(SQLNamedQueries.InsertEmployee, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Empid", emp.Empid);
            cmd.Parameters.AddWithValue("@Ename", emp.Ename);
            cmd.Parameters.AddWithValue("@DeptID", emp.Dept_ID);

            var newId = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(newId);
        }

        public async Task UpdateEmployeeAsync(EmployeeDT emp)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(SQLNamedQueries.UpdateEmployee, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Empid", emp.Empid);
            cmd.Parameters.AddWithValue("@Ename", emp.Ename);
            cmd.Parameters.AddWithValue("@DeptID", emp.Dept_ID);

            await cmd.ExecuteNonQueryAsync();
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            using var conn = new SqlConnection(_connectionString);
            await conn.OpenAsync();

            using var cmd = new SqlCommand(SQLNamedQueries.DeleteEmployee, conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Empid", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
```

### Step 8: Create Business Layer

#### 8.1 Business/Contracts/IEmployeeService.cs
```csharp
using YourProject.DataCarrier;
using YourProject.DataModel;
using System.Threading.Tasks;

namespace YourProject.Business.Contracts
{
    public interface IEmployeeService
    {
        Task<EmployeeEntity[]> GetAllEmployeesAsync();
        Task<EmployeeEntity> GetEmployeeByIdAsync(int id);
        Task CreateEmployeeAsync(EmployeeDT emp);
        Task UpdateEmployeeAsync(EmployeeDT emp);
        Task DeleteEmployeeAsync(int id);
    }
}
```

#### 8.2 Business/EmployeeService.cs
```csharp
using YourProject.Business.Contracts;
using YourProject.DataCarrier;
using YourProject.DataModel;
using YourProject.Repository.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace YourProject.Business
{
    public class EmployeeService : IEmployeeService
    {
        private readonly IEmployeeRepository _repository;
        private readonly ILogger<EmployeeService> _logger;

        public EmployeeService(IEmployeeRepository repository, ILogger<EmployeeService> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        private EmployeeEntity MapToEntity(EmployeeDT dt)
        {
            return new EmployeeEntity
            {
                Empid = dt.Empid,
                Ename = dt.Ename,
                Dept_ID = dt.Dept_ID
            };
        }

        public async Task<EmployeeEntity[]> GetAllEmployeesAsync()
        {
            var dtList = await _repository.GetAllEmployeesAsync();
            return dtList.Select(MapToEntity).ToArray();
        }

        public async Task<EmployeeEntity> GetEmployeeByIdAsync(int id)
        {
            var dt = await _repository.GetEmployeeByIdAsync(id);
            return dt == null ? null : MapToEntity(dt);
        }

        public async Task CreateEmployeeAsync(EmployeeDT emp)
        {
            await _repository.CreateEmployeeAsync(emp);
        }

        public async Task UpdateEmployeeAsync(EmployeeDT emp)
        {
            await _repository.UpdateEmployeeAsync(emp);
        }

        public async Task DeleteEmployeeAsync(int id)
        {
            await _repository.DeleteEmployeeAsync(id);
        }
    }
}
```

### Step 9: Create API Controller

#### 9.1 ApiService/Controllers/EmployeeController.cs
```csharp
using YourProject.Business.Contracts;
using YourProject.DataCarrier;
using YourProject.DataModel;
using Microsoft.AspNetCore.Mvc;

namespace YourProject.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;

        public EmployeeController(IEmployeeService employeeService)
        {
            _employeeService = employeeService;
        }

        [HttpGet]
        public async Task<ActionResult<EmployeeEntity[]>> GetAll()
        {
            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<EmployeeEntity>> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        public async Task<IActionResult> Create(EmployeeDT emp)
        {
            await _employeeService.CreateEmployeeAsync(emp);
            return Ok();
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, EmployeeDT emp)
        {
            if (id != emp.Empid) return BadRequest();
            await _employeeService.UpdateEmployeeAsync(emp);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return Ok();
        }
    }
}
```

### Step 10: Configure Dependency Injection

#### 10.1 ApiService/Extensions/DependencyRegistration.cs
```csharp
using YourProject.Repository.Contracts.Repositories;
using YourProject.Repository.Repositories;
using YourProject.Business.Contracts;
using YourProject.Business;

namespace YourProject.ApiService.Extensions
{
    public static class DependencyRegistration
    {
        public static void RegisterServiceDependencies(this IServiceCollection services)
        {
            // Register repositories
            services.AddScoped<IEmployeeRepository>(sp =>
            {
                var configuration = sp.GetRequiredService<IConfiguration>();
                var connectionString = configuration.GetConnectionString("DefaultConnection");
                return new EmployeeRepository(connectionString);
            });

            // Register business services
            services.AddScoped<IEmployeeService, EmployeeService>();
        }
    }
}
```

### Step 11: Configure Program.cs

#### 11.1 ApiService/Program.cs
```csharp
using YourProject.ApiService.Extensions;

namespace YourProject.ApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container
            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register your custom dependencies
            builder.Services.RegisterServiceDependencies();

            // Configure Serilog
            builder.Host.UseSerilog((context, configuration) =>
                configuration.ReadFrom.Configuration(context.Configuration));

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
```

### Step 12: Configure App Settings

#### 12.1 ApiService/appsettings.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "AllowedHosts": "*",
  "ConnectionStrings": {
    "DefaultConnection": "Server=your_server;Database=your_database;Trusted_Connection=true;MultipleActiveResultSets=true;"
  }
}
```

#### 12.2 ApiService/appsettings.Development.json
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "ConnectionStrings": {
    "DefaultConnection": "Server=localhost;Database=YourDatabase_Dev;Trusted_Connection=true;MultipleActiveResultSets=true;"
  }
}
```

### Step 13: Create Dummy Data (Optional)

#### 13.1 ApiService/_dummyUserData/User.json
```json
{
  "UserId": "1",
  "UserName": "Test User",
  "Email": "test@example.com",
  "DefaultCompanyId": 1,
  "Roles": [
    {
      "Name": "Admin",
      "Policies": [
        {
          "Type": "ActionPolicy",
          "Name": "AllowAllAccess"
        }
      ]
    }
  ]
}
```

### Step 14: Database Setup

Create the following stored procedures in your database:

```sql
-- Get all employees
CREATE PROCEDURE getEmp
AS
BEGIN
    SELECT Empid, Ename, DeptID FROM Employee
END

-- Get employee by ID
CREATE PROCEDURE getEmpById
    @EmpID INT
AS
BEGIN
    SELECT Empid, Emp_name, DeptID FROM Employee WHERE Empid = @EmpID
END

-- Insert employee
CREATE PROCEDURE insertEmployee
    @Empid INT,
    @Ename NVARCHAR(100),
    @DeptID INT
AS
BEGIN
    INSERT INTO Employee (Empid, Ename, DeptID) 
    VALUES (@Empid, @Ename, @DeptID)
    SELECT SCOPE_IDENTITY()
END

-- Update employee
CREATE PROCEDURE updateEmployee
    @Empid INT,
    @Ename NVARCHAR(100),
    @DeptID INT
AS
BEGIN
    UPDATE Employee 
    SET Ename = @Ename, DeptID = @DeptID 
    WHERE Empid = @Empid
END

-- Delete employee
CREATE PROCEDURE deleteEmployee
    @Empid INT
AS
BEGIN
    DELETE FROM Employee WHERE Empid = @Empid
END
```

## Key Architecture Principles

1. **Separation of Concerns**: Each layer has a specific responsibility
2. **Dependency Injection**: Used throughout for loose coupling
3. **Interface Segregation**: Interfaces define contracts between layers
4. **Repository Pattern**: Abstracts data access logic
5. **DTO Pattern**: Separates internal models from API contracts

## Testing Your Setup

1. **Build the solution** to ensure all references are correct
2. **Update connection string** in appsettings.json
3. **Create database and stored procedures**
4. **Run the application** and test endpoints using Swagger UI
5. **Test each CRUD operation** through the API

## Common Issues and Solutions

1. **Missing References**: Ensure all project references are added correctly
2. **Connection String Issues**: Verify database connection string format
3. **Stored Procedure Errors**: Check procedure names and parameters
4. **Dependency Injection**: Ensure all services are registered in Program.cs

This framework provides a solid foundation for building scalable microservices with proper separation of concerns and dependency injection.
