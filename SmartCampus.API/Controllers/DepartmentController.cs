using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
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
            var department = await _departmentService.GetDepartmentById(id);
            if (department == null) return NotFound();
            return Ok(department);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<ActionResult<DepartmentDTO>> CreateDepartment(DepartmentDTO dto)
        {
            //if (dto.HeadId == null) return BadRequest("There is no instructor with this Id");
            var department = await _departmentService.AddDepartment(dto);
            return Ok(department);

        }

        [Authorize(Roles = "Admin")]
        [HttpPut("{id}")]
        public async Task<ActionResult<DepartmentDTO>> UpdateDepartment(int id, DepartmentDTO dto)
        {
            if (id == null) return BadRequest();
            var updatedDepartment = await _departmentService.UpdateDepartment(id,dto);
            return Ok(updatedDepartment);
        }

        //[HttpPatch("{id}/description")]
        //public async Task<ActionResult<TaskDto>> ChangeProjectDescription(int id, UpdateProjectDescriptionDto dto)
        //{
        //    var updatedProject = await _projectService.UpdateProjectDescription(id, dto.Description);
        //    if (updatedProject == null) return NotFound();

        //    return Ok(updatedProject);
        //}

        [Authorize(Roles = "Admin")]
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteDepartment(int id)
        {
            var deleted = await _departmentService.DeleteDepartment(id);
            if (!deleted) return NotFound();
            return Ok("Department deleted Succesfully");
        }


    }
}
