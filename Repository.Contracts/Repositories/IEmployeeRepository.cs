using MariApps.MS.Purchase.MSA.Employee.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories
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
