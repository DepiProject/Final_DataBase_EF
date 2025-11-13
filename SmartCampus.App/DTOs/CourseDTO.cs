using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.DTOs
{
    public class CourseDTO
    {
        public required string Name { get; set; }
        public int CreditHours { get; set; }
        public int InstructorId { get; set; }

    }
    public class CreateCourseDTO 
    {
        public required string CourseCode { get; set; } = string.Empty;
        public required string Name { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public int InstructorId { get; set; }
        public int DepartmentId { get; set; }  
    }
    public class EnrollCourseDTO 
    {
       
   
        public string CourseName { get; set; } = string.Empty;
        public string courseCode { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public string DepartmentName { get; set; } = string.Empty;
    }

    public class CreateEnrollmentDTO 
    {
        public int StudentId { get; set; }
        public int CourseId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public string CourseName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        
    }
    public class studentEnrollmentDTO 
    {
        public string studentName { get; set; } = string.Empty;
        public string courseName { get; set; } = string.Empty;
        public int CreditHours { get; set; }
        public string courseCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;
    }
    public class InstructorCoursesDTO
    {
        public string InstructorName { get; set; }= string.Empty;
        public string CourseName { get;set; } = string.Empty;
        public int CreditHours { get; set; }
        public string CourseCode { get; set; } = string.Empty;
        public string DepartmentName { get; set; } = string.Empty;

    }
}