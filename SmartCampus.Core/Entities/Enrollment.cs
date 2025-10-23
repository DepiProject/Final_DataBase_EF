﻿namespace SmartCampus.Core.Entities
{
    public class Enrollment
    {
        public int EnrollmentId { get; set; }
        public DateTime EnrollmentDate { get; set; } = DateTime.UtcNow;
        public string Status { get; set; } = "Enrolled"; // Enrolled, Dropped, Completed
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int StudentId { get; set; }
        public Student? Student { get; set; }

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public Grade? Grade { get; set; }

    }
}
