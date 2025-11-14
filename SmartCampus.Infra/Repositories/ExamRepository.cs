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

        public async Task<IEnumerable<Exam>> GetAllExams()
        {
            return await _context.Exams
                .Include(e => e.Course)
                .ToListAsync();
        }
        public async Task<IEnumerable<Exam>> GetAllExamsForCourse(int id)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Where(e => e.CourseId == id)
                .ToListAsync();
        }

        public async Task<Exam?> GetExamById(int id, int courseID)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .FirstOrDefaultAsync(e => e.ExamId == id && e.CourseId == courseID);
        }

        public async Task<Exam?> AddExam(Exam exam)
        {
            _context.Exams?.AddAsync(exam);
            await _context.SaveChangesAsync();
            return exam;
        }
        public async Task<ExamQuestion?> AddExamQuestion(ExamQuestion question)
        {
            await _context.SaveChangesAsync();
            return question;
        }
        public async Task<MCQOption> AddExamMcqOption(MCQOption examOption)
        {
            _context.MCQOptions?.AddAsync(examOption);
            await _context.SaveChangesAsync();
            return examOption;
        }
        public async Task<TrueFalseQuestion> AddExamTFQuestion(TrueFalseQuestion trueFalse)
        {
            _context.TrueFalseQuestions?.AddAsync(trueFalse);
            await _context.SaveChangesAsync();
            return trueFalse;
        }

        //public async Task UpdateAsync(Exam exam)
        //{
        //    _context.Exams.Update(exam);
        //}

        //public async Task<bool> DeleteExam(int id, int courseId)
        //{
        //    var examExit = await _context.Exams
        //        .Include(e => e.Course)
        //        .FirstOrDefaultAsync(e => e.ExamId == id && e.CourseId == courseId);
        //    if (examExit != null)
        //    {
        //        _context.Exams.Remove(examExit);
        //        await _context.SaveChangesAsync();
        //        return true;
        //    }
        //    return false;
        //}


    }
}
