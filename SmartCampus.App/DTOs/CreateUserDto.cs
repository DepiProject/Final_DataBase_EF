using System.ComponentModel.DataAnnotations;

namespace SmartCampus.App.DTOs
{
    public class CreateUserDto
    {
        public required string UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Student or Instructor
        public int? DepartmentId { get; set; } 
        public string? StudentCode { get; set; } // if student
        public string? ContactNumber { get; set; }
        public string? Level { get; set; } // if student
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
    public class StudentDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public required string StudentCode { get; set; }
        public string? ContactNumber { get; set; }
        public string? Level { get; set; } 
        public int? DepartmentId { get; set; }
    }
    public class InstructorDto
    {
        public required string FullName { get; set; }
        public required string Email { get; set; }
        public string? ContactNumber { get; set; }
        public int? DepartmentId { get; set; }
    }
    public class UpdateStudentNumberDto
    {
        public required string FullName { get; set; }
        public required string StudentCode { get; set; }
        [Required]
        [MaxLength(20)]
        public string? ContactNumber { get; set; }
    }





    public class UpdateStudentDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public string Level { get; set; } 
        public int? DepartmentId { get; set; }
    }
    public class UpdateInstructorDto
    {
        public string UserName { get; set; }
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public int? DepartmentId { get; set; }
    }


}
