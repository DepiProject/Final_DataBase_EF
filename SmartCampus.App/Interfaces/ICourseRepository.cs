using SmartCampus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task<Course> GetEnrollmentStudentsByCourseID(int CourseID);
        Task<IEnumerable<Course>> GetAllCoursesByDepartmentID(int DepartmentId);
        Task<Enrollment?> AddEnrollCourse(Enrollment enrollment);
        Task<bool> RemoveEnrollCourse(int enrollmentId);
        Task<IEnumerable<Enrollment>> GetEnrollmentsByStudentId(int studentId);
        Task<IEnumerable<Enrollment?>> GetEnrollmentByStudentIdAndCourseId(int studentId, int courseId);
    }
}
