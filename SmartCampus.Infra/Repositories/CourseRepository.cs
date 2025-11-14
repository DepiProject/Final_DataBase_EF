using Microsoft.EntityFrameworkCore;
using SmartCampus.App.Interfaces;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Data;

namespace SmartCampus.Infra.Repositories
{
    public class CourseRepository: ICourseRepository
    {
        private readonly SmartCampusDbContext _context;


        public CourseRepository(SmartCampusDbContext context)
        {
            _context = context;
        }
        public async Task<IEnumerable<Course>> GetAllCourses()
        {
            return await _context.Courses.ToListAsync();
        }

        public async Task<Course?> GetCourseById(int id)
        {
            return await _context.Courses.FindAsync(id);
        }
        public async Task<Course?> AddCourse(Course course)
        {
            _context.Courses.Add(course);
            await _context.SaveChangesAsync();
            return course;
        }

        public async Task<Course?> UpdateCourse(Course course)
        {
            var courseExit = _context.Courses.FindAsync(course.CourseId);
            if (courseExit != null)
            {
                _context.Courses.Update(course);
                await _context.SaveChangesAsync();
                return course;
            }
            return null;
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var hasExams = _context.Exams.Any(e => e.CourseId == id);
            if (hasExams)
            {
                throw new InvalidOperationException("Cannot delete course with existing exams.");
            }
            var courseExit = await _context.Courses.FindAsync(id);
            if (courseExit != null)
            {
                _context.Courses.Remove(courseExit);
                await _context.SaveChangesAsync();
                return true;
            }
            return false;
        }



    }
}
