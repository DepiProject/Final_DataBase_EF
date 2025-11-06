using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Services.Implementations
{
    public class DepartmentService : IDepartmentService
    {
        private readonly IDepartmentRepository _DepartmentRepo;

        public DepartmentService(IDepartmentRepository DepartmentRepo)
        {
            _DepartmentRepo = DepartmentRepo;
        }


        public async Task<IEnumerable<DepartmentDTO>> GetAllDepartments()
        {
            var departments = await _DepartmentRepo.GetAllDepartments();
            return departments.Select(d => new DepartmentDTO
            {
                Name = d.Name,
                Building = d.Building,
                HeadId = d.HeadId
            });
        }

        public async Task<DepartmentDTO?> GetDepartmentById(int id)
        {
            var department = await _DepartmentRepo.GetDepartmentById(id);
            return new DepartmentDTO
            {
                Name = department.Name,
                Building = department.Building,
                HeadId = department.HeadId
            };
        }
        public async Task<DepartmentDTO> AddDepartment(DepartmentDTO departmentDto)
        {
            if (departmentDto.HeadId == null) throw new Exception("There is no instructor with this Id");
            var department = new Department
            {
                
                Name = departmentDto.Name,
                Building = departmentDto.Building,
                HeadId = departmentDto.HeadId
            };

            await _DepartmentRepo.AddDepartment(department);
            return new DepartmentDTO
            {
                Name = departmentDto.Name,
                Building = departmentDto.Building,
                HeadId = departmentDto.HeadId
            };
        }

        public async Task<DepartmentDTO?> UpdateDepartment(int id, DepartmentDTO departmentDto)
        {
            var existingDepartment = await _DepartmentRepo.GetDepartmentById(id);
            if (existingDepartment == null)
                return null;

            existingDepartment.Name = departmentDto.Name;
            existingDepartment.Building = departmentDto.Building;
            existingDepartment.HeadId = departmentDto.HeadId;

            var updatedDepartment = await _DepartmentRepo.UpdateDepartment(existingDepartment);

            return new DepartmentDTO
            {
                Name = updatedDepartment.Name,
                Building = updatedDepartment.Building,
                HeadId = updatedDepartment.HeadId
            };
        }
        public async Task<bool> DeleteDepartment(int id)
        {

            var department = await _DepartmentRepo.GetDepartmentById(id);
            if (department == null)
                return false;

            return await _DepartmentRepo.DeleteDepartment(id);
        }


    }
}
