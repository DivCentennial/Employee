using MariApps.MS.Purchase.MSA.Employee.Business.Contracts;
using MariApps.MS.Purchase.MSA.Employee.DataCarrier;
using MariApps.MS.Purchase.MSA.Employee.DataModel;
using Microsoft.AspNetCore.Mvc;

namespace MariApps.MS.Purchase.MSA.Employee.Api.Controllers
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
