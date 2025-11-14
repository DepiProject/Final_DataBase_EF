using SmartCampus.Core.Entities;

namespace SmartCampus.App.Interfaces
{
    public interface ISubmissionRepository
    {
        // Submission crud
        Task<ExamSubmission?> GetSubmissionAsync(int examId, int studentId);
        Task<ExamSubmission?> GetSubmissionByIdAsync(int submissionId);
        Task<ExamSubmission?> GetSubmissionWithDetailsAsync(int examId, int studentId);
        Task<IEnumerable<ExamSubmission>> GetStudentSubmissionsAsync(int studentId);
        Task<IEnumerable<ExamSubmission>> GetExamSubmissionsAsync(int examId);
        Task AddSubmissionAsync(ExamSubmission submission);
        Task UpdateSubmissionAsync(ExamSubmission submission);

        // Answer crud
        Task AddAnswerAsync(ExamAnswer answer);
        Task UpdateAnswerAsync(ExamAnswer answer);
        Task<ExamAnswer?> GetAnswerAsync(int submissionId, int questionId);

        // Question crud
        Task<ExamQuestion?> GetQuestionByIdAsync(int questionId);
        Task<Exam?> GetExamWithQuestionsAsync(int examId);

        // Save changes
        Task SaveChangesAsync();
    }
}
