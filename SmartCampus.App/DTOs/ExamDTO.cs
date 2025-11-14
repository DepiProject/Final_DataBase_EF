using System.ComponentModel.DataAnnotations;

namespace SmartCampus.App.DTOs
{
    public class ExamDTO
    {
       
        public string? CourseName { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int Duration { get; set; }
        // public decimal TotalPoints { get; set; }
        // public int CourseId { get; set; }
         //public int ExamId { get; set; }

        //public List<ExamQuestionDto> Questions { get; set; } = new();
    }
    public class ExamQuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public int OrderNumber { get; set; }
        public decimal Score { get; set; }
        public int ExamID { get; set; }
        public int QuestionType { get; set; } // 1 mcq, 2 t&f
    }




    public class CreateExamDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int Duration { get; set; }
        public decimal TotalPoints { get; set; }
        [Required]
        public int CourseId { get; set; }

        //public List<ExamQuestionDto> Questions { get; set; } = new();
    }
    //public class CreateQuestionDto
    //{
    //    [Required]
    //    public string? QuestionTitle { get; set; }
    //    public int Duration { get; set; }

    //    public int OrderNumber { get; set; }  // the order in the exam
    //    public int QuestionPoints { get; set; }

    //    [Required]
    //    public int ExamId { get; set; }
    //    public int QuestionType { get; set; } //  1 for mcq & 2 for true and false
    //}
}
