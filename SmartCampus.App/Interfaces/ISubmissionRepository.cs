using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Interfaces
{
    public interface ISubmissionRepository
    {
        Task<ExamSubmission?> GetSubmissionAsync(int examId, int studentId);
        Task AddSubmissionAsync(ExamSubmission submission);
        Task AddAnswerAsync(ExamAnswer answer);
        Task<ExamQuestion?> GetQuestionByIdAsync(int questionId);

        Task SaveChangesAsync();
    }
}
