using MariApps.MS.Purchase.MSA.Employee.Business.Contracts;
using MariApps.MS.Purchase.MSA.Employee.DataCarrier;
using MariApps.MS.Purchase.MSA.Employee.DataModel;
using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;


namespace MariApps.MS.Purchase.MSA.Employee.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class EmployeeController : ControllerBase
    {
        private readonly IEmployeeService _employeeService;
        private readonly HttpClient _httpClient;
        private readonly HttpClient _departmentClient;

        public EmployeeController(IEmployeeService employeeService, IHttpClientFactory httpClientFactory)
        {
            _employeeService = employeeService;
            _httpClient = httpClientFactory.CreateClient();
            _departmentClient = httpClientFactory.CreateClient("DepartmentApi"); // Named client with auto Hard-Token
        }

        [HttpGet]
        [Authorize(Policy = "CanReadEmployees")]
        public async Task<ActionResult<EmployeeEntity[]>> GetAll()
        {
            // Get user information from JWT token for logging
            var username = User.FindFirst(ClaimTypes.Name)?.Value;
            var role = User.FindFirst(ClaimTypes.Role)?.Value;
            var userId = User.FindFirst("user_id")?.Value;

            // Log user information (for debugging - remove in production)
            Console.WriteLine($"User: {username}, Role: {role}, User ID: {userId}");

            var employees = await _employeeService.GetAllEmployeesAsync();
            return Ok(employees);
        }

        [HttpGet("{id}")]
        [Authorize(Policy = "CanReadEmployees")]
        public async Task<ActionResult<EmployeeEntity>> GetById(int id)
        {
            var employee = await _employeeService.GetEmployeeByIdAsync(id);
            if (employee == null)
                return NotFound();
            return Ok(employee);
        }

        [HttpPost]
        [Authorize(Policy = "CanCreateEmployees")]
        public async Task<IActionResult> Create(EmployeeDT emp)
        {
            await _employeeService.CreateEmployeeAsync(emp);
            return Ok(new { message = "Employee created successfully" });
        }

        [HttpPut("{id}")]
        [Authorize(Policy = "CanUpdateEmployees")]
        public async Task<IActionResult> Update(int id, EmployeeDT emp)
        {
            if (id != emp.Empid) return BadRequest();
            await _employeeService.UpdateEmployeeAsync(emp);
            return Ok(new { message = "Employee updated successfully" });
        }

        [HttpDelete("{id}")]
        [Authorize(Policy = "CanDeleteEmployees")]
        public async Task<IActionResult> Delete(int id)
        {
            await _employeeService.DeleteEmployeeAsync(id);
            return Ok(new { message = "Employee deleted successfully" });
        }

        // Complex inter-service communication methods removed - use simple method below

        // Simple Employee → Department communication via Ocelot
        [HttpGet("with-dept-simple")]
        [Authorize(Policy = "CanReadEmployees")]
        public async Task<IActionResult> GetEmployeesWithDepartmentSimple()
        {
            try
            {

                /* // Get employees from Employee service
                 var employees = await _employeeService.GetAllEmployeesAsync();

                 // Call Department API with automatic Hard-Token header injection
                 Console.WriteLine($"Calling Department API with Hard-Token header...");
                 var response = await _departmentClient.GetAsync("");
                 Console.WriteLine($"Department API response status: {response.StatusCode}");
                 if (!response.IsSuccessStatusCode)
                 {
                     var errorContent = await response.Content.ReadAsStringAsync();
                     Console.WriteLine($"Department API error: {errorContent}");
                     return StatusCode((int)response.StatusCode, errorContent);
                 }
                 var deptResponse = await response.Content.ReadAsStringAsync();
                 var departments = JsonSerializer.Deserialize<List<DepartmentInfo>>(deptResponse, new JsonSerializerOptions
                 {
                     PropertyNameCaseInsensitive = true
                 });*/

                // Get employees from Employee service
                var employees = await _employeeService.GetAllEmployeesAsync();

                // Get Hard-Token from request headers
                var hardToken = Request.Headers["Hard-Token"].FirstOrDefault();

                if (string.IsNullOrEmpty(hardToken))
                {
                    return BadRequest("Hard-Token header is required");
                }

                // Call Department API with manual Hard-Token header
                Console.WriteLine($"Calling Department API with Hard-Token header...");

                // Clear existing headers and add the manual Hard-Token
                _departmentClient.DefaultRequestHeaders.Clear();
                _departmentClient.DefaultRequestHeaders.Add("Hard-Token", hardToken);

                var response = await _departmentClient.GetAsync("");
                Console.WriteLine($"Department API response status: {response.StatusCode}");
                if (!response.IsSuccessStatusCode)
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    Console.WriteLine($"Department API error: {errorContent}");
                    return StatusCode((int)response.StatusCode, errorContent);
                }
                var deptResponse = await response.Content.ReadAsStringAsync();
                var departments = JsonSerializer.Deserialize<List<DepartmentInfo>>(deptResponse, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                // Join employee with department name
                var result = from e in employees
                             join d in departments on e.Dept_ID equals d.DeptId
                             select new
                             {
                                 EmpId = e.Empid,
                                 EName = e.Ename,
                                 DeptId = e.Dept_ID,
                                 DeptName = d.DeptName
                             };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"Error: {ex.Message}");
            }
        }

        // Token-based validation methods (no API calls needed)
        private bool ValidateUserExistsInJson()
        {
            var existsInJson = User.FindFirst("ExistsInJson")?.Value;
            return existsInJson == "true";
        }

        private bool ValidateUserPolicy(string policyName)
        {
            var jsonPolicies = User.FindFirst("JsonPolicies")?.Value;
            return !string.IsNullOrEmpty(jsonPolicies) && jsonPolicies.Contains(policyName);
        }

        private bool ValidateUserClaim(string claimName, string claimValue = null)
        {
            var jsonClaims = User.FindFirst("JsonClaims")?.Value;
            if (string.IsNullOrEmpty(jsonClaims)) return false;

            var claims = jsonClaims.Split(',');
            foreach (var claim in claims)
            {
                var parts = claim.Split(':');
                if (parts.Length == 2 && parts[0].Equals(claimName, StringComparison.OrdinalIgnoreCase))
                {
                    if (claimValue == null || parts[1].Equals(claimValue, StringComparison.OrdinalIgnoreCase))
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }

    // Simple Department model for JSON deserialization
    public class DepartmentInfo
    {
        public int DeptId { get; set; }
        public string DeptName { get; set; }
    }
}
