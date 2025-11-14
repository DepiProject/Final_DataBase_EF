using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCampus.App.DTOs;
using SmartCampus.App.Services.Implementations;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;

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
        [HttpPost]
        [Route("EnrollCourse")]
        public async Task<ActionResult<CreateEnrollmentDTO>> EnrollCourse(CreateEnrollmentDTO dto)
        {
            var enrollment = await _courseService.AddEnrollCourse(dto);
            return Ok("Course enrolled Succesfully");
        }
        [HttpDelete]
        [Route("RemoveEnrollment/{enrollmentId}")]
        public async Task<ActionResult> RemoveEnrollCourse(int enrollmentId)
        {
            var removed = await _courseService.RemoveEnrollCourse(enrollmentId);
            if (!removed) return NotFound();
            return Ok("Enrollment removed Succesfully");
        }
        [HttpGet]
        [Route("InstructorCourses/{instructorId}")]
        public async Task<ActionResult<IEnumerable<EnrollCourseDTO>>> GetCoursesByInstructorId(int instructorId)
        {
            var courses = await _courseService.GetCoursesByInstructorId(instructorId);
            return Ok(courses);
        }
        [HttpGet]
        [Route("CourseEnrollments/{courseId}")]
        public async Task<ActionResult<IEnumerable<CreateEnrollmentDTO>>> GetEnrollmentStudentsByCourseID(int courseId)
        {
            var enrollments = await _courseService.GetEnrollmentStudentsByCourseID(courseId);
            return Ok(enrollments);
        }
        [HttpGet]
        [Route("DepartmentCourses/{departmentId}")]
        public async Task<ActionResult<IEnumerable<EnrollCourseDTO>>> GetAllCoursesByDepartmentID(int departmentId)
        {
            var courses = await _courseService.GetAllCoursesByDepartmentID(departmentId);
            return Ok(courses);
        }
        [HttpGet]
        [Route("StudentEnrollments/{studentId}")]
        public async Task<ActionResult<IEnumerable<EnrollCourseDTO>>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _courseService.GetEnrollmentsByStudentId(studentId);
            return Ok(enrollments);
        }
    }
}