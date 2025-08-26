using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace MariApps.MS.Purchase.MSA.Employee.ApiService.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        public EmployeeController(ILogger<EmployeeController> logger)
        {
            logger.LogInformation("EmployeeController created");
        }

        [HttpGet]
        public IActionResult Get()
        {
            return Ok("Hello World");
        }
    }
}
