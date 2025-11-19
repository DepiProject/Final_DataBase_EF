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

        // ============= COURSE MANAGEMENT =============

     
        /// Get all active (non-deleted) courses
     
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetAllCourses()
        {
            var courses = await _courseService.GetAllCourses();
            return Ok(new
            {
                Success = true,
                Message = "Courses retrieved successfully",
                Count = courses.Count(),
                Data = courses
            });
        }

        
        /// Get all courses including soft-deleted ones (Admin only)
       
        [HttpGet("all-including-deleted")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<ActionResult<IEnumerable<CourseDTO>>> GetAllCoursesIncludingDeleted()
        {
            var courses = await _courseService.GetAllCoursesIncludingDeleted();
            return Ok(new
            {
                Success = true,
                Message = "All courses (including deleted) retrieved successfully",
                Count = courses.Count(),
                Data = courses
            });
        }

  
        /// Get a specific course by ID
     
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDTO>> GetCourseById(int id)
        {
            var course = await _courseService.GetCourseById(id);
            if (course == null)
                return NotFound(new
                {
                    Success = false,
                    Message = "Course not found or has been deleted"
                });

            return Ok(new
            {
                Success = true,
                Message = "Course retrieved successfully",
                Data = course
            });
        }

   
        /// Create a new course
        /// Business Rules Applied:
        /// - Instructor must exist
        /// - Instructor cannot teach more than 2 courses
        /// - Instructor cannot teach more than 12 credit hours
        /// - Course code must be unique
      
        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateCourseDTO>> CreateCourse([FromBody] CreateCourseDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });

            try
            {
                var course = await _courseService.AddCourse(dto);
                return Ok(new
                {
                    Success = true,
                    Message = "Course created successfully",
                    Data = course
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Validation error",
                    Error = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Success = false,
                    Message = "Business rule violation",
                    Error = ex.Message
                });
            }
        }

      
        /// Update an existing course
        /// Business Rules Applied:
        /// - New instructor must exist
        /// - New instructor workload validated (if instructor changes)
     
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult<CourseDTO>> UpdateCourse(int id, [FromBody] CourseDTO dto)
        {
            if (id == 0)
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid course ID"
                });

            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });

            try
            {
                var updateCourse = await _courseService.UpdateCourse(id, dto);
                if (updateCourse == null)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Course not found or has been deleted"
                    });

                return Ok(new
                {
                    Success = true,
                    Message = "Course updated successfully",
                    Data = updateCourse
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Business rule violation",
                    Error = ex.Message
                });
            }
        }

        /// Soft delete a course
        /// Note: Related data (enrollments, exams, attendance) is preserved
    
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteCourse(int id)
        {
            try
            {
                var deleted = await _courseService.DeleteCourse(id);
                if (!deleted)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Course not found"
                    });

                return Ok(new
                {
                    Success = true,
                    Message = "Course deleted successfully (soft delete)",
                    Note = "Related exams, enrollments, and attendance records are preserved"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Cannot delete course",
                    Error = ex.Message
                });
            }
        }

       
        /// Restore a soft-deleted course (Admin only)
      
        [HttpPost("{id}/restore")]
        [Authorize(Roles = "Admin")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RestoreCourse(int id)
        {
            try
            {
                var restored = await _courseService.RestoreCourse(id);
                if (!restored)
                    return NotFound(new
                    {
                        Success = false,
                        Message = "Course not found"
                    });

                return Ok(new
                {
                    Success = true,
                    Message = "Course restored successfully"
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Cannot restore course",
                    Error = ex.Message
                });
            }
        }

       
        /// Permanently delete a course (Admin only)
        /// Warning: This cannot be undone and requires no related data
   
        //[HttpDelete("{id}/permanent")]
        //[Authorize(Roles = "Admin")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //[ProducesResponseType(StatusCodes.Status400BadRequest)]
        //[ProducesResponseType(StatusCodes.Status404NotFound)]
        //public async Task<ActionResult> PermanentlyDeleteCourse(int id)
        //{
        //    try
        //    {
        //        var deleted = await _courseService.PermanentlyDeleteCourse(id);
        //        if (!deleted)
        //            return NotFound(new
        //            {
        //                Success = false,
        //                Message = "Course not found"
        //            });

        //        return Ok(new
        //        {
        //            Success = true,
        //            Message = "Course permanently deleted",
        //            Warning = "This action cannot be undone"
        //        });
        //    }
        //    catch (InvalidOperationException ex)
        //    {
        //        return BadRequest(new
        //        {
        //            Success = false,
        //            Message = "Cannot permanently delete course",
        //            Reason = ex.Message
        //        });
        //    }
        //}

        // ============= ENROLLMENT MANAGEMENT =============

   
        /// Enroll a student in a course
        /// Business Rules Applied:
        /// - Course must exist and not be deleted
        /// - Student cannot enroll twice in same course
        /// - Course capacity limit (max 50 students)
        /// - Student semester credit limit (max 21 credits)
        /// - Student yearly credit limit (max 36 credits)
        /// - Prerequisites must be completed
        /// - Department restriction (student can only enroll in their department courses)
      
        [HttpPost("enroll")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<ActionResult<CreateEnrollmentDTO>> EnrollCourse([FromBody] CreateEnrollmentDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(new
                {
                    Success = false,
                    Message = "Invalid data",
                    Errors = ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage))
                });

            try
            {
                var enrollment = await _courseService.AddEnrollCourse(dto);
                return Ok(new
                {
                    Success = true,
                    Message = "Student enrolled in course successfully",
                    Data = enrollment
                });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Validation error",
                    Error = ex.Message
                });
            }
            catch (InvalidOperationException ex)
            {
                return Conflict(new
                {
                    Success = false,
                    Message = "Enrollment failed - Business rule violation",
                    Error = ex.Message
                });
            }
        }

        /// Remove a student enrollment

        [HttpDelete("enrollment/{enrollmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RemoveEnrollCourse(int enrollmentId)
        {
            var removed = await _courseService.RemoveEnrollCourse(enrollmentId);
            if (!removed)
                return NotFound(new
                {
                    Success = false,
                    Message = "Enrollment not found"
                });

            return Ok(new
            {
                Success = true,
                Message = "Enrollment removed successfully"
            });
        }

        // ============= QUERY ENDPOINTS =============


        /// Get all courses taught by a specific instructor

        [HttpGet("instructor/{instructorId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<InstructorCoursesDTO>>> GetCoursesByInstructorId(int instructorId)
        {
            var courses = await _courseService.GetCoursesByInstructorId(instructorId);
            return Ok(new
            {
                Success = true,
                Message = "Instructor courses retrieved successfully",
                InstructorId = instructorId,
                CourseCount = courses.Count(),
                Data = courses
            });
        }

   
        /// Get all students enrolled in a specific course
     
        [HttpGet("{courseId}/enrollments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<studentEnrollmentDTO>>> GetEnrollmentStudentsByCourseID(int courseId)
        {
            var enrollments = await _courseService.GetEnrollmentStudentsByCourseID(courseId);
            return Ok(new
            {
                Success = true,
                Message = "Course enrollments retrieved successfully",
                CourseId = courseId,
                EnrollmentCount = enrollments.Count(),
                Data = enrollments
            });
        }

  
        /// Get all courses in a specific department (Admin/Instructor view)

        [HttpGet("department/{departmentId}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<EnrollCourseDTO>>> GetAllCoursesByDepartmentID(int departmentId)
        {
            var courses = await _courseService.GetAllCoursesByDepartmentID(departmentId);
            return Ok(new
            {
                Success = true,
                Message = "Department courses retrieved successfully",
                DepartmentId = departmentId,
                CourseCount = courses.Count(),
                Data = courses
            });
        }

     
        /// NEW: Get available courses for a specific student (filtered by their department)
        /// Students can only see courses from their own department
        
        [HttpGet("student/{studentId}/available-courses")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<IEnumerable<EnrollCourseDTO>>> GetAvailableCoursesForStudent(int studentId)
        {
            try
            {
                var courses = await _courseService.GetAvailableCoursesForStudent(studentId);
                return Ok(new
                {
                    Success = true,
                    Message = "Available courses for student retrieved successfully",
                    Note = "Only courses from student's department are shown",
                    StudentId = studentId,
                    CourseCount = courses.Count(),
                    Data = courses
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new
                {
                    Success = false,
                    Message = "Cannot retrieve courses",
                    Error = ex.Message
                });
            }
        }

     
        /// Get all enrollments for a specific student

        [HttpGet("student/{studentId}/enrollments")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult<IEnumerable<studentEnrollmentDTO>>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _courseService.GetEnrollmentsByStudentId(studentId);
            return Ok(new
            {
                Success = true,
                Message = "Student enrollments retrieved successfully",
                StudentId = studentId,
                EnrollmentCount = enrollments.Count(),
                Data = enrollments
            });
        }

        // ============= UTILITY ENDPOINTS =============

        /// NEW: Check if a course can run (minimum 5 students enrolled)
 
        [HttpGet("{courseId}/can-run")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<ActionResult> CanCourseRun(int courseId)
        {
            var canRun = await _courseService.CanCourseRun(courseId);
            return Ok(new
            {
                Success = true,
                CourseId = courseId,
                CanRun = canRun,
                Message = canRun
                    ? "Course has minimum required enrollment and can run"
                    : "Course does not have minimum required enrollment (5 students)"
            });
        }

    
        ///  NEW: Get available seats in a course

        //[HttpGet("{courseId}/available-seats")]
        //[ProducesResponseType(StatusCodes.Status200OK)]
        //public async Task<ActionResult> GetAvailableSeats(int courseId)
        //{
        //    var availableSeats = await _courseService.GetAvailableSeats(courseId);
        //    return Ok(new
        //    {
        //        Success = true,
        //        CourseId = courseId,
        //        AvailableSeats = availableSeats,
        //        MaxCapacity = 50,
        //        IsFull = availableSeats == 0,
        //        Message = availableSeats > 0
        //            ? $"{availableSeats} seats available"
        //            : "Course is full"
        //    });
        //}
    }
}
