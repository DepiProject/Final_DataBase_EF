namespace SmartCampus.Core.Entities
{
    public class Attendance
    {
        public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty; // Present, Absent

        public int? CourseId { get; set; }
        public Course? Course { get; set; }

        // ✅ Course Snapshot - بيانات الكورس وقت تسجيل الحضور
        public string CourseName { get; set; } = string.Empty;
        public string CourseCode { get; set; } = string.Empty;
        public int? StudentId { get; set; }
        public Student? Student { get; set; }
    }
}
