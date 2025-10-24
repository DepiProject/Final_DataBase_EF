using Microsoft.AspNetCore.Identity;

namespace SmartCampus.Core.Entities
{
    public class AppUser : IdentityUser<int>
    {
        
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
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
