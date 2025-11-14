using Microsoft.EntityFrameworkCore.Diagnostics;
using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.Services.Implementations
{
    public class ExamService:IExamService
    {
        private readonly IExamRepository _examRepository;
        public ExamService(IExamRepository examRepository)
        {
            _examRepository = examRepository;
        }

        public async Task<IEnumerable<ExamDTO>> GetAllExams()
        {
            var exams = await _examRepository.GetAllExams();
            return exams.Select(e => new ExamDTO
            {
                CourseName = e.Course?.Name,
                Title = e.Title,
                ExamDate = e.ExamDate,
                Duration = e.Duration
            });
        }
        public async Task<IEnumerable<ExamDTO>> GetAllExamsForCourse(int id)
        {
            var exams = await _examRepository.GetAllExamsForCourse(id);
            return exams.Select(e => new ExamDTO
            {
                CourseName = e.Course.Name,
                Title = e.Title,
                ExamDate = e.ExamDate,
                Duration = e.Duration


            });
        }
        public async Task<ExamDTO?> GetExamById(int id, int courseID)
        {
            var exam = await _examRepository.GetExamById(id, courseID);
            if (exam == null)
                throw new Exception("This Course ha no exam with this id ");
            return new ExamDTO
            {
                CourseName = exam.Course?.Name,
                Title = exam.Title,
                ExamDate = exam.ExamDate,
                Duration = exam.Duration

            };
        }
        public async Task<CreateExamDto?> AddExam(CreateExamDto examDto)
        {
            var exam = new Exam
            {
                Title = examDto.Title,
                ExamDate = examDto.ExamDate,
                Duration = examDto.Duration,
                TotalPoints = examDto.TotalPoints,
                CourseId = examDto.CourseId
            };
            if (examDto?.CourseId == null) throw new Exception("You must assign course for this exam ");
            await _examRepository.AddExam(exam);
            return new CreateExamDto
            {
                CourseId = examDto.CourseId,
                Title = examDto.Title,
                ExamDate = examDto.ExamDate,
                Duration = examDto.Duration,
                TotalPoints = examDto.TotalPoints
            };
        }



        //public async Task<ExamQuestionDto?> AddExamQuestion(ExamQuestionDto examQuestionDto)
        //{
        //    var examQuestion = new ExamQuestion
        //    {
        //        QuestionText = examQuestionDto.QuestionText,
        //        Score = examQuestionDto.Score,
        //        OrderNumber = examQuestionDto.OrderNumber,
        //        ExamId = examQuestionDto.ExamID,
        //        TypeId = examQuestionDto.QuestionType
        //    };
        //    if (examQuestionDto?.ExamID == null) throw new Exception("You must assign Exam for this Question ");
        //    await _examRepository.AddExamQuestion(examQuestion);
        //    //return new ExamQuestionDto
        //    //{
        //    //    QuestionText = examQuestionDto.QuestionText,
        //    //    Score = examQuestionDto.Score,
        //    //    OrderNumber = examQuestionDto.OrderNumber,
        //    //    ExamID = examQuestionDto.ExamID,
        //    //    QuestionType = examQuestionDto.QuestionType
        //    //};

        //    if (examQuestionDto?.QuestionType == 1)
        //    {
        //        var mcqQuestion = new MCQOption();
        //        for (int i = 0; i < 4; i++)
        //        {
        //            mcqQuestion = new MCQOption
        //            {
        //                OptionText=examQuestionDto.QuestionText,
        //                OrderNumber=examQuestionDto.OrderNumber,
        //                QuestionId = examQuestionDto.QuestionId,
        //                IsCorrect=false
        //            };
        //            await _examRepository.AddExamMcqOption(mcqQuestion);
        //        }
        //    }
        //    else if (examQuestionDto?.QuestionType == 2)
        //    {
        //         //tfQuestion = new TrueFalseQuestion();

        //        var tfQuestion = new TrueFalseQuestion
        //            {
        //                QuestionText = examQuestionDto.QuestionText,
        //                //OrderNumber = examQuestionDto.OrderNumber,
        //                QuestionId = examQuestionDto.QuestionId,
        //                IsCorrect = false
        //            };
        //            await _examRepository.AddExamTFQuestion(tfQuestion);
                
        //    }
        //    else
        //    {
        //        throw new Exception("Question Type Id must be 1 or 2");
        //    }
        //    return examQuestionDto;
        //}
        //public async Task<bool> DeleteExam(int id, int courseId)
        //{
        //    var exam = await _examRepository.GetExamById(id, courseId);
        //    if (exam == null) throw new Exception("Exam Not Found");

        //    await _examRepository.DeleteExam(id, courseId);

        //    return true;
        //}
    }
}
