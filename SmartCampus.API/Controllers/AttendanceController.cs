using Microsoft.AspNetCore.Mvc;
using SmartCampus.App.DTOs;
using SmartCampus.App.Services.IServices;

namespace SmartCampus.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AttendanceController : ControllerBase
    {
        private readonly IAttendanceService _service;

        public AttendanceController(IAttendanceService service)
        {
            _service = service;
        }

        [HttpPost("mark")]
        public async Task<IActionResult> Mark(MarkAttendanceDto dto)
        {
            await _service.MarkAttendanceAsync(dto);
            return Ok("Attendance marked!");
        }

        [HttpGet("student/{studentId}")]
        public async Task<IActionResult> StudentHistory(int studentId)
        {
            return Ok(await _service.GetStudentHistoryAsync(studentId));
        }

        [HttpGet("filter")]
        public async Task<IActionResult> Filter(int? studentId, int? courseId, DateTime? from, DateTime? to)
        {
            return Ok(await _service.FilterAsync(studentId, courseId, from, to));
        }
        [HttpGet("summary/{studentId}")]
        public async Task<IActionResult> Summary(int studentId, [FromQuery] int? courseId)
        {
            var summary = await _service.GetAttendanceSummaryAsync(studentId, courseId);
            return Ok(summary);
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, [FromBody] string status)
        {
            await _service.UpdateAttendanceAsync(id, status);
            return Ok("Updated");
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _service.DeleteAttendanceAsync(id);
            return Ok("Deleted");
        }
    }

}
