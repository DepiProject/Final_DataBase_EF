//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using SmartCampus.App.Dtos;
//using SmartCampus.App.Services.IServices;

//namespace SmartCampus.Controllers
//{
//    [Route("api/[controller]")]
//    [ApiController]
//    public class SubmissionController : ControllerBase
//    {
//        private readonly ISubmissionService _service;
//        public SubmissionController (ISubmissionService service)
//        {
//            _service = service;
//        }


//        [HttpPost("Start")]

//        public async Task<IActionResult>StartExam(int examId, int studentId)
//        {
//            var res = await _service.StartExamAsync(examId, studentId);
//            return Ok(res);
//        }


//        [HttpPost("Submit")]

//        public async Task<IActionResult>SubmitExam([FromBody] SubmitExamDto dto)
//        {
//            var res = await _service.SubmitExamAsync(dto);
//            return Ok(res);

//        }

//        [HttpGet("result/{examId}/{studentId}")]
//        public async Task<IActionResult> GetExamResult(int examId, int studentId)
//        {
//            var result = await _service.GetExamResultAsync(examId, studentId);
//            if (result == null) return NotFound("Exam submission not found.");
//            return Ok(result);
//        }

//    }
//}
