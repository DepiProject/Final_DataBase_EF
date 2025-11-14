using SmartCampus.App.DTOs;
using SmartCampus.Core.Entities;


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
        Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentStudentsByCourseID(int CourseID);
        Task<IEnumerable<EnrollCourseDTO>> GetAllCoursesByDepartmentID(int DepartmentId);
        Task<CreateEnrollmentDTO?> AddEnrollCourse(CreateEnrollmentDTO enrollCourseDto);
        Task<bool> RemoveEnrollCourse(int enrollmentId);
        Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentsByStudentId(int studentId);
        Task<IEnumerable<CreateEnrollmentDTO>> GetEnrollmentByStudentIdAndCourseId(int studentId, int courseId);


    }
}
