using AspNetCoreMVC.Models.ViewModel;

namespace AspNetCoreMVC.Models.RepositoryPatterns
{
    public interface IEmployeeRepository
    {
        Employee GetEmployee(int Id);
        IEnumerable<Employee> GetAllEmployee();
        Employee Add(Employee employee);
        Employee Update(Employee employeeChanges);
        Employee Delete(int Id);
    }
}
