using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SmartCampus.App.DTOs;
using SmartCampus.App.Services.IServices;

namespace SmartCampus.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ExamsController : ControllerBase
    {
        private readonly IExamService _examService;

        public ExamsController(IExamService examService)
        {
            _examService = examService;
        }

        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            var exams = await _examService.GetAllAsync();
            return Ok(exams);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetById(int id)
        {
            var exam = await _examService.GetByIdAsync(id);
            if (exam == null)
                return NotFound(new { message = "Exam not found" });
            return Ok(exam);
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CreateExamDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exam = await _examService.CreateAsync(dto);
            return Ok(new { message = "Exam created successfully", exam });
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] CreateExamDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var exam = await _examService.UpdateAsync(id, dto);
            if (exam == null)
                return NotFound(new { message = "Exam not found" });

            return Ok(new { message = "Exam updated successfully", exam });
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            var deleted = await _examService.DeleteAsync(id);
            if (!deleted)
                return NotFound(new { message = "Exam not found" });

            return Ok(new { message = "Exam deleted successfully" });
        }

        [HttpPost("{examId}/questions")]
        public async Task<IActionResult> AddQuestion(int examId, [FromBody] ExamQuestionDto questionDto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var question = await _examService.AddQuestionToExamAsync(examId, questionDto);
                return Ok(new { message = "Question added successfully", question });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpDelete("{examId}/questions/{questionId}")]
        public async Task<IActionResult> RemoveQuestion(int examId, int questionId)
        {
            var deleted = await _examService.RemoveQuestionFromExamAsync(examId, questionId);
            if (!deleted)
                return NotFound(new { message = "Question not found" });

            return Ok(new { message = "Question removed successfully" });
        }
    }
}
