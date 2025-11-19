using Microsoft.EntityFrameworkCore;
using SmartCampus.App.Interfaces;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;

namespace SmartCampus.Infra.Repositories
{
    public class AttendanceRepository : IAttendanceRepository
    {
        private readonly SmartCampusDbContext _context;

        public AttendanceRepository(SmartCampusDbContext context)
        {
            _context = context;
        }

        public async Task<Attendance?> GetAttendanceById(int id)
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .FirstOrDefaultAsync(a => a.AttendanceId == id);
        }

        public async Task<IEnumerable<Attendance>> GetAllAttendances()
        {
            return await _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> GetAttendancesByStudentId(int studentId)
        {
            return await _context.Attendances
                .Where(a => a.StudentId == studentId)
                .Include(a => a.Student)
                .Include(a => a.Course)
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<IEnumerable<Attendance>> FilterAttendances(
            int? studentId,
            int? courseId,
            DateTime? from,
            DateTime? to)
        {
            var query = _context.Attendances
                .Include(a => a.Student)
                .Include(a => a.Course)
                .AsQueryable();

            if (studentId.HasValue)
                query = query.Where(a => a.StudentId == studentId.Value);

            if (courseId.HasValue)
                query = query.Where(a => a.CourseId == courseId.Value);

            if (from.HasValue)
                query = query.Where(a => a.Date.Date >= from.Value.Date);

            if (to.HasValue)
                query = query.Where(a => a.Date.Date <= to.Value.Date);

            return await query
                .OrderByDescending(a => a.Date)
                .ToListAsync();
        }

        public async Task<Attendance?> AddAttendance(Attendance attendance)
        {
            _context.Attendances.Add(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task<Attendance?> UpdateAttendance(Attendance attendance)
        {
            var existingAttendance = await _context.Attendances.FindAsync(attendance.AttendanceId);
            if (existingAttendance == null)
                return null;

            _context.Attendances.Update(attendance);
            await _context.SaveChangesAsync();
            return attendance;
        }

        public async Task<bool> DeleteAttendance(int id)
        {
            var attendance = await _context.Attendances.FindAsync(id);
            if (attendance == null)
                return false;

            _context.Attendances.Remove(attendance);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> AttendanceExists(int studentId, int courseId, DateTime date)
        {
            var dateOnly = date.Date;
            return await _context.Attendances.AnyAsync(a =>
                a.StudentId == studentId &&
                a.CourseId == courseId &&
                a.Date.Date == dateOnly);
        }

        public async Task<bool> StudentExists(int studentId)
        {
            return await _context.Students.AnyAsync(s => s.StudentId == studentId);
        }

        public async Task<bool> CourseExists(int courseId)
        {
            return await _context.Courses.AnyAsync(c => c.CourseId == courseId);
        }
    }
}