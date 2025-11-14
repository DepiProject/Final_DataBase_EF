using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCampus.App.DTOs;
using SmartCampus.App.Services.IServices;

namespace SmartCampus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAllExams()
        {
            var exams = await _examService.GetAllExams();
            return Ok(exams);
        }
        [HttpGet("{CourseId}")]
        public async Task<IActionResult> GetCourseExams(int CourseId)
        {
            var exams = await _examService.GetAllExamsForCourse(CourseId);
            return Ok(exams);
        }

        [HttpGet("{id}/{courseId}")]
        public async Task<IActionResult> GetById(int id, int courseId)
        {
            var exam = await _examService.GetExamById(id, courseId);
            if (exam == null)
                return NotFound(new { message = "This Course has no Exams" });
            return Ok(exam);
        }

        [HttpPost]
        public async Task<IActionResult> CreateExam([FromBody] CreateExamDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exam = await _examService.AddExam(dto);
            return Ok(new { message = "Exam created successfully", exam });
        }
        //[HttpPost]
        //public async Task<IActionResult> AddQuestion([FromBody] ExamQuestionDto dto)
        //{
        //    if (!ModelState.IsValid)
        //        return BadRequest(ModelState);

        //    var exam = await _examService.AddExamQuestion(dto);
        //    return Ok(new { message = "Exam created successfully", exam });
        //}

        //[HttpDelete("{id}/{courseId}")]
        //public async Task<IActionResult> DeleteExam(int id, int courseId)
        //{
        //    var deleted = await _examService.DeleteExam(id, courseId);
        //    if (!deleted)
        //        return NotFound(new { message = "Exam not found" });

        //    return Ok(new { message = "Exam deleted successfully" });
        //}
    }
}
