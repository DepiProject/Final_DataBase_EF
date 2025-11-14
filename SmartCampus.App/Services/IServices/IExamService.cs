using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCampus.App.DTOs;

namespace SmartCampus.App.Services.IServices
{
    public interface IExamService
    {

        Task<IEnumerable<ExamDto>> GetAllAsync();
        Task<ExamDto?> GetByIdAsync(int id);
        Task<ExamDto> CreateAsync(CreateExamDto dto);
        Task<ExamDto?> UpdateAsync(int id, CreateExamDto dto);
        Task<bool> DeleteAsync(int id);
        // add examQuestion
        Task<ExamQuestionDto> AddQuestionToExamAsync(int examId, ExamQuestionDto questionDto);
        Task<bool> RemoveQuestionFromExamAsync(int examId, int questionId);

    }
}
