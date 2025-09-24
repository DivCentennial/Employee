namespace MariApps.MS.Purchase.MSA.Employee.DataModel
{
    public class EmployeeWithDepartmentDT
    {
        public int Empid { get; set; }
        public string? Ename { get; set; }
        public int Dept_ID { get; set; }
        public DepartmentInfo? Department { get; set; }
    }

    public class DepartmentInfo
    {
        public int DeptId { get; set; }
        public string? DeptName { get; set; }
    }
}
