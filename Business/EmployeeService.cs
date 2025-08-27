using MariApps.MS.Purchase.MSA.Employee.Business.Contracts;
using MariApps.MS.Purchase.MSA.Employee.DataCarrier;
using MariApps.MS.Purchase.MSA.Employee.DataModel;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace MariApps.MS.Purchase.MSA.Employee.Business
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
