using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SmartCampus.App.DTOs;
using SmartCampus.App.Services.IServices;

namespace SmartCampus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DepartmentController : ControllerBase
    {
        private readonly IDepartmentService _departmentService;

        public DepartmentController(IDepartmentService departmentService)
        {
            _departmentService = departmentService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<DepartmentDTO>>> GetAllDepartments()
        {
            var departments = await _departmentService.GetAllDepartments();
            return Ok(departments);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<DepartmentDTO>> GetDepartmentById(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid department ID");

            var department = await _departmentService.GetDepartmentById(id);
            if (department == null)
                return NotFound($"Department with ID {id} not found");

            return Ok(department);
        }

       // [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<DepartmentDTO>> CreateDepartment(CreateDepartmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var department = await _departmentService.AddDepartment(dto);
            if (department == null)
                return BadRequest("Failed to create department. Please ensure the head instructor exists.");

            return CreatedAtAction(
                nameof(GetDepartmentById),
                new { id = department.Id },
                department
            );
        }

        //[Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<DepartmentDTO>> UpdateDepartment(int id, UpdateDepartmentDTO dto)
        {
            if (id <= 0)
                return BadRequest("Invalid department ID");

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var updatedDepartment = await _departmentService.UpdateDepartment(id, dto);
            if (updatedDepartment == null)
                return NotFound($"Department with ID {id} not found");

            return Ok(updatedDepartment);
        }

        //[Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            if (id <= 0)
                return BadRequest("Invalid department ID");

            var deleted = await _departmentService.DeleteDepartment(id);
            if (!deleted)
                return NotFound($"Department with ID {id} not found");

            return Ok(new { message = "Department deleted successfully", id });
        }
    }
}