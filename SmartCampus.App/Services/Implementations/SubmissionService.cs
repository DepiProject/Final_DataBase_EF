using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Services.Implementations
{
    public class SubmissionService : ISubmissionService
    {
        private readonly ISubmissionRepository _submissionRepository;

        public SubmissionService(ISubmissionRepository submissionRepository)
        {
            _submissionRepository = submissionRepository;
        }

        public async Task<ExamSubmissionDto> StartExamAsync(int examId, int studentId)
        {
            // Check if student already started this exam
            var existingSubmission = await _submissionRepository.GetSubmissionAsync(examId, studentId);
            if (existingSubmission != null)
            {
                throw new InvalidOperationException("You have already started this exam.");
            }

            // is exam exists
            var exam = await _submissionRepository.GetExamWithQuestionsAsync(examId);
            if (exam == null)
            {
                throw new InvalidOperationException("Exam not found.");
            }

            // close exam if the date is ended 
            if (exam.ExamDate > DateTime.UtcNow)
            {
                throw new InvalidOperationException("This exam is not available yet.");
            }

            // create submission
            var submission = new ExamSubmission
            {
                ExamId = examId,
                StudentId = studentId,
                StartedAt = DateTime.UtcNow,
                SubmittedAt = null,
                Score = null,
                GradedBy = null
            };

            await _submissionRepository.AddSubmissionAsync(submission);
            await _submissionRepository.SaveChangesAsync();

            return new ExamSubmissionDto
            {
                SubmissionId = submission.SubmissionId,
                ExamId = submission.ExamId ?? 0,
                ExamTitle = exam.Title,
                StudentId = submission.StudentId ?? 0,
                StartedAt = submission.StartedAt,
                SubmittedAt = submission.SubmittedAt,
                IsSubmitted = submission.SubmittedAt.HasValue,
                IsGraded = submission.GradedBy.HasValue
            };
        }

        public async Task<ExamResultDto> SubmitExamAsync(SubmitExamDto dto)
        {
            // retrive submission
            var submission = await _submissionRepository.GetSubmissionAsync(dto.ExamId, dto.StudentId);
            if (submission == null)
            {
                throw new InvalidOperationException("Exam not started. Please start the exam first.");
            }

            if (submission.SubmittedAt.HasValue)
            {
                throw new InvalidOperationException("Exam already submitted.");
            }

            // Get exam with questions
            var exam = await _submissionRepository.GetExamWithQuestionsAsync(dto.ExamId);
            if (exam == null)
            {
                throw new InvalidOperationException("Exam not found.");
            }

            // calc the score of each answer (total is the sum)
            decimal totalScore = 0;
            int correctAnswers = 0;
            var questionResults = new List<QuestionResultDto>();

            foreach (var answerDto in dto.Answers)
            {
                var question = await _submissionRepository.GetQuestionByIdAsync(answerDto.QuestionId);
                if (question == null || question.ExamId != dto.ExamId)
                {
                    continue; 
                }

                bool isCorrect = false;
                decimal pointsAwarded = 0;

                // Grade based on question type
                if (question.TypeId == 1) // MCQ
                {
                    if (answerDto.SelectedOptionId.HasValue && answerDto.SelectedOptionId.Value > 0)
                    {
                        var selectedOption = question.Options
                            .FirstOrDefault(o => o.OptionId == answerDto.SelectedOptionId.Value);

                        if (selectedOption != null && selectedOption.IsCorrect)
                        {
                            isCorrect = true;
                            pointsAwarded = question.Score;
                            correctAnswers++;
                        }

                        var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);

                        questionResults.Add(new QuestionResultDto
                        {
                            QuestionId = question.QuestionId,
                            QuestionText = question.QuestionText,
                            QuestionType = question.TypeId,
                            MaxScore = question.Score,
                            PointsAwarded = pointsAwarded,
                            IsCorrect = isCorrect,
                            StudentSelectedOptionId = answerDto.SelectedOptionId,
                            StudentSelectedOptionText = selectedOption?.OptionText,
                            CorrectOptionId = correctOption?.OptionId,
                            CorrectOptionText = correctOption?.OptionText
                        });

                        // Save MCQ answer  
                        var examAnswer = new ExamAnswer
                        {
                            SubmissionId = submission.SubmissionId,
                            QuestionId = answerDto.QuestionId,
                            TrueFalseAnswer = null,
                            SelectedOptionId = answerDto.SelectedOptionId > 0 ? answerDto.SelectedOptionId : null,
                            IsCorrect = isCorrect,
                            PointsAwarded = pointsAwarded
                        };

                        await _submissionRepository.AddAnswerAsync(examAnswer);
                    }
                }
                else if (question.TypeId == 2) // TF
                {
                    if (answerDto.TrueFalseAnswer.HasValue && question.TrueFalseQuestion != null)
                    {
                        if (answerDto.TrueFalseAnswer.Value == question.TrueFalseQuestion.IsCorrect)
                        {
                            isCorrect = true;
                            pointsAwarded = question.Score;
                            correctAnswers++;
                        }

                        questionResults.Add(new QuestionResultDto
                        {
                            QuestionId = question.QuestionId,
                            QuestionText = question.QuestionText,
                            QuestionType = question.TypeId,
                            MaxScore = question.Score,
                            PointsAwarded = pointsAwarded,
                            IsCorrect = isCorrect,
                            StudentTrueFalseAnswer = answerDto.TrueFalseAnswer,
                            CorrectTrueFalseAnswer = question.TrueFalseQuestion.IsCorrect
                        });

                        // Save TF answer 
                        var examAnswer = new ExamAnswer
                        {
                            SubmissionId = submission.SubmissionId,
                            QuestionId = answerDto.QuestionId,
                            TrueFalseAnswer = answerDto.TrueFalseAnswer,
                            SelectedOptionId = null,
                            IsCorrect = isCorrect,
                            PointsAwarded = pointsAwarded
                        };

                        await _submissionRepository.AddAnswerAsync(examAnswer);
                    }
                }

                totalScore += pointsAwarded;
            }

            // Update submission
            submission.SubmittedAt = DateTime.UtcNow;
            submission.Score = totalScore;

            await _submissionRepository.UpdateSubmissionAsync(submission);
            await _submissionRepository.SaveChangesAsync();

            // calc percentage
            decimal percentage = exam.TotalPoints > 0
                ? (totalScore / exam.TotalPoints) * 100
                : 0;

            return new ExamResultDto
            {
                SubmissionId = submission.SubmissionId,
                ExamId = exam.ExamId,
                ExamTitle = exam.Title,
                StudentId = submission.StudentId ?? 0,
                Score = totalScore,
                TotalPoints = exam.TotalPoints,
                Percentage = Math.Round(percentage, 2),
                CorrectAnswers = correctAnswers,
                TotalQuestions = exam.ExamQuestions.Count,
                IsSubmitted = true,
                IsGraded = submission.GradedBy.HasValue,
                StartedAt = submission.StartedAt,
                SubmittedAt = submission.SubmittedAt,
                QuestionResults = questionResults
            };
        }

        public async Task<ExamResultDto?> GetExamResultAsync(int examId, int studentId)
        {
            var submission = await _submissionRepository.GetSubmissionWithDetailsAsync(examId, studentId);
            if (submission == null || !submission.SubmittedAt.HasValue)
            {
                return null;
            }

            var questionResults = new List<QuestionResultDto>();

            foreach (var answer in submission.Answers)
            {
                var question = answer.Question;
                if (question == null) continue;

                var questionResult = new QuestionResultDto
                {
                    QuestionId = question.QuestionId,
                    QuestionText = question.QuestionText,
                    QuestionType = question.TypeId,
                    MaxScore = question.Score,
                    PointsAwarded = answer.PointsAwarded ?? 0,
                    IsCorrect = answer.IsCorrect ?? false
                };

                if (question.TypeId == 1) // MCQ
                {
                    questionResult.StudentSelectedOptionId = answer.SelectedOptionId;
                    var selectedOption = question.Options
                        .FirstOrDefault(o => o.OptionId == answer.SelectedOptionId);
                    questionResult.StudentSelectedOptionText = selectedOption?.OptionText;

                    var correctOption = question.Options.FirstOrDefault(o => o.IsCorrect);
                    questionResult.CorrectOptionId = correctOption?.OptionId;
                    questionResult.CorrectOptionText = correctOption?.OptionText;
                }
                else if (question.TypeId == 2) // TF
                {
                    questionResult.StudentTrueFalseAnswer = answer.TrueFalseAnswer;
                    questionResult.CorrectTrueFalseAnswer = question.TrueFalseQuestion?.IsCorrect;
                }

                questionResults.Add(questionResult);
            }

            decimal percentage = submission.Exam.TotalPoints > 0
                ? ((submission.Score ?? 0) / submission.Exam.TotalPoints) * 100
                : 0;

            return new ExamResultDto
            {
                SubmissionId = submission.SubmissionId,
                ExamId = submission.ExamId ?? 0,
                ExamTitle = submission.Exam?.Title ?? "",
                StudentId = submission.StudentId ?? 0,
                StudentName = submission.Student?.FullName ?? "",
                Score = submission.Score ?? 0,
                TotalPoints = submission.Exam?.TotalPoints ?? 0,
                Percentage = Math.Round(percentage, 2),
                CorrectAnswers = submission.Answers.Count(a => a.IsCorrect == true),
                TotalQuestions = submission.Exam?.ExamQuestions.Count ?? 0,
                IsSubmitted = submission.SubmittedAt.HasValue,
                IsGraded = submission.GradedBy.HasValue,
                StartedAt = submission.StartedAt,
                SubmittedAt = submission.SubmittedAt,
                GradedBy = submission.GradedBy,
                GradedByName = submission.Instructor?.FullName,
                QuestionResults = questionResults
            };
        }

        public async Task<ExamSubmissionDto?> GetSubmissionStatusAsync(int examId, int studentId)
        {
            var submission = await _submissionRepository.GetSubmissionWithDetailsAsync(examId, studentId);
            if (submission == null)
            {
                return null;
            }

            return new ExamSubmissionDto
            {
                SubmissionId = submission.SubmissionId,
                ExamId = submission.ExamId ?? 0,
                ExamTitle = submission.Exam?.Title ?? "",
                StudentId = submission.StudentId ?? 0,
                StudentName = submission.Student?.FullName ?? "",
                StartedAt = submission.StartedAt,
                SubmittedAt = submission.SubmittedAt,
                Score = submission.Score,
                IsSubmitted = submission.SubmittedAt.HasValue,
                IsGraded = submission.GradedBy.HasValue,
                GradedBy = submission.GradedBy
            };
        }

        public async Task<IEnumerable<ExamSubmissionDto>> GetStudentSubmissionsAsync(int studentId)
        {
            var submissions = await _submissionRepository.GetStudentSubmissionsAsync(studentId);

            return submissions.Select(s => new ExamSubmissionDto
            {
                SubmissionId = s.SubmissionId,
                ExamId = s.ExamId ?? 0,
                ExamTitle = s.Exam?.Title ?? "",
                StudentId = s.StudentId ?? 0,
                StartedAt = s.StartedAt,
                SubmittedAt = s.SubmittedAt,
                Score = s.Score,
                IsSubmitted = s.SubmittedAt.HasValue,
                IsGraded = s.GradedBy.HasValue,
                GradedBy = s.GradedBy
            });
        }

        public async Task<IEnumerable<ExamResultDto>> GetExamSubmissionsAsync(int examId)
        {
            var submissions = await _submissionRepository.GetExamSubmissionsAsync(examId);
            var exam = await _submissionRepository.GetExamWithQuestionsAsync(examId);

            return submissions
                .Where(s => s.SubmittedAt.HasValue)
                .Select(s =>
                {
                    decimal percentage = exam?.TotalPoints > 0
                        ? ((s.Score ?? 0) / exam.TotalPoints) * 100
                        : 0;

                    return new ExamResultDto
                    {
                        SubmissionId = s.SubmissionId,
                        ExamId = s.ExamId ?? 0,
                        ExamTitle = s.Exam?.Title ?? "",
                        StudentId = s.StudentId ?? 0,
                        StudentName = s.Student?.FullName ?? "",
                        Score = s.Score ?? 0,
                        TotalPoints = exam?.TotalPoints ?? 0,
                        Percentage = Math.Round(percentage, 2),
                        CorrectAnswers = s.Answers?.Count(a => a.IsCorrect == true) ?? 0,
                        TotalQuestions = exam?.ExamQuestions.Count ?? 0,
                        IsSubmitted = s.SubmittedAt.HasValue,
                        IsGraded = s.GradedBy.HasValue,
                        StartedAt = s.StartedAt,
                        SubmittedAt = s.SubmittedAt,
                        GradedBy = s.GradedBy,
                        GradedByName = s.Instructor?.FullName
                    };
                });
        }
    }
}