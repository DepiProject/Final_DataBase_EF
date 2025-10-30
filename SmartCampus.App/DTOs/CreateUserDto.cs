namespace SmartCampus.App.DTOs
{
    public class CreateUserDto
    {
        public string UserName { get; set; }
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Student or Instructor
        public int? DepartmentId { get; set; } 
        public string? StudentCode { get; set; } // if student
        public string? ContactNumber { get; set; }
        public string Level { get; set; } // if student
    }

    public class LoginDto
    {
        public string Email { get; set; } = string.Empty;
        public string Password { get; set; } = string.Empty;
    }
}
