using SmartCampus.App.Dtos;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;


namespace SmartCampus.App.Services.Implementations
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _repository;

        public SubmissionService(ISubmissionRepository repository)
        {
            _repository = repository;
        }

        //  Start Exam
        public async Task<ExamSubmission> StartExamAsync(int examId, int studentId)
        {
            var existing = await _repository.GetSubmissionAsync(examId, studentId);
            if (existing != null)
                throw new InvalidOperationException("You have already started or submitted this exam.");

            var submission = new ExamSubmission
            {
                ExamId = examId,
                StudentId = studentId,
                StartedAt = DateTime.UtcNow
            };

            await _repository.AddSubmissionAsync(submission);
            await _repository.SaveChangesAsync();

            return submission;
        }

        //  Submit Exam + Auto Grading
        public async Task<ExamResultDto> SubmitExamAsync(SubmitExamDto dto)
        {
            var submission = await _repository.GetSubmissionAsync(dto.ExamId, dto.StudentId);
            if (submission == null)
                throw new InvalidOperationException("Exam not started.");

            if (submission.SubmittedAt != null)
                throw new InvalidOperationException("You have already submitted this exam.");

            decimal totalScore = 0;
            int correctCount = 0;

            foreach (var ans in dto.Answers)
            {
                var question = await _repository.GetQuestionByIdAsync(ans.QuestionId);
                if (question == null)
                    continue;

                var examAnswer = new ExamAnswer
                {
                    QuestionId = ans.QuestionId,
                    SubmissionId = submission.SubmissionId
                };

                if (question.QuestionType!.Name == "TrueFalse")
                {
                    examAnswer.TrueFalseAnswer = ans.TrueFalseAnswer;
                    examAnswer.IsCorrect = (question.TrueFalseQuestion!.IsCorrect == ans.TrueFalseAnswer);
                }
                else if (question.QuestionType!.Name == "MCQ")
                {
                    examAnswer.SelectedOptionId = ans.SelectedOptionId;
                    var selectedOption = question.Options.FirstOrDefault(o => o.OptionId == ans.SelectedOptionId);
                    examAnswer.IsCorrect = selectedOption?.IsCorrect ?? false;
                }

                // points 
                examAnswer.PointsAwarded = examAnswer.IsCorrect == true ? question.Score : 0;
                totalScore += examAnswer.PointsAwarded ?? 0;
                if (examAnswer.IsCorrect == true) correctCount++;

                await _repository.AddAnswerAsync(examAnswer);
            }

            submission.Score = totalScore;
            submission.SubmittedAt = DateTime.UtcNow;

            await _repository.SaveChangesAsync();

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
            var submission = await _repository.GetSubmissionAsync(examId, studentId);
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
