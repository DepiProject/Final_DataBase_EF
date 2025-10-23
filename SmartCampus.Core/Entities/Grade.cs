namespace SmartCampus.Core.Entities
{
    public  class Grade
    {
        public int GradeId { get; set; }
        public decimal Score { get; set; }
        public string GradeLetter { get; set; } = string.Empty; // A+, A, B+, etc
        public DateTime EnteredAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int EnrollmentId { get; set; }
        public Enrollment? Enrollment { get; set; }

        public int EnteredBy { get; set; }
        public Instructor? Instructor { get; set; }

    }
}
