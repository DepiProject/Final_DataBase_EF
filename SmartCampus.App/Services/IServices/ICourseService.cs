using SmartCampus.App.DTOs;

namespace SmartCampus.App.Services.IServices
{
    public interface ICourseService
    {
        // ============= COURSE MANAGEMENT =============
        Task<CourseDTO?> GetCourseById(int id);
        Task<IEnumerable<CourseDTO>> GetAllCourses();
        Task<CreateCourseDTO?> AddCourse(CreateCourseDTO courseDto);
        Task<CourseDTO?> UpdateCourse(int id, CourseDTO courseDto);
        Task<bool> DeleteCourse(int id);

        // ============= SOFT DELETE MANAGEMENT =============
        Task<bool> RestoreCourse(int id);
        Task<bool> PermanentlyDeleteCourse(int id);
        Task<IEnumerable<CourseDTO>> GetAllCoursesIncludingDeleted();

        // ============= COURSE QUERIES =============
        Task<IEnumerable<InstructorCoursesDTO>> GetCoursesByInstructorId(int instructorId);
        Task<IEnumerable<EnrollCourseDTO>> GetAllCoursesByDepartmentID(int departmentId);

       //Get available courses for a specific student (filtered by their department)
        /// Students can only see courses from their own department
    
        Task<IEnumerable<EnrollCourseDTO>> GetAvailableCoursesForStudent(int studentId);

        // ============= ENROLLMENT MANAGEMENT =============
        Task<CreateEnrollmentDTO?> AddEnrollCourse(CreateEnrollmentDTO enrollCourseDto);
        Task<bool> RemoveEnrollCourse(int enrollmentId);
        Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentsByStudentId(int studentId);
        Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentStudentsByCourseID(int courseId);
        Task<bool> CanCourseRun(int courseId);
        //Task<int> GetAvailableSeats(int courseId);

    }
}