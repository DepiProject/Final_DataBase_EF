namespace SmartCampus.Core.Entities
{
    public class QuestionType
    {
        public int TypeId { get; set; }
        public string Name { get; set; } = string.Empty; // MCQ, TrueFalse
        public string? Description { get; set; }

        public ICollection<ExamQuestion> ExamQuestions { get; set; } = new List<ExamQuestion>();
    }
}
