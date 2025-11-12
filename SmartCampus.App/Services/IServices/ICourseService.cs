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
    }
}
