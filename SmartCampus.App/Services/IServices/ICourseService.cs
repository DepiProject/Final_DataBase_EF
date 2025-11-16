

// ICourseService.cs
using SmartCampus.App.DTOs;

namespace SmartCampus.App.Services.IServices
{
    public interface ICourseService
    {
        Task<CourseDTO?> GetCourseById(int id);
        Task<IEnumerable<CourseDTO>> GetAllCourses();
        Task<CreateCourseDTO?> AddCourse(CreateCourseDTO courseDto);
        Task<CourseDTO?> UpdateCourse(int id, CourseDTO courseDto);
        Task<bool> DeleteCourse(int id);
        Task<IEnumerable<InstructorCoursesDTO>> GetCoursesByInstructorId(int instructorId);
        Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentStudentsByCourseID(int courseId);
        Task<IEnumerable<EnrollCourseDTO>> GetAllCoursesByDepartmentID(int departmentId);
        Task<CreateEnrollmentDTO?> AddEnrollCourse(CreateEnrollmentDTO enrollCourseDto);
        Task<bool> RemoveEnrollCourse(int enrollmentId);
        Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentsByStudentId(int studentId);
        Task<bool> RestoreCourse(int id);
        Task<bool> PermanentlyDeleteCourse(int id);
        Task<IEnumerable<CourseDTO>> GetAllCoursesIncludingDeleted();
    }
}