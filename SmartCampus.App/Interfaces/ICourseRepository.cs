using SmartCampus.App.DTOs;
using SmartCampus.Core.Entities;

namespace SmartCampus.App.Interfaces
{
    public interface ICourseRepository
    {
        Task<Course?> GetCourseById(int id);
        Task<IEnumerable<Course>> GetAllCourses();
        Task<Course?> AddCourse(Course course);
        Task<Course?> UpdateCourse(Course course);
        Task<bool> DeleteCourse(int id);
        Task<IEnumerable<Course>> GetCoursesByInstructorId(int instructorId);
        Task<Course?> GetEnrollmentStudentsByCourseID(int courseId);
        Task<IEnumerable<Course>> GetAllCoursesByDepartmentID(int departmentId);
        Task<Enrollment?> AddEnrollCourse(Enrollment enrollment);
        Task<bool> RemoveEnrollCourse(int enrollmentId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentId(int studentId);
        Task<Enrollment?> GetEnrollmentByStudentIdAndCourseId(int studentId, int courseId);
        Task<bool> IsStudentEnrolledInCourse(int studentId, int courseId);

        // New methods for soft delete management
        Task<bool> RestoreCourse(int id);
        Task<bool> PermanentlyDeleteCourse(int id);
        Task<IEnumerable<Course>> GetAllCoursesIncludingDeleted();
    }
}