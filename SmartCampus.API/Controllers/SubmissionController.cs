using Microsoft.AspNetCore.Mvc;
using SmartCampus.App.Dtos;
using SmartCampus.App.Services.IServices;

namespace SmartCampus.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class SubmissionController : ControllerBase
    {
        private readonly ISubmissionService _submissionService;

        public SubmissionController(ISubmissionService submissionService)
        {
            _submissionService = submissionService;
        }

   
        [HttpPost("start")]
        public async Task<IActionResult> StartExam([FromQuery] int examId, [FromQuery] int studentId)
        {
            try
            {
                var submission = await _submissionService.StartExamAsync(examId, studentId);
                return Ok(new
                {
                    message = "Exam started successfully.",
                    submissionId = submission.SubmissionId,
                    examId = submission.ExamId,
                    studentId = submission.StudentId,
                    startedAt = submission.StartedAt
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

       
        [HttpPost("submit")]
        public async Task<IActionResult> SubmitExam([FromBody] SubmitExamDto dto)
        {
            try
            {
                var result = await _submissionService.SubmitExamAsync(dto);
                return Ok(new
                {
                    message = "Exam submitted successfully.",
                    result
                });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet("result/{examId}/{studentId}")]
        public async Task<IActionResult> GetExamResult(int examId, int studentId)
        {
            var result = await _submissionService.GetExamResultAsync(examId, studentId);
            if (result == null)
                return NotFound(new { message = "Exam submission not found." });

            return Ok(result);
        }
    }
}

