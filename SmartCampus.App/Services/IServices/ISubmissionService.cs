using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCampus.App.Dtos;
using SmartCampus.Core.Entities;


namespace SmartCampus.App.Services.IServices
{
   public interface ISubmissionService
   {

       Task<ExamSubmission> StartExamAsync(int examId, int studentId);
       Task<ExamResultDto> SubmitExamAsync(SubmitExamDto dto);
       Task<ExamResultDto?> GetExamResultAsync(int examId, int studentId);
   }
}
