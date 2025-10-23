namespace SmartCampus.Core.Entities
{
    public class User
    {
        public int UserId { get; set; }
        public string Username { get; set; } = string.Empty;
        public string PasswordHash { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty; // Admin, Student, Instructor
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // 1 to 1 relation 
        public Student? Student { get; set; }
        public Instructor? Instructor { get; set; }

        // 1 to m relation 
        public ICollection<Notification> Notifications { get; set; } = new List<Notification>();

    }
}
