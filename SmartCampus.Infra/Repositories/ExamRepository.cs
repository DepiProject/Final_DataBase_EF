using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<IEnumerable<Exam>> GetAllAsync()
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.ExamQuestions)
                .ThenInclude(q => q.Options)
            .Include(e => e.ExamQuestions)
                .ThenInclude(q => q.QuestionType)
                .ToListAsync();
        }

        public async Task<Exam?> GetByIdAsync(int id)
        {
            return await _context.Exams
                .Include(e => e.Course)
                .Include(e => e.ExamQuestions)
                .ThenInclude(q => q.Options) 
            .Include(e => e.ExamQuestions)
                .ThenInclude(q => q.QuestionType)
                .FirstOrDefaultAsync(e => e.ExamId == id);
        }

        public async Task AddAsync(Exam exam)
        {
            await _context.Exams.AddAsync(exam);
        }

        public async Task UpdateAsync(Exam exam)
        {
            _context.Exams.Update(exam);
        }

        public async Task DeleteAsync(Exam exam)
        {
            _context.Exams.Remove(exam);
        }

        public async Task SaveChangesAsync()
        {
            await _context.SaveChangesAsync();
        }
    }

}
