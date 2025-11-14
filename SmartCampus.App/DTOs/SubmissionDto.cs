
namespace SmartCampus.App.Dtos

{
    public class ExamAnswerDto
    {

        public int QuestionId { get; set; }
        public bool? TrueFalseAnswer { get; set; }
        public int? SelectedOptionId { get; set; }
    }


    public class SubmitExamDto
    {
        public int ExamId { get; set; }
        public int StudentId { get; set; }
        public List<ExamAnswerDto> Answers { get; set; }= new List<ExamAnswerDto>();


    }


    public class ExamResultDto
    {
        public int ExamId { get; set; }
        public int StudentId { get; set; }

        public decimal Score { get; set; }

        public int CorrectAnswers { get; set; }
        public int TotalQuestions { get; set; }
        public string Status { get; set; } = string.Empty;

    }


}
