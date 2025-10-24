using System.ComponentModel.DataAnnotations.Schema;

namespace SmartCampus.Core.Entities
{
    public class Department
    {
        public int DepartmentId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Building { get; set; } = string.Empty;

        // 1 to 1 relation 
        public int? HeadId {  get; set; }
        public Instructor? Instructor { get; set; }

        // 1 to m relation 
        public ICollection<Student> Students { get; set; } = new List<Student>();
        public ICollection<Instructor> Instructors { get; set; } = new List<Instructor>();
        public ICollection<Course> Courses { get; set; } = new List<Course>();


    }
}
