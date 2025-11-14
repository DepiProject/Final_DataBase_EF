using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.DTOs
{


    public class ExamQuestionDto
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public decimal Score { get; set; }
        public string QuestionType { get; set; } = string.Empty;
        public List<string> Options { get; set; } = new();
    }


    public class ExamDto
    {
        public int ExamId { get; set; }
        public string Title { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int Duration { get; set; }
        public decimal TotalPoints { get; set; }
        public int CourseId { get; set; }
    

       public List<ExamQuestionDto> Questions { get; set; } = new();

    }

    public class CreateExamDto
    {
        public string Title { get; set; } = string.Empty;
        public DateTime ExamDate { get; set; }
        public int Duration { get; set; }
        public decimal TotalPoints { get; set; }
        public int CourseId { get; set; }

        public List<ExamQuestionDto> Questions { get; set; } = new();
    }


}
