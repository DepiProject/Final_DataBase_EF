using Microsoft.EntityFrameworkCore;
using SmartCampus.App.Interfaces;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;

namespace SmartCampus.Infra.Repositories.Implementations
{
    public class SubmissionRepository : ISubmissionRepository
    {
        private readonly SmartCampusDbContext _context;

        public SubmissionRepository(SmartCampusDbContext context)
        {
            _context = context;
        }

        // Submission crud
        public async Task<ExamSubmission?> GetSubmissionAsync(int examId, int studentId)
        {
            return await _context.ExamSubmissions
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.ExamId == examId && s.StudentId == studentId);
        }

        public async Task<ExamSubmission?> GetSubmissionByIdAsync(int submissionId)
        {
            return await _context.ExamSubmissions
                .Include(s => s.Answers)
                .Include(s => s.Exam)
                .Include(s => s.Student)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(s => s.SubmissionId == submissionId);
        }

        public async Task<ExamSubmission?> GetSubmissionWithDetailsAsync(int examId, int studentId)
        {
            return await _context.ExamSubmissions
                .Include(s => s.Answers)
                    .ThenInclude(a => a.Question)
                        .ThenInclude(q => q.QuestionType)
                .Include(s => s.Answers)
                    .ThenInclude(a => a.Question)
                        .ThenInclude(q => q.Options)
                .Include(s => s.Answers)
                    .ThenInclude(a => a.Question)
                        .ThenInclude(q => q.TrueFalseQuestion)
                .Include(s => s.Exam)
                    .ThenInclude(e => e.Course)
                .Include(s => s.Exam)
                    .ThenInclude(e => e.ExamQuestions)
                .Include(s => s.Student)
                .Include(s => s.Instructor)
                .FirstOrDefaultAsync(s => s.ExamId == examId && s.StudentId == studentId);
        }

        public async Task<IEnumerable<ExamSubmission>> GetStudentSubmissionsAsync(int studentId)
        {
            return await _context.ExamSubmissions
                .Include(s => s.Exam)
                    .ThenInclude(e => e.Course)
                .Include(s => s.Instructor)
                .Where(s => s.StudentId == studentId)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<ExamSubmission>> GetExamSubmissionsAsync(int examId)
        {
            return await _context.ExamSubmissions
                .Include(s => s.Student)
                .Include(s => s.Instructor)
                .Include(s => s.Answers)
                .Where(s => s.ExamId == examId)
                .OrderByDescending(s => s.StartedAt)
                .ToListAsync();
        }

        public async Task AddSubmissionAsync(ExamSubmission submission)
        {
            await _context.ExamSubmissions.AddAsync(submission);
        }

        public async Task UpdateSubmissionAsync(ExamSubmission submission)
        {
            _context.ExamSubmissions.Update(submission);
        }

        // Answer crud
        public async Task AddAnswerAsync(ExamAnswer answer)
        {
            await _context.ExamAnswers.AddAsync(answer);
        }

        public async Task UpdateAnswerAsync(ExamAnswer answer)
        {
            _context.ExamAnswers.Update(answer);
        }

        public async Task<ExamAnswer?> GetAnswerAsync(int submissionId, int questionId)
        {
            return await _context.ExamAnswers
                .FirstOrDefaultAsync(a => a.SubmissionId == submissionId && a.QuestionId == questionId);
        }

        // Question crud
        public async Task<ExamQuestion?> GetQuestionByIdAsync(int questionId)
        {
            return await _context.ExamQuestions
                .Include(q => q.QuestionType)
                .Include(q => q.TrueFalseQuestion)
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task<Exam?> GetExamWithQuestionsAsync(int examId)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(q => q.QuestionType)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(q => q.Options)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(q => q.TrueFalseQuestion)
                .FirstOrDefaultAsync(e => e.ExamId == examId);
        }

        // Save changes
        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}