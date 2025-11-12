using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.DTOs
{
    public class CourseDTO
    {
        public required string Name {  get; set; }
        public int CreditHours { get; set; }
        public int InstructorId { get; set; }

    }
    public class CreateCourseDTO
    {
        public required string CourseCode { get; set; } = string.Empty;
        public required string Name { get; set; } = string.Empty;
        public int CreditHours { get;  set; }
        public int InstructorId { get; set; }
        public int DepartmentId { get; set; }  // optional for now
    }
}
