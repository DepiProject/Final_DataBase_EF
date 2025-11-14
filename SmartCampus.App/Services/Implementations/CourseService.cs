using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.App.Services.Implementations
{
    public class CourseService : ICourseService
    {

        private readonly ICourseRepository _courseRepo;
        private readonly IUserRepository _userRepo;

        public CourseService(ICourseRepository courseRepo, IUserRepository userRepo) 
        {
            _courseRepo = courseRepo;
            _userRepo = userRepo;
        }
        public async Task<IEnumerable<CourseDTO>> GetAllCourses()
        {
            var courses = await _courseRepo.GetAllCourses();
            return courses.Select(c => new CourseDTO
            {
                Name = c.Name,
                CreditHours = c.Credits,
                InstructorId = c.InstructorId
            });
        }
        public async Task<CourseDTO?> GetCourseById(int id)
        {
            var course = await _courseRepo.GetCourseById(id);
            return new CourseDTO
            {
                Name = course.Name,
                CreditHours = course.Credits,
                InstructorId = course.InstructorId
            };
        }

        public async Task<CreateCourseDTO?> AddCourse(CreateCourseDTO courseDto)
        {
            if (courseDto.InstructorId == null) throw new Exception("You must assign Instructor for this new course");
            var course = new Course
            {
                CourseCode = courseDto.CourseCode,
                Name = courseDto.Name,
                Credits = courseDto.CreditHours,
                InstructorId = courseDto.InstructorId,
                DepartmentId = courseDto.DepartmentId
            };
            await _courseRepo.AddCourse(course);
            return new CreateCourseDTO
            {
                CourseCode = courseDto.CourseCode,
                Name = courseDto.Name,
                CreditHours = courseDto.CreditHours,
                InstructorId = courseDto.InstructorId,
                DepartmentId = courseDto.DepartmentId 
            };
        }
        public async Task<CourseDTO?> UpdateCourse(int id, CourseDTO courseDto)
        {
            var courseExist = await _courseRepo.GetCourseById(id);
            if (courseExist == null)
                return null;
            var Newinstructor = courseDto.InstructorId;
            var instExit=await _userRepo.GetInstructorById(Newinstructor);
            if (instExit == null) throw new Exception("This Instructor not exist");
            else
            {
                courseExist.Name = courseDto.Name;
                courseExist.Credits = courseDto.CreditHours;
                courseExist.InstructorId = courseDto.InstructorId;
            }
            var updatedCourse = await _courseRepo.UpdateCourse(courseExist);

            return new CourseDTO
            {
                Name = updatedCourse.Name,
                CreditHours=updatedCourse.Credits,
                InstructorId=updatedCourse.InstructorId
            };
        }
        public async Task<bool> DeleteCourse(int id)
        {

            var course = await _courseRepo.GetCourseById(id);
            if (course == null) return false;

            await _courseRepo.DeleteCourse(id);
            return true;
        }
    }
}
