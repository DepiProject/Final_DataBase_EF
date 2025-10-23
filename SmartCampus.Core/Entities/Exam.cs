﻿namespace SmartCampus.Core.Entities
{
    public class Exam
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;  // "Midterm Exam", "Final Exam"
        public DateTime ExamDate { get; set; }
        public int Duration { get; set; }
        public decimal TotalPoints { get; set; } 
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public int CourseId { get; set; }
        public Course? Course { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; }  = new List<ExamQuestion>();
        public ICollection<ExamSubmission> ExamSubmissions { get; set; } = new List<ExamSubmission>();
    }
}
