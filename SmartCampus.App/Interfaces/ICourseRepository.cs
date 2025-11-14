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
    }
}
