namespace SmartCampus.Core.Entities
{
    public class TrueFalseQuestion
    {
        public int QuestionId { get; set; }
        public bool IsCorrect { get; set; }
        public string QuestionText { get; set; } = string.Empty;
        public ExamQuestion? ExamQuestion { get; set; }
    }
}
