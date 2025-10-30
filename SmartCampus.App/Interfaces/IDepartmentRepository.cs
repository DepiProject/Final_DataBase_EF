using SmartCampus.Core.Entities;

namespace SmartCampus.App.Interfaces
{
    public interface IDepartmentRepository
    {
        Task<Department?> GetDepartmentById(int id);
        Task<IEnumerable<Department>> GetAllDepartments();
        Task<Department?> AddDepartment(Department department);
        Task<Department?> UpdateDepartment(Department department);
        Task<bool> DeleteDepartment(int id);
    }
}
