namespace SmartCampus.Core.Entities
{
    public class Notification
    {
        public int NotificationId { get; set; }
        public string Type { get; set; } = string.Empty; // GradeUpdate, AttendanceWarning, Enrolled
        public string Message { get; set; } = string.Empty;
        public DateTime SentAt { get; set; } = DateTime.UtcNow;

        // m to 1 relation 
        public int UserId { get; set; }
        public User? User { get; set; }
    }
}
