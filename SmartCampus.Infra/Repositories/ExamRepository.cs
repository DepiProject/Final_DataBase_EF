using Microsoft.EntityFrameworkCore;
using SmartCampus.App.Interfaces;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;
namespace SmartCampus.Infra.Repositories
{
    public class ExamRepository : IExamRepository
    {
        private readonly SmartCampusDbContext _context;

        public ExamRepository(SmartCampusDbContext context)
        {
            _context = context;
        }

        // Get All Exams with Course Name
        public async Task<IEnumerable<Exam>> GetAllExams()
        {
            return await _context.Exams
                .Include(e => e.Course)
                .ToListAsync();
        }
        // Get All Exams For spesific exam
        public async Task<IEnumerable<Exam>> GetAllExamsForCourse(int courseId)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Where(e => e.CourseId == courseId)
                .ToListAsync();
        }
        // Get exam by his id and course id
        public async Task<Exam?> GetExamById(int id, int courseId)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.ExamId == id && e.CourseId == courseId);
        }

        // Get exam details 
        public async Task<Exam?> GetExamByIdWithQuestions(int id, int courseId)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(q => q.QuestionType)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(q => q.Options)
                .Include(e => e.ExamQuestions)
                    .ThenInclude(q => q.TrueFalseQuestion)
                .FirstOrDefaultAsync(e => e.ExamId == id && e.CourseId == courseId);
        }

        // Create Exam like empty templete
        public async Task<Exam?> AddExam(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
            await _context.SaveChangesAsync();
            return exam;
        }

        // update Exam templete
        public async Task<Exam?> UpdateExam(Exam exam)
        {
            exam.UpdatedAt = DateTime.UtcNow;
            _context.Exams.Update(exam);
            await _context.SaveChangesAsync();
            return exam;
        }

        // elete Exam
        public async Task<bool> DeleteExam(int id, int courseId)
        {
            var exam = await _context.Exams
                .FirstOrDefaultAsync(e => e.ExamId == id && e.CourseId == courseId);

            if (exam == null)
                return false;

            _context.Exams.Remove(exam);
            await _context.SaveChangesAsync();
            return true;
        }

        // Question CRUD
        public async Task<ExamQuestion?> GetQuestionById(int questionId, int examId)
        {
            return await _context.ExamQuestions
                .Include(q => q.QuestionType)
                .Include(q => q.Options)
                .Include(q => q.TrueFalseQuestion)
                .FirstOrDefaultAsync(q => q.QuestionId == questionId && q.ExamId == examId);
        }

        public async Task<ExamQuestion?> AddExamQuestion(ExamQuestion question)
        {
            await _context.ExamQuestions.AddAsync(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<ExamQuestion?> UpdateExamQuestion(ExamQuestion question)
        {
            _context.ExamQuestions.Update(question);
            await _context.SaveChangesAsync();
            return question;
        }

        public async Task<bool> DeleteExamQuestion(int questionId, int examId)
        {
            var question = await _context.ExamQuestions
                .FirstOrDefaultAsync(q => q.QuestionId == questionId && q.ExamId == examId);

            if (question == null)
                return false;

            _context.ExamQuestions.Remove(question);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<ExamQuestion>> GetQuestionsByExamId(int examId)
        {
            return await _context.ExamQuestions
                .Include(q => q.QuestionType)
                .Include(q => q.Options)
                .Include(q => q.TrueFalseQuestion)
                .Where(q => q.ExamId == examId)
                .OrderBy(q => q.OrderNumber)
                .ToListAsync();
        }

        // MCQ crud
        public async Task<MCQOption> AddExamMcqOption(MCQOption examOption)
        {
            await _context.MCQOptions.AddAsync(examOption);
            await _context.SaveChangesAsync();
            return examOption;
        }

        public async Task<MCQOption?> UpdateMCQOption(MCQOption option)
        {
            _context.MCQOptions.Update(option);
            await _context.SaveChangesAsync();
            return option;
        }

        public async Task<bool> DeleteMCQOption(int optionId)
        {
            var option = await _context.MCQOptions.FindAsync(optionId);
            if (option == null)
                return false;

            _context.MCQOptions.Remove(option);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<MCQOption>> GetMCQOptionsByQuestionId(int questionId)
        {
            return await _context.MCQOptions
                .Where(o => o.QuestionId == questionId)
                .OrderBy(o => o.OrderNumber)
                .ToListAsync();
        }

        // True/False crud
        public async Task<TrueFalseQuestion> AddExamTFQuestion(TrueFalseQuestion trueFalse)
        {
            await _context.TrueFalseQuestions.AddAsync(trueFalse);
            await _context.SaveChangesAsync();
            return trueFalse;
        }

        public async Task<TrueFalseQuestion?> UpdateTFQuestion(TrueFalseQuestion trueFalse)
        {
            _context.TrueFalseQuestions.Update(trueFalse);
            await _context.SaveChangesAsync();
            return trueFalse;
        }

        public async Task<bool> DeleteTFQuestion(int questionId)
        {
            var tfQuestion = await _context.TrueFalseQuestions
                .FirstOrDefaultAsync(t => t.QuestionId == questionId);

            if (tfQuestion == null)
                return false;

            _context.TrueFalseQuestions.Remove(tfQuestion);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<TrueFalseQuestion?> GetTFQuestionByQuestionId(int questionId)
        {
            return await _context.TrueFalseQuestions
                .FirstOrDefaultAsync(t => t.QuestionId == questionId);
        }
    }
}