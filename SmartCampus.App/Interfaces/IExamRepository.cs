using SmartCampus.Core.Entities;
namespace SmartCampus.App.Interfaces
{
    public interface IExamRepository
    {
        // Exam crud
        Task<IEnumerable<Exam>> GetAllExams();
        Task<IEnumerable<Exam>> GetAllExamsForCourse(int courseId);
        Task<Exam?> GetExamById(int id, int courseId);
        Task<Exam?> GetExamByIdWithQuestions(int id, int courseId);
        Task<Exam?> AddExam(Exam exam);
        Task<Exam?> UpdateExam(Exam exam);
        Task<bool> DeleteExam(int id, int courseId);

        // Question crud
        Task<ExamQuestion?> GetQuestionById(int questionId, int examId);
        Task<ExamQuestion?> AddExamQuestion(ExamQuestion question);
        Task<ExamQuestion?> UpdateExamQuestion(ExamQuestion question);
        Task<bool> DeleteExamQuestion(int questionId, int examId);
        Task<IEnumerable<ExamQuestion>> GetQuestionsByExamId(int examId);

        // MCQ crud
        Task<MCQOption> AddExamMcqOption(MCQOption examOption);
        Task<MCQOption?> UpdateMCQOption(MCQOption option);
        Task<bool> DeleteMCQOption(int optionId);
        Task<IEnumerable<MCQOption>> GetMCQOptionsByQuestionId(int questionId);

        // TF crud
        Task<TrueFalseQuestion> AddExamTFQuestion(TrueFalseQuestion trueFalse);
        Task<TrueFalseQuestion?> UpdateTFQuestion(TrueFalseQuestion trueFalse);
        Task<bool> DeleteTFQuestion(int questionId);
        Task<TrueFalseQuestion?> GetTFQuestionByQuestionId(int questionId);
    }
}