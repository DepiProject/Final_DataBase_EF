using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Services.Implementations
{
    public class AttendanceService : IAttendanceService
    {
        private readonly IAttendanceRepository _attendanceRepo;
        private readonly List<string> _validStatuses = new() { "Present", "Absent", "Late", "Excused" };

        public AttendanceService(IAttendanceRepository attendanceRepo)
        {
            _attendanceRepo = attendanceRepo;
        }

        public async Task MarkAttendanceAsync(MarkAttendanceDto dto)
        {
            
            if (!_validStatuses.Contains(dto.Status)) throw new ArgumentException($"Invalid status. Must be one of: {string.Join(", ", _validStatuses)}");           
            if (!await _attendanceRepo.StudentExists(dto.StudentId)) throw new InvalidOperationException($"Student with ID {dto.StudentId} not found.");
            if (!await _attendanceRepo.CourseExists(dto.CourseId)) throw new InvalidOperationException($"Course with ID {dto.CourseId} not found.");
            if (await _attendanceRepo.AttendanceExists(dto.StudentId, dto.CourseId, dto.Date))
            {
                throw new InvalidOperationException(
                    $"Attendance already marked for student {dto.StudentId} in course {dto.CourseId} on {dto.Date.Date:yyyy-MM-dd}");
            }
            var attendance = new Attendance
            {
                StudentId = dto.StudentId,
                CourseId = dto.CourseId,
                Date = dto.Date,
                Status = dto.Status
            };
            await _attendanceRepo.AddAttendance(attendance);
        }

        public async Task<List<AttendanceDto>> GetStudentHistoryAsync(int studentId)
        {
            if (!await _attendanceRepo.StudentExists(studentId)) throw new InvalidOperationException($"Student with ID {studentId} not found.");
            var attendances = await _attendanceRepo.GetAttendancesByStudentId(studentId);
            return attendances.Select(a => new AttendanceDto
            {
                AttendanceId = a.AttendanceId,
                StudentId = a.StudentId,
                StudentName = a.Student?.FullName ?? string.Empty,
                CourseId = a.CourseId,
                CourseName = a.Course?.Name ?? string.Empty,
                Date = a.Date,
                Status = a.Status
            }).ToList();
        }
        public async Task<List<AttendanceDto>> FilterAsync(
            int? studentId,
            int? courseId,
            DateTime? from,
            DateTime? to)
        {
            var attendances = await _attendanceRepo.FilterAttendances(studentId, courseId, from, to);

            return attendances.Select(a => new AttendanceDto
            {
                AttendanceId = a.AttendanceId,
                StudentId = a.StudentId,
                StudentName = a.Student?.FullName ?? string.Empty,
                CourseId = a.CourseId,
                CourseName = a.Course?.Name ?? string.Empty,
                Date = a.Date,
                Status = a.Status
            }).ToList();
        }

        public async Task<object> GetAttendanceSummaryAsync(int studentId, int? courseId = null)
        {
            // Verify student exists
            if (!await _attendanceRepo.StudentExists(studentId))
            {
                throw new InvalidOperationException($"Student with ID {studentId} not found.");
            }

            var attendances = await _attendanceRepo.FilterAttendances(studentId, courseId, null, null);
            var attendanceList = attendances.ToList();

            var total = attendanceList.Count;
            var present = attendanceList.Count(a => a.Status == "Present");
            var late = attendanceList.Count(a => a.Status == "Late");
            var absent = attendanceList.Count(a => a.Status == "Absent");
            var excused = attendanceList.Count(a => a.Status == "Excused");

            double percentage = total == 0 ? 0 : (present * 100.0 / total);

            return new
            {
                StudentId = studentId,
                CourseId = courseId,
                TotalClasses = total,
                PresentCount = present,
                LateCount = late,
                AbsentCount = absent,
                ExcusedCount = excused,
                AttendancePercentage = Math.Round(percentage, 2)
            };
        }

        public async Task UpdateAttendanceAsync(int id, string status)
        {
            // Validate status
            if (!_validStatuses.Contains(status))
            {
                throw new ArgumentException($"Invalid status. Must be one of: {string.Join(", ", _validStatuses)}");
            }

            var existingAttendance = await _attendanceRepo.GetAttendanceById(id);
            if (existingAttendance == null)
            {
                throw new InvalidOperationException($"Attendance record with ID {id} not found.");
            }

            existingAttendance.Status = status;
            await _attendanceRepo.UpdateAttendance(existingAttendance);
        }

        public async Task DeleteAttendanceAsync(int id)
        {
            var deleted = await _attendanceRepo.DeleteAttendance(id);
            if (!deleted)
            {
                throw new InvalidOperationException($"Attendance record with ID {id} not found.");
            }
        }
    }
}