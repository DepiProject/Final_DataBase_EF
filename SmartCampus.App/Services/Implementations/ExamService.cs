using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Services.Implementations
{
    public class ExamService : IExamService
    {
        private readonly IExamRepository _examRepository;

        public ExamService(IExamRepository examRepository)
        {
            _examRepository = examRepository;
        }

        // Exam crud
        public async Task<IEnumerable<ExamDTO>> GetAllExams()
        {
            var exams = await _examRepository.GetAllExams();
            return exams.Select(e => new ExamDTO
            {
                ExamId = e.ExamId,
                CourseName = e.Course?.Name,
                Title = e.Title,
                ExamDate = e.ExamDate,
                Duration = e.Duration,
                TotalPoints = e.TotalPoints,
                CourseId = e.CourseId
            });
        }

        public async Task<IEnumerable<ExamDTO>> GetAllExamsForCourse(int courseId)
        {
            var exams = await _examRepository.GetAllExamsForCourse(courseId);
            return exams.Select(e => new ExamDTO
            {
                ExamId = e.ExamId,
                CourseName = e.Course?.Name,
                Title = e.Title,
                ExamDate = e.ExamDate,
                Duration = e.Duration,
                TotalPoints = e.TotalPoints,
                CourseId = e.CourseId
            });
        }

        public async Task<ExamDTO?> GetExamById(int id, int courseId)
        {
            var exam = await _examRepository.GetExamById(id, courseId);
            if (exam == null)
                throw new Exception("This course has no exam with this id");

            return new ExamDTO
            {
                ExamId = exam.ExamId,
                CourseName = exam.Course?.Name,
                Title = exam.Title,
                ExamDate = exam.ExamDate,
                Duration = exam.Duration,
                TotalPoints = exam.TotalPoints,
                CourseId = exam.CourseId
            };
        }

        public async Task<ExamWithQuestionsDTO?> GetExamWithQuestions(int id, int courseId)
        {
            var exam = await _examRepository.GetExamByIdWithQuestions(id, courseId);
            if (exam == null)
                throw new Exception("This course has no exam with this id");

            return new ExamWithQuestionsDTO
            {
                ExamId = exam.ExamId,
                CourseName = exam.Course?.Name,
                Title = exam.Title,
                ExamDate = exam.ExamDate,
                Duration = exam.Duration,
                TotalPoints = exam.TotalPoints,
                CourseId = exam.CourseId,
                Questions = exam.ExamQuestions?.Select(q => MapToQuestionDTO(q)).ToList() ?? new()  // list of ques
            };
        }

        public async Task<CreateExamDto?> AddExam(CreateExamDto examDto)
        {
            if (examDto?.CourseId == null || examDto.CourseId <= 0)
                throw new Exception("You must assign a valid course for this exam");

            var exam = new Exam
            {
                Title = examDto.Title,
                ExamDate = examDto.ExamDate,
                Duration = examDto.Duration,
                TotalPoints = examDto.TotalPoints,
                CourseId = examDto.CourseId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            await _examRepository.AddExam(exam);
            return examDto;
        }

        public async Task<ExamDTO?> UpdateExam(int id, int courseId, UpdateExamDto dto)
        {
            var exam = await _examRepository.GetExamById(id, courseId);
            if (exam == null)
                throw new Exception("Exam not found");

            exam.Title = dto.Title;
            exam.ExamDate = dto.ExamDate;
            exam.Duration = dto.Duration;
            exam.TotalPoints = dto.TotalPoints;
            exam.UpdatedAt = DateTime.UtcNow;

            await _examRepository.UpdateExam(exam);

            return new ExamDTO
            {
                ExamId = exam.ExamId,
                CourseName = exam.Course?.Name,
                Title = exam.Title,
                ExamDate = exam.ExamDate,
                Duration = exam.Duration,
                TotalPoints = exam.TotalPoints,
                CourseId = exam.CourseId
            };
        }

        public async Task<bool> DeleteExam(int id, int courseId)
        {
            var exam = await _examRepository.GetExamById(id, courseId);
            if (exam == null)
                throw new Exception("Exam not found");

            return await _examRepository.DeleteExam(id, courseId);
        }

        // Question crud
        public async Task<ExamQuestionDTO?> AddExamQuestion(CreateQuestionDto dto)
        {
            if (dto?.ExamId == null || dto.ExamId <= 0)
                throw new Exception("You must assign an exam for this question"); 

            if (dto.TypeId != 1 && dto.TypeId != 2)
                throw new Exception("Question Type ID must be 1 (MCQ) or 2 (True/False)");

            // Validate MCQ options
            if (dto.TypeId == 1)
            {
                if (dto.MCQOptions == null || !dto.MCQOptions.Any())
                    throw new Exception("MCQ questions must have at least one option");

                if (!dto.MCQOptions.Any(o => o.IsCorrect))
                    throw new Exception("MCQ questions must have at least one correct answer");
            }

            // Validate True/False answer
            if (dto.TypeId == 2 && dto.TrueFalseAnswer == null)
                throw new Exception("True/False questions must have an answer specified");

            var question = new ExamQuestion
            {
                QuestionText = dto.QuestionText,
                Score = dto.Score,
                OrderNumber = dto.OrderNumber,
                ExamId = dto.ExamId,
                TypeId = dto.TypeId
            };

            var addedQuestion = await _examRepository.AddExamQuestion(question);
            if (addedQuestion == null)
                throw new Exception("Failed to add question");

            // Add MCQ list of options
            if (dto.TypeId == 1 && dto.MCQOptions != null)
            {
                foreach (var optionDto in dto.MCQOptions)
                {
                    var option = new MCQOption
                    {
                        OptionText = optionDto.OptionText,
                        OrderNumber = optionDto.OrderNumber,
                        IsCorrect = optionDto.IsCorrect,
                        QuestionId = addedQuestion.QuestionId
                    };
                    await _examRepository.AddExamMcqOption(option);
                }
            }

            // Add True/False answer
            if (dto.TypeId == 2 && dto.TrueFalseAnswer.HasValue)
            {
                var tfQuestion = new TrueFalseQuestion
                {
                    QuestionId = addedQuestion.QuestionId,
                    IsCorrect = dto.TrueFalseAnswer.Value,
                    QuestionText = dto.QuestionText
                };
                await _examRepository.AddExamTFQuestion(tfQuestion);
            }

            // retrive the whole ques details
            var completeQuestion = await _examRepository.GetQuestionById(addedQuestion.QuestionId, dto.ExamId);
            return completeQuestion != null ? MapToQuestionDTO(completeQuestion) : null;
        }

        public async Task<ExamQuestionDTO?> UpdateExamQuestion(int questionId, int examId, UpdateQuestionDto dto)
        {
            var question = await _examRepository.GetQuestionById(questionId, examId);
            if (question == null)
                throw new Exception("Question not found");

            question.QuestionText = dto.QuestionText;
            question.Score = dto.Score;
            question.OrderNumber = dto.OrderNumber;

            await _examRepository.UpdateExamQuestion(question);

            // Update MCQ options
            if (question.TypeId == 1 && dto.MCQOptions != null)
            {
                var existingOptions = await _examRepository.GetMCQOptionsByQuestionId(questionId);

                // delete to replace with new one
                var optionIdsToKeep = dto.MCQOptions
                    .Where(o => o.OptionId.HasValue)
                    .Select(o => o.OptionId!.Value)
                    .ToList();

                foreach (var existingOption in existingOptions)
                {
                    if (!optionIdsToKeep.Contains(existingOption.OptionId))
                    {
                        await _examRepository.DeleteMCQOption(existingOption.OptionId);
                    }
                }

                // Update or add options
                foreach (var optionDto in dto.MCQOptions)
                {
                    if (optionDto.OptionId.HasValue)
                    {
                        var existingOption = existingOptions.FirstOrDefault(o => o.OptionId == optionDto.OptionId.Value);
                        if (existingOption != null)
                        {
                            existingOption.OptionText = optionDto.OptionText;
                            existingOption.OrderNumber = optionDto.OrderNumber;
                            existingOption.IsCorrect = optionDto.IsCorrect;
                            await _examRepository.UpdateMCQOption(existingOption);
                        }
                    }
                    else
                    {
                        var newOption = new MCQOption
                        {
                            OptionText = optionDto.OptionText,
                            OrderNumber = optionDto.OrderNumber,
                            IsCorrect = optionDto.IsCorrect,
                            QuestionId = questionId
                        };
                        await _examRepository.AddExamMcqOption(newOption);
                    }
                }
            }

            // Update True/False answer
            if (question.TypeId == 2 && dto.TrueFalseAnswer.HasValue)
            {
                var tfQuestion = await _examRepository.GetTFQuestionByQuestionId(questionId);
                if (tfQuestion != null)
                {
                    tfQuestion.IsCorrect = dto.TrueFalseAnswer.Value;
                    tfQuestion.QuestionText = dto.QuestionText;
                    await _examRepository.UpdateTFQuestion(tfQuestion);
                }
            }

            var updatedQuestion = await _examRepository.GetQuestionById(questionId, examId);
            return updatedQuestion != null ? MapToQuestionDTO(updatedQuestion) : null;
        }

        public async Task<bool> DeleteExamQuestion(int questionId, int examId)
        {
            var question = await _examRepository.GetQuestionById(questionId, examId);
            if (question == null)
                throw new Exception("Question not found");

            return await _examRepository.DeleteExamQuestion(questionId, examId);
        }

        public async Task<ExamQuestionDTO?> GetQuestionById(int questionId, int examId)
        {
            var question = await _examRepository.GetQuestionById(questionId, examId);
            if (question == null)
                throw new Exception("Question not found");

            return MapToQuestionDTO(question);
        }

        public async Task<IEnumerable<ExamQuestionDTO>> GetQuestionsByExamId(int examId)
        {
            var questions = await _examRepository.GetQuestionsByExamId(examId);
            return questions.Select(q => MapToQuestionDTO(q));
        }

        // Helper method
        private ExamQuestionDTO MapToQuestionDTO(ExamQuestion question)
        {
            var dto = new ExamQuestionDTO
            {
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                OrderNumber = question.OrderNumber,
                Score = question.Score,
                ExamId = question.ExamId,
                TypeId = question.TypeId,
                QuestionTypeName = question.QuestionType?.Name
            };

            if (question.TypeId == 1 && question.Options != null)
            {
                dto.MCQOptions = question.Options.Select(o => new MCQOptionDTO
                {
                    OptionId = o.OptionId,
                    OptionText = o.OptionText,
                    OrderNumber = o.OrderNumber,
                    IsCorrect = o.IsCorrect
                }).OrderBy(o => o.OrderNumber).ToList();
            }

            if (question.TypeId == 2 && question.TrueFalseQuestion != null)
            {
                dto.TrueFalseAnswer = new TrueFalseAnswerDTO
                {
                    TFQuestionId = question.QuestionId,
                    IsCorrect = question.TrueFalseQuestion.IsCorrect
                };
            }

            return dto;
        }
    }
}