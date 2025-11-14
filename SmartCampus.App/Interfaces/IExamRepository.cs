using SmartCampus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.Interfaces
{
    public interface IExamRepository
    {
        Task<IEnumerable<Exam>> GetAllExams();
        Task<IEnumerable<Exam>> GetAllExamsForCourse(int id);
        Task<Exam?> GetExamById(int id, int courseID);
        Task<Exam?> AddExam(Exam exam);
        //Task<bool> DeleteExam(int id, int courseId);
        Task<ExamQuestion?> AddExamQuestion(ExamQuestion examQuestion);
        Task<MCQOption> AddExamMcqOption(MCQOption examOption);
        Task<TrueFalseQuestion> AddExamTFQuestion(TrueFalseQuestion trueFalse);

    }
}
