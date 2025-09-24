using MariApps.MS.Purchase.MSA.Employee.ApiService.Extensions;
using MariApps.MS.Purchase.MSA.Employee.Business;
using MariApps.MS.Purchase.MSA.Employee.Business.Contracts;
using MariApps.MS.Purchase.MSA.Employee.Repository.Repositories;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories;
using MariApps.Framework.MS.Core.Extensions;
using MariApps.Framework.MS.Core.Middlewares;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace MariApps.MS.Purchase.MSA.Employee.ApiService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Register your repository and service dependencies
            builder.Services.AddScoped<IEmployeeRepository>(sp =>
            {
                var connString = builder.Configuration.GetConnectionString("PALConnectionString");
                return new EmployeeRepository(connString);
            });
            builder.Services.AddScoped<IEmployeeService, EmployeeService>();

            // Add Controllers
            builder.Services.AddControllers();

            // Add Swagger services
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            // Register HttpClient for simple inter-service communication via Ocelot
            builder.Services.AddHttpClient();

            // Register HttpClient with automatic Hard-Token header injection for Department API
            builder.Services.AddHttpClient("DepartmentApi", (sp, client) =>
            {
                var config = sp.GetRequiredService<IConfiguration>();
                var baseUrl = config["DepartmentApi:BaseUrl"];
                var token = config["DepartmentApi:HardToken"];

                client.BaseAddress = new Uri(baseUrl);
                client.DefaultRequestHeaders.Add("Hard-Token", token); // Auto inject
            });

            // Add JWT Authentication
            /* builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                 .AddJwtBearer(options =>
                 {
                     options.TokenValidationParameters = new TokenValidationParameters
                     {
                         ValidateIssuer = true,
                         ValidateAudience = true,
                         ValidateLifetime = true,
                         ValidateIssuerSigningKey = true,
                         ValidIssuer = "http://localhost:5000",
                         ValidAudience = "http://localhost:5000",
                         IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong!"))
                     };
                 });*/

            // Add JWT Authentication
            builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = false,        // Disable for testing
                        ValidateAudience = false,      // Disable for testing
                        ValidateLifetime = false,      // Disable for testing
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes("YourSuperSecretKeyThatIsAtLeast32CharactersLong!"))
                    };
                });

            // Configure authorization policies
            builder.Services.AddAuthorization(options =>
            {
                options.AddPolicy("CanReadEmployees", policy =>
                    policy.RequireClaim("permissions", "Employee.Read"));

                options.AddPolicy("CanCreateEmployees", policy =>
                    policy.RequireClaim("permissions", "Employee.Create"));

                options.AddPolicy("CanUpdateEmployees", policy =>
                    policy.RequireClaim("permissions", "Employee.Update"));

                options.AddPolicy("CanDeleteEmployees", policy =>
                    policy.RequireClaim("permissions", "Employee.Delete"));
            });

            // Your existing extension methods for other services
            builder.Services.RegisterServiceDependecies();
            // builder.InitMSCoreService(); // Commented out - interferes with JWT

            var app = builder.Build();

            // Configure the HTTP request pipeline
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            // Add custom timing middleware
            app.UseMiddleware<MariApps.MS.Purchase.MSA.Employee.ApiService.Middlewares.RequestTimingMiddleware>();
            // Add Authentication & Authorization middleware
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}