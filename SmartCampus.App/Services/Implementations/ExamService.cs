using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<ExamDto> CreateAsync(CreateExamDto dto)
        {
            var exam = new Exam
            {
                Title = dto.Title,
                ExamDate = dto.ExamDate,
                Duration = dto.Duration,
                TotalPoints = dto.TotalPoints,
                CourseId = dto.CourseId,
                CreatedAt = DateTime.UtcNow,
                UpdatedAt = DateTime.UtcNow
            };

            // Add questions 
            if (dto.Questions != null && dto.Questions.Any())
            {
                foreach (var questionDto in dto.Questions)
                {
                    var question = new ExamQuestion
                    {
                        QuestionText = questionDto.QuestionText,
                        Score = questionDto.Score,
                        OrderNumber = exam.ExamQuestions.Count + 1,
                        TypeId = GetQuestionTypeId(questionDto.QuestionType)
                    };

                    // Add optiions for MCQ
                    if (questionDto.QuestionType.ToLower() == "mcq" && questionDto.Options.Any())
                    {
                        for (int i = 0; i < questionDto.Options.Count; i++)
                        {
                            question.Options.Add(new MCQOption
                            {
                                OptionText = questionDto.Options[i],
                                OrderNumber = i + 1
                            });
                        }
                    }

                    //    Add True/False question
                    if (questionDto.QuestionType.ToLower() == "truefalse")
                    {
                        question.TrueFalseQuestion = new TrueFalseQuestion();
                    }

                    exam.ExamQuestions.Add(question);
                }
            }

             await _examRepository.AddAsync(exam);
            await _examRepository.SaveChangesAsync();

            return MapToExamDto(exam);
        }

        public async Task<ExamDto?> GetByIdAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            return exam == null ? null : MapToExamDto(exam);
        }

        public async Task<IEnumerable<ExamDto>> GetAllAsync()
        {
            var exams = await _examRepository.GetAllAsync();
            return exams.Select(MapToExamDto);
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null) return false;

            await _examRepository.DeleteAsync(exam);
            await _examRepository.SaveChangesAsync();
            return true;
        }

        public async Task<ExamDto?> UpdateAsync(int id, CreateExamDto dto)
        {
            var exam = await _examRepository.GetByIdAsync(id);
            if (exam == null) return null;

            exam.Title = dto.Title;
            exam.ExamDate = dto.ExamDate;
            exam.Duration = dto.Duration;
            exam.TotalPoints = dto.TotalPoints;
            exam.CourseId = dto.CourseId;
            exam.UpdatedAt = DateTime.UtcNow;

            await _examRepository.UpdateAsync(exam);
            await _examRepository.SaveChangesAsync();

            return await GetByIdAsync(id);
        }

        public async Task<ExamQuestionDto> AddQuestionToExamAsync(int examId, ExamQuestionDto questionDto)
        {
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null)
                throw new Exception("Exam not found");

            var question = new ExamQuestion
            {
                QuestionText = questionDto.QuestionText,
                Score = questionDto.Score,
                OrderNumber = exam.ExamQuestions.Count + 1,
                TypeId = GetQuestionTypeId(questionDto.QuestionType),
                ExamId = examId
            };

            // Add Options
            if (questionDto.QuestionType.ToLower() == "mcq" && questionDto.Options.Any())
            {
                for (int i = 0; i < questionDto.Options.Count; i++)
                {
                    question.Options.Add(new MCQOption
                    {
                        OptionText = questionDto.Options[i],
                        OrderNumber = i + 1
                    });
                }
            }

            // True/False question
            if (questionDto.QuestionType.ToLower() == "truefalse")
            {
                question.TrueFalseQuestion = new TrueFalseQuestion();
            }

            exam.ExamQuestions.Add(question);
            await _examRepository.SaveChangesAsync();

            return new ExamQuestionDto
            {
                QuestionId = question.QuestionId,
                QuestionText = question.QuestionText,
                Score = question.Score,
                QuestionType = questionDto.QuestionType,
                Options = questionDto.Options
            };
        }

        public async Task<bool> RemoveQuestionFromExamAsync(int examId, int questionId)
        {
            var exam = await _examRepository.GetByIdAsync(examId);
            if (exam == null) return false;

            var question = exam.ExamQuestions.FirstOrDefault(q => q.QuestionId == questionId);
            if (question == null) return false;

            exam.ExamQuestions.Remove(question);
            await _examRepository.SaveChangesAsync();
            return true;
        }

        private ExamDto MapToExamDto(Exam exam)
        {
            return new ExamDto
            {
                ExamId = exam.ExamId,
                Title = exam.Title,
                ExamDate = exam.ExamDate,
                Duration = exam.Duration,
                TotalPoints = exam.TotalPoints,
                CourseId = exam.CourseId,
                Questions = exam.ExamQuestions.Select(q => new ExamQuestionDto
                {
                    QuestionId = q.QuestionId,
                    QuestionText = q.QuestionText,
                    Score = q.Score,
                    QuestionType = q.QuestionType?.Name ?? "",
                    Options = q.Options.Select(o => o.OptionText).ToList()
                }).ToList()
            };
        }

        private int GetQuestionTypeId(string typeName)
        {
            return typeName.ToLower() switch
            {
                "mcq" => 1,
                "truefalse" => 2,
                "text" => 3,
                _ => 1
            };
        }
    }
}
