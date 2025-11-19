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

  
        /// Get the number of students currently enrolled in a course
     
        Task<int> GetActiveEnrollmentCountByCourseId(int courseId);

       
        /// Get student's total credit hours for current semester

        Task<int> GetStudentCurrentSemesterCredits(int studentId, DateTime semesterStartDate);


        /// Get student's total credit hours for current academic year
     
        Task<int> GetStudentCurrentYearCredits(int studentId, DateTime yearStartDate);

    
        /// Get the number of active (non-deleted) courses taught by instructor
     
        Task<int> GetInstructorActiveCourseCount(int instructorId);

      
        /// Get instructor's total teaching credit hours
      
        Task<int> GetInstructorTotalCreditHours(int instructorId);


        /// Get list of completed course codes for a student (for prerequisites validation)

        Task<List<string>> GetStudentCompletedCourseCodes(int studentId);

  
        /// Get courses available for a specific department (for student course visibility)
      
        Task<IEnumerable<Course>> GetCoursesByDepartmentForStudent(int departmentId);

    
        /// Check if a course belongs to a specific department
    
        Task<bool> IsCourseBelongsToDepartment(int courseId, int departmentId);
    }
}