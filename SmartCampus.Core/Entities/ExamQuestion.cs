namespace SmartCampus.Core.Entities
{
    public class ExamQuestion
    {
        public int QuestionId { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public decimal Score { get; set; } 
        public int OrderNumber { get; set; } 

        public int ExamId { get; set; }
        public Exam? Exam { get; set; }

        public int TypeId { get; set; }
        public QuestionType? QuestionType { get; set; }

        public TrueFalseQuestion? TrueFalseQuestion { get; set; }

        public ICollection<MCQOption> Options { get; set; } = new List<MCQOption>();
        public ICollection<ExamAnswer> Answers { get; set; } = new List<ExamAnswer>();

    }
}
