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

        public async Task<ExamSubmission?> GetSubmissionAsync(int examId, int studentId)
        {
            return await _context.ExamSubmissions
                .Include(s => s.Answers)
                .Include(s => s.Exam)
                    .ThenInclude(e => e.Questions)
                        .ThenInclude(q => q.QuestionType)
                .Include(s => s.Exam)
                    .ThenInclude(e => e.Questions)
                        .ThenInclude(q => q.TrueFalseQuestion)
                .Include(s => s.Exam)
                    .ThenInclude(e => e.Questions)
                        .ThenInclude(q => q.Options)
                .FirstOrDefaultAsync(s => s.ExamId == examId && s.StudentId == studentId);
        }

        public async Task AddSubmissionAsync(ExamSubmission submission)
        {
            await _context.ExamSubmissions.AddAsync(submission);
        }

        public async Task AddAnswerAsync(ExamAnswer answer)
        {
            await _context.ExamAnswers.AddAsync(answer);
        }

        public async Task<ExamQuestion?> GetQuestionByIdAsync(int questionId)
        {
            return await _context.ExamQuestions
                .Include(q => q.QuestionType)
                .Include(q => q.TrueFalseQuestion)
                .Include(q => q.Options)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
