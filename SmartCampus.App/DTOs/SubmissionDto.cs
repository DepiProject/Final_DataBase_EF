using System.ComponentModel.DataAnnotations;

namespace SmartCampus.App.DTOs
{
    public class ExamAnswerDto
    {
        [Required(ErrorMessage = "Question ID is required")]
        public int QuestionId { get; set; }

        // TF questions (TypeId = 2)
        public bool? TrueFalseAnswer { get; set; }

        // MCQ questions (TypeId = 1)
        public int? SelectedOptionId { get; set; }
    }

    public class SubmitExamDto
    {
        [Required(ErrorMessage = "Exam ID is required")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }

        [Required(ErrorMessage = "Answers are required")]
        [MinLength(1, ErrorMessage = "At least one answer is required")]
        public List<ExamAnswerDto> Answers { get; set; } = new List<ExamAnswerDto>();
    }

    public class ExamResultDto
    {
        public int SubmissionId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public decimal TotalPoints { get; set; }
        public decimal Percentage { get; set; }
        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsGraded { get; set; }
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public int? GradedBy { get; set; }
        public string? GradedByName { get; set; }
        public List<QuestionResultDto> QuestionResults { get; set; } = new List<QuestionResultDto>();
    }

    public class QuestionResultDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int QuestionType { get; set; } // 1 = MCQ, 2 = TF
        public decimal MaxScore { get; set; }
        public decimal PointsAwarded { get; set; }
        public bool IsCorrect { get; set; }

        // Student answer
        public bool? StudentTrueFalseAnswer { get; set; }
        public int? StudentSelectedOptionId { get; set; }
        public string? StudentSelectedOptionText { get; set; }

        // Correct answer
        public bool? CorrectTrueFalseAnswer { get; set; }
        public int? CorrectOptionId { get; set; }
        public string? CorrectOptionText { get; set; }
    }

    public class StartExamDto
    {
        [Required(ErrorMessage = "Exam ID is required")]
        public int ExamId { get; set; }

        [Required(ErrorMessage = "Student ID is required")]
        public int StudentId { get; set; }
    }

    public class ExamSubmissionDto
    {
        public int SubmissionId { get; set; }
        public int ExamId { get; set; }
        public string ExamTitle { get; set; } = string.Empty;
        public int StudentId { get; set; }
        public string StudentName { get; set; } = string.Empty;
        public DateTime StartedAt { get; set; }
        public DateTime? SubmittedAt { get; set; }
        public decimal? Score { get; set; }
        public bool IsSubmitted { get; set; }
        public bool IsGraded { get; set; }
        public int? GradedBy { get; set; }
    }
}