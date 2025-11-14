using SmartCampus.App.DTOs;
using SmartCampus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.Services.IServices
{
    public interface IExamService
    {
        Task<IEnumerable<ExamDTO>> GetAllExams();
        Task<IEnumerable<ExamDTO>> GetAllExamsForCourse(int Id);
        Task<ExamDTO?> GetExamById(int id, int courseID);
        Task<CreateExamDto?> AddExam(CreateExamDto dto);
        //Task<ExamQuestionDto?> AddExamQuestion(ExamQuestionDto examQuestiondto);
  
        //Task<bool> DeleteExam(int id, int courseId);
    }
}
