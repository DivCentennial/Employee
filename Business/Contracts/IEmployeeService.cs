using MariApps.MS.Purchase.MSA.Employee.DataCarrier;
using MariApps.MS.Purchase.MSA.Employee.DataModel;
using System.Threading.Tasks;

namespace MariApps.MS.Purchase.MSA.Employee.Business.Contracts
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
