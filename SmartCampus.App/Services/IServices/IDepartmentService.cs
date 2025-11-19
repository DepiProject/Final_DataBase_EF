using SmartCampus.App.DTOs;

namespace SmartCampus.App.Services.IServices
{
    public interface IDepartmentService
    {
        Task<DepartmentDTO?> GetDepartmentById(int id);
        Task<IEnumerable<DepartmentDTO>> GetAllDepartments();
        Task<DepartmentDTO?> AddDepartment(CreateDepartmentDTO departmentDto);
        Task<DepartmentDTO?> UpdateDepartment(int id, UpdateDepartmentDTO departmentDto);
        Task<bool> DeleteDepartment(int id);
    }
}