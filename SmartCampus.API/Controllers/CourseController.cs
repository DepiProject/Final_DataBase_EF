using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCampus.App.DTOs;
using SmartCampus.App.Services.Implementations;
using SmartCampus.App.Services.IServices;

namespace SmartCampus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CourseController : ControllerBase
    {
        private readonly ICourseService _courseService;
        public CourseController(ICourseService courseService)
        {
            _courseService = courseService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetAllCourses()
        {
            var courses = await _courseService.GetAllCourses();
            return Ok(courses);
        }
        [HttpPost]
        public async Task<ActionResult<CreateCourseDTO>> CreateCourse(CreateCourseDTO dto)
        {
            var course = await _courseService.AddCourse(dto);
            return Ok(course);
        }
        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseById(id);
            if (course == null) return NotFound();
            return Ok(course);
        }
        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDTO>> UpdateCourse(int id, CourseDTO dto)
        {
            if (id == 0) return BadRequest();
            var updateCourse = await _courseService.UpdateCourse(id, dto);
            return Ok(updateCourse);
        }
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            var deleted = await _courseService.DeleteCourse(id);
            if (!deleted) return NotFound();
            return Ok("Course deleted Succesfully");
        }
    }
}
