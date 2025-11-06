using Azure.Core;
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
 
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }
        [HttpGet("Students")]
        public async Task<ActionResult<IEnumerable<StudentDto>>> GetAllStudents()
        {
            var students = await _userService.GetAllStudents();
            return Ok(students);
        }
        [HttpGet("Instructors")]
        public async Task<ActionResult<IEnumerable<InstructorDto>>> GetAllInstructors()
        {
            var instructors = await _userService.GetAllInstructors();
            return Ok(instructors);
        }
        [Authorize(Roles = "Admin")]
        [HttpPost("create")]
        public async Task<IActionResult> CreateUser([FromBody] CreateUserDto dto)
        { 
            var user = await _userService.CreateUserAsync(dto);
            if (user == null)
                return BadRequest(new { message = "Failed to create user" });
            return Ok(dto);
        }

        [HttpPut("student/{email}")]
        public async Task<ActionResult<UpdateStudentDto>> UpdateStudent(string email, UpdateStudentDto studentDto)
        {
            if (email == null) return BadRequest();
            var updatedStudent = await _userService.UpdateStudent(email, studentDto);
            return Ok(updatedStudent);
        }

        [HttpPut("instructor/{email}")]
        public async Task<ActionResult<UpdateInstructorDto>> UpdateInstructor(string email, UpdateInstructorDto instructorDto)
        {
            if (email == null) return BadRequest();
            var updatedInstructor = await _userService.UpdateInstructor(email, instructorDto);
            return Ok(updatedInstructor);
        }

        [HttpPatch("{id}/PhoneNumber")]
        public async Task<ActionResult<UpdateStudentNumberDto>> UpdateSudentContactNumber(int id, string phone)
        {
            var student = await _userService.UpdateSudentContactNumber(id, phone);

            if (student == null)
            {
                return NotFound(new { message = $"Student with ID {id} not found" });
            }
            return Ok(student); ;

        }

        [HttpDelete("student/{id}")]
        public async Task<ActionResult> DeleteStudent(int id)
        {
            var deleted = await _userService.DeleteStudent(id);
            if (!deleted) return NotFound();
            return Ok("Student deleted Succesfully");
        }

        [HttpDelete("instructor/{id}/{replaceId}")]
        public async Task<ActionResult> DeleteInstructor(int id,int replaceId)
        {
            var deleted = await _userService.DeleteInstructor(id, replaceId);
            if (!deleted) return NotFound();
            return Ok("Instructor deleted Succesfully");
        }

        [HttpGet("StudentProfile/{id}")]
        public async Task<ActionResult<StudentDto>> GetStudentProfile(int id)
        {
            var student = await _userService.GetStudentById(id);
            if (student == null) return NotFound();
            return Ok(student);
        }
        [HttpGet("InstructorProfile/{id}")]
        public async Task<ActionResult<InstructorDto>> GetInstructorProfile(int id)
        {
            var instructor = await _userService.GetInstructorById(id);
            if (instructor == null) return NotFound();
            return Ok(instructor);
        }
    }
}
