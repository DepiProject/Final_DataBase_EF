using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _departmentRepo;

        public DepartmentService(IDepartmentRepository departmentRepo)
        {
            _departmentRepo = departmentRepo;
        }

        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartments()
        {
            var departments = await _departmentRepo.GetAllDepartments();
            return departments.Select(d => new DepartmentDTO
            {
                Id = d.DepartmentId,
                Name = d.Name,
                Building = d.Building,
                HeadId = d.HeadId
            });
        }

        public async Task<DepartmentDTO?> GetDepartmentById(int id)
        {
            var department = await _departmentRepo.GetDepartmentById(id);
            if (department == null) return null;

            return new DepartmentDTO
            {
                Id = department.DepartmentId,
                Name = department.Name,
                Building = department.Building,
                HeadId = department.HeadId
            };
        }

        public async Task<DepartmentDTO?> AddDepartment(CreateDepartmentDTO departmentDto)
        {
            if (departmentDto.HeadId == null)
                return null;

            var department = new Department
            {
                Name = departmentDto.Name,
                Building = departmentDto.Building,
                HeadId = departmentDto.HeadId
            };

            var addedDepartment = await _departmentRepo.AddDepartment(department);
            if (addedDepartment == null) return null;

            return new DepartmentDTO
            {
                Id = addedDepartment.DepartmentId,
                Name = addedDepartment.Name,
                Building = addedDepartment.Building,
                HeadId = addedDepartment.HeadId
            };
        }

        public async Task<DepartmentDTO?> UpdateDepartment(int id, UpdateDepartmentDTO departmentDto)
        {
            var existingDepartment = await _departmentRepo.GetDepartmentById(id);
            if (existingDepartment == null)
                return null;

            existingDepartment.Name = departmentDto.Name;
            existingDepartment.Building = departmentDto.Building;
            existingDepartment.HeadId = departmentDto.HeadId;

            var updatedDepartment = await _departmentRepo.UpdateDepartment(existingDepartment);
            if (updatedDepartment == null) return null;

            return new DepartmentDTO
            {
                Id = updatedDepartment.DepartmentId,
                Name = updatedDepartment.Name,
                Building = updatedDepartment.Building,
                HeadId = updatedDepartment.HeadId
            };
        }

        public async Task<bool> DeleteDepartment(int id)
        {
            var department = await _departmentRepo.GetDepartmentById(id);
            if (department == null)
                return false;

            return await _departmentRepo.DeleteDepartment(id);
        }
    }
}