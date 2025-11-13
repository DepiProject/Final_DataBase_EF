using SmartCampus.App.DTOs;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SmartCampus.App.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly SmartCampusDbContext _context;
        public AttendanceService(SmartCampusDbContext context)
        {
            _context = context;
        }
        public async Task MarkAttendanceAsync(MarkAttendanceDto dto)
        {
            var a = new Attendance
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Date = DateTime.UtcNow,
                Status = dto.Status
            };
            _context.Attendances.Add(a); await _context.SaveChangesAsync();
        }
        public async Task<List<AttendanceDto>> GetStudentHistoryAsync(int studentId)
        {
            return await _context.Attendances.Where(a => a.StudentId == studentId).Select(a => new AttendanceDto
            {
                AttendanceId = a.AttendanceId, CourseId = a.CourseId, Date = a.Date, Status = a.Status
            })
                .ToListAsync();
        }
        public async Task<List<AttendanceDto>> FilterAsync(int? studentId, int? courseId, DateTime? from, DateTime? to)
        {
            var q = _context.Attendances.AsQueryable();
            if (studentId.HasValue) q = q.Where(a => a.StudentId == studentId.Value);
            if (courseId.HasValue) q = q.Where(a => a.CourseId == courseId.Value);
            if (from.HasValue) q = q.Where(a => a.Date >= from.Value);
            if (to.HasValue) q = q.Where(a => a.Date <= to.Value);
            return await q.Select(a => new AttendanceDto
            { 
                AttendanceId = a.AttendanceId,
                CourseId = a.CourseId,
                Date = a.Date,
                Status = a.Status
            })
                .ToListAsync();
        }
        public async Task<object> GetAttendanceSummaryAsync(int studentId, int? courseId = null)
        {
            var query = _context.Attendances.AsQueryable();
            query = query.Where(a => a.StudentId == studentId);

            if (courseId.HasValue)
                query = query.Where(a => a.CourseId == courseId.Value);
            var total = await query.CountAsync();
            var present = await query.CountAsync(a => a.Status == "Present");
            double percentage = total == 0 ? 0 : (present * 100.0 / total);
            return new 
            {
                StudentId = studentId,
                CourseId = courseId,
                TotalClasses = total,
                PresentCount = present,
                AttendancePercentage = Math.Round(percentage, 2)
            };
        }
        public async Task UpdateAttendanceAsync(int id, string status)
        {
            var record = await _context.Attendances.FindAsync(id);
            record!.Status = status; await _context.SaveChangesAsync();
        }
        public async Task DeleteAttendanceAsync(int id)
        {
            var record = await _context.Attendances.FindAsync(id);
            _context.Attendances.Remove(record!);
            await _context.SaveChangesAsync();
        }
    }
}