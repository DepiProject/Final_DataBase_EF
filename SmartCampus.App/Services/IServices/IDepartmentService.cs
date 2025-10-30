using SmartCampus.App.DTOs;
using SmartCampus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.Services.IServices
{
    public interface IDepartmentService
    {
        Task<DepartmentDTO?> GetDepartmentById(int id);
        Task<IEnumerable<DepartmentDTO>> GetAllDepartments();
        Task<DepartmentDTO?> AddDepartment(DepartmentDTO departmentDto);
        Task<DepartmentDTO?> UpdateDepartment(int id, DepartmentDTO departmentDto);
        Task<bool> DeleteDepartment(int id);
    }
}
