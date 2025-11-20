namespace SmartCampus.Core.Entities
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty; 

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public int? StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
