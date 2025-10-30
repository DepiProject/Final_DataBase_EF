using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;
using SmartCampus.App.Dtos;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;

namespace SmartCampus.App.Services.Implementations
{

    public class SubmissionService : ISubmissionService
    {
        private readonly SmartCampusDbContext _context;

        public SubmissionService(SmartCampusDbContext context)
        {
            _context = context;
        }

        //  Start Exam
        public async Task<ExamSubmission> StartExamAsync(int examId, int studentId)
        {
            var existing = await _context.ExamSubmissions
                .FirstOrDefaultAsync(s => s.ExamId == examId && s.StudentId == studentId);

            if (existing != null)
                throw new InvalidOperationException("You have already started or submitted this exam.");

            var submission = new ExamSubmission
            {
                ExamId = examId,
                StudentId = studentId,
                StartedAt = DateTime.UtcNow
            };

            _context.ExamSubmissions.Add(submission);
            await _context.SaveChangesAsync();

            return submission;
        }

        // Submit Exam + Auto Grading
        public async Task<ExamResultDto> SubmitExamAsync(SubmitExamDto dto)
        {
            var submission = await _context.ExamSubmissions
                .Include(s => s.Exam)
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.ExamId == dto.ExamId && s.StudentId == dto.StudentId);

            if (submission == null)
                throw new InvalidOperationException("Exam not started.");

            if (submission.SubmittedAt != null)
                throw new InvalidOperationException("You have already submitted this exam.");

            decimal totalScore = 0;
            int correctCount = 0;

            foreach (var ans in dto.Answers)
            {
                var question = await _context.ExamQuestions
                    .Include(q => q.QuestionType)
                    .Include(q => q.TrueFalseQuestion)
                    .Include(q => q.Options)
                    .FirstOrDefaultAsync(q => q.QuestionId == ans.QuestionId);

                if (question == null)
                    continue;

                var examAnswer = new ExamAnswer
                {
                    QuestionId = ans.QuestionId,
                    SubmissionId = submission.SubmissionId
                };

                //  Handle True/False
                if (question.QuestionType!.Name == "TrueFalse")
                {
                    examAnswer.TrueFalseAnswer = ans.TrueFalseAnswer;
                    examAnswer.IsCorrect = (question.TrueFalseQuestion!.IsCorrect == ans.TrueFalseAnswer);
                }

                //  Handle MCQ
                else if (question.QuestionType!.Name == "MCQ")
                {
                    examAnswer.SelectedOptionId = ans.SelectedOptionId;
                    var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == ans.SelectedOptionId);
                    examAnswer.IsCorrect = selectedOption?.IsCorrect ?? false;
                }

                //  Points
                examAnswer.PointsAwarded = examAnswer.IsCorrect == true ? question.Score : 0;
                totalScore += examAnswer.PointsAwarded ?? 0;
                if (examAnswer.IsCorrect == true) correctCount++;

                _context.ExamAnswers.Add(examAnswer);
            }

            submission.Score = totalScore;
            submission.SubmittedAt = DateTime.UtcNow;
            await _context.SaveChangesAsync();

            return new ExamResultDto
            {
                ExamId = dto.ExamId,
                StudentId = dto.StudentId,
                Score = totalScore,
                CorrectAnswers = correctCount,
                TotalQuestions = dto.Answers.Count,
                Status = "Submitted & Graded Automatically"
            };
        }

        //  Get Exam Result
        public async Task<ExamResultDto?> GetExamResultAsync(int examId, int studentId)
        {
            var submission = await _context.ExamSubmissions
                .Include(s => s.Answers)
                .FirstOrDefaultAsync(s => s.ExamId == examId && s.StudentId == studentId);

            if (submission == null) return null;

            return new ExamResultDto
            {
                ExamId = examId,
                StudentId = studentId,
                Score = submission.Score ?? 0,
                CorrectAnswers = submission.Answers.Count(a => a.IsCorrect == true),
                TotalQuestions = submission.Answers.Count,
                Status = submission.SubmittedAt == null ? "In Progress" : "Submitted"
            };
        }
    }
}
