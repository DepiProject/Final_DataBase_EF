using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
namespace SmartCampus.Core.Entities
{
    public class Instructor
    {
        public int InstructorId { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string? ContactNumber { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public Department? HeadOfDepartment { get; set; }
        // 1 to 1 relation 
        public int UserId { get; set; }
        public User? User { get; set; }

        public int DepartmentId { get; set; }
        public Department? Department { get; set; }

        // 1 - m
        public ICollection<Course> Courses { get; set; } = new List<Course>();
        public ICollection<Grade> Grades { get; set; } = new List<Grade>();
        public ICollection<ExamSubmission> ExamSubmissions { get; set; } = new List<ExamSubmission>();

    }
}
