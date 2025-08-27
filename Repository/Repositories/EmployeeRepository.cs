using MariApps.MS.Purchase.MSA.Employee.DataModel;
using MariApps.MS.Purchase.MSA.Employee.Repository.Contracts.Repositories;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariApps.MS.Purchase.MSA.Employee.Repository.Repositories
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

            using var cmd = new SqlCommand(SQLNamedQueries.getEmp, conn);
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

            using var cmd = new SqlCommand("getEmpById", conn);
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

            using var cmd = new SqlCommand("insertEmployee", conn);
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

            using var cmd = new SqlCommand("updateEmployee", conn);
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

            using var cmd = new SqlCommand("deleteEmployee", conn);
            cmd.CommandType = CommandType.StoredProcedure;
            cmd.Parameters.AddWithValue("@Empid", id);

            await cmd.ExecuteNonQueryAsync();
        }
    }
}
