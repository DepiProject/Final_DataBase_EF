using Microsoft.EntityFrameworkCore;
using SmartCampus.App.Interfaces;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;

namespace SmartCampus.Infra.Repositories
{
    public class DepartmentRepository : IDepartmentRepository
    {
        private readonly SmartCampusDbContext _context;

        public DepartmentRepository(SmartCampusDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Department>> GetAllDepartments()
        {
            return await _context.Departments.ToListAsync();
        }

        public async Task<Department?> GetDepartmentById(int id)
        {
            return await _context.Departments.FindAsync(id);
        }

        public async Task<Department?> AddDepartment(Department department)
        {
            _context.Departments.Add(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<Department?> UpdateDepartment(Department department)
        {
            var deptExist = await _context.Departments.FindAsync(department.DepartmentId);
            if (deptExist == null)
                return null;

            _context.Departments.Update(department);
            await _context.SaveChangesAsync();
            return department;
        }

        public async Task<bool> DeleteDepartment(int id)
        {
            var deptExist = await _context.Departments.FindAsync(id);
            if (deptExist == null)
                return false;

            _context.Departments.Remove(deptExist);
            await _context.SaveChangesAsync();
            return true;
        }
    }
}