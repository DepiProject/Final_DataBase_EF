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

       
        [HttpGet("all-including-deleted")]
        // [Authorize(Roles = "Admin")] 
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetAllCoursesIncludingDeleted()
        {
            var courses = await _courseService.GetAllCoursesIncludingDeleted();
            return Ok(courses);
        }

        [HttpPost]
        public async Task<ActionResult<CreateCourseDTO>> CreateCourse(CreateCourseDTO dto)
        {
            try
            {
                var course = await _courseService.AddCourse(dto);
                return Ok(new { Message = "Course created successfully", Data = course });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseById(id);
            if (course == null)
                return NotFound(new { Message = "Course not found or has been deleted" });

            return Ok(course);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CourseDTO>> UpdateCourse(int id, CourseDTO dto)
        {
            if (id == 0)
                return BadRequest(new { Message = "Invalid course ID" });

            try
            {
                var updateCourse = await _courseService.UpdateCourse(id, dto);
                if (updateCourse == null)
                    return NotFound(new { Message = "Course not found or has been deleted" });

                return Ok(new { Message = "Course updated successfully", Data = updateCourse });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            try
            {
                var deleted = await _courseService.DeleteCourse(id);
                if (!deleted)
                    return NotFound(new { Message = "Course not found" });

                return Ok(new
                {
                    Message = "Course deleted successfully (soft delete)",
                    Note = "Related exams, enrollments, and attendance records are preserved"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

       
        [HttpPost("{id}/restore")]
        // [Authorize(Roles = "Admin")] 
        public async Task<ActionResult> RestoreCourse(int id)
        {
            try
            {
                var restored = await _courseService.RestoreCourse(id);
                if (!restored)
                    return NotFound(new { Message = "Course not found" });

                return Ok(new { Message = "Course restored successfully" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
        }

       
        [HttpDelete("{id}/permanent")]
        // [Authorize(Roles = "Admin")] 
        public async Task<ActionResult> PermanentlyDeleteCourse(int id)
        {
            try
            {
                var deleted = await _courseService.PermanentlyDeleteCourse(id);
                if (!deleted)
                    return NotFound(new { Message = "Course not found" });

                return Ok(new { Message = "Course permanently deleted" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Message = "Cannot permanently delete course",
                    Reason = ex.Message
                });
            }
        }

        [HttpPost]
        [Route("EnrollCourse")]
        public async Task<ActionResult<CreateEnrollmentDTO>> EnrollCourse(CreateEnrollmentDTO dto)
        {
            try
            {
                var enrollment = await _courseService.AddEnrollCourse(dto);
                return Ok(new { Message = "Course enrolled successfully", Data = enrollment });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { Message = ex.Message });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new { Message = ex.Message });
            }
        }

        [HttpDelete]
        [Route("RemoveEnrollment/{enrollmentId}")]
        public async Task<ActionResult> RemoveEnrollCourse(int enrollmentId)
        {
            var removed = await _courseService.RemoveEnrollCourse(enrollmentId);
            if (!removed)
                return NotFound(new { Message = "Enrollment not found" });

            return Ok(new { Message = "Enrollment removed successfully" });
        }

        [HttpGet]
        [Route("InstructorCourses/{instructorId}")]
        public async Task<ActionResult<IEnumerable<InstructorCoursesDTO>>> GetCoursesByInstructorId(int instructorId)
        {
            var courses = await _courseService.GetCoursesByInstructorId(instructorId);
            return Ok(courses);
        }

        [HttpGet]
        [Route("CourseEnrollments/{courseId}")]
        public async Task<ActionResult<IEnumerable<studentEnrollmentDTO>>> GetEnrollmentStudentsByCourseID(int courseId)
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
        public async Task<ActionResult<IEnumerable<studentEnrollmentDTO>>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _courseService.GetEnrollmentsByStudentId(studentId);
            return Ok(enrollments);
        }
    }
}