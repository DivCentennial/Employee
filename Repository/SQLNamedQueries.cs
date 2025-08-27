using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MariApps.MS.Purchase.MSA.Employee.Repository
{
    internal static class SQLNamedQueries
    {
        // Employee related stored procedures
        public const string getEmp = "getEmp";
        public const string GetEmployeeById = "getEmpById";
        public const string InsertEmployee = "insertEmployee";
        public const string UpdateEmployee = "updateEmployee";
        public const string DeleteEmployee = "deleteEmployee";
    }
}
