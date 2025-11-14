using SmartCampus.App.DTOs;
using SmartCampus.App.Interfaces;
using SmartCampus.App.Services.IServices;
using SmartCampus.Core.Entities;
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
            if (courseDto?.InstructorId == null) throw new Exception("You must assign Instructor for this new course");
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
            var instExit = await _userRepo.GetInstructorById(Newinstructor);
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
                CreditHours = updatedCourse.Credits,
                InstructorId = updatedCourse.InstructorId
            };
        }
        public async Task<bool> DeleteCourse(int id)
        {

            var course = await _courseRepo.GetCourseById(id);
            if (course == null) return false;

            await _courseRepo.DeleteCourse(id);
            return true;
        }

        public async Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentStudentsByCourseID(int CourseID)
        {
            var enrollments = await _courseRepo.GetEnrollmentStudentsByCourseID(CourseID);
            if (enrollments == null)
                return new List<studentEnrollmentDTO>();

            return enrollments.Enrollments.Select(e => new studentEnrollmentDTO
            {
                courseName = enrollments.Name,
                studentName = e.Student?.FullName ?? "Unknown"
            });

        }

        public async Task<IEnumerable<EnrollCourseDTO>> GetAllCoursesByDepartmentID(int DepartmentId)
        {
            var courses = await _courseRepo.GetAllCoursesByDepartmentID(DepartmentId);
            return courses.Select(c => new EnrollCourseDTO
            {
                CreditHours = c.Credits,
                CourseName = c.Name,
                courseCode = c.CourseCode,
                DepartmentName = c.Department.Name,

            });
        }

        public async Task<CreateEnrollmentDTO?> AddEnrollCourse(CreateEnrollmentDTO enrollCourseDto)
        {
            if (enrollCourseDto.StudentId == 0 || enrollCourseDto.CourseId == 0)
                throw new Exception("StudentId and CourseId are required to enroll in a course.");
            var existingEnrollment = await _courseRepo.GetEnrollmentByStudentIdAndCourseId(enrollCourseDto.StudentId, enrollCourseDto.CourseId);

            if (existingEnrollment != null)
                throw new Exception("Student is already enrolled in this course.");
            var enrollment = new Enrollment
            {

                CourseId = enrollCourseDto.CourseId,
                StudentId = enrollCourseDto.StudentId,
                EnrollmentDate = DateTime.Now
            };



            await _courseRepo.AddEnrollCourse(enrollment);
            return new CreateEnrollmentDTO
            {
                CourseCode = enrollCourseDto.CourseCode,
                CourseName = enrollCourseDto.CourseName,
                CreditHours = enrollCourseDto.CreditHours
            };

        }
        public async Task<bool> RemoveEnrollCourse(int enrollmentId)
        {
            var enrollmentcourse = await _courseRepo.GetCourseById(enrollmentId);
            if (enrollmentcourse == null) return false;
            await _courseRepo.RemoveEnrollCourse(enrollmentId);
            return true;
        }

        public async Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _courseRepo.GetEnrollmentsByStudentId(studentId);
            return enrollments.Select(c => new studentEnrollmentDTO
            {
                courseName = c.Course.Name,
                studentName = c.Student.FullName,

            });
        }

        public async Task<IEnumerable<InstructorCoursesDTO>> GetCoursesByInstructorId(int instructorId)
        {
            var courses = await _courseRepo.GetCoursesByInstructorId(instructorId);
            return courses.Select(c => new InstructorCoursesDTO
            {
                CourseName = c.Name,
                CourseCode = c.CourseCode,
                DepartmentName = c.Department.Name,
                CreditHours = c.Credits,
                InstructorName = c.Instructor.FullName
            });
        }

        public async Task<IEnumerable<CreateEnrollmentDTO>> GetEnrollmentByStudentIdAndCourseId(int studentId, int courseId)
        {
            var enrollments = await _courseRepo.GetEnrollmentByStudentIdAndCourseId(studentId, courseId);
            return enrollments.Select(e => new CreateEnrollmentDTO
            {
                StudentName = e.Student.FullName,
                CourseName = e.Course.Name,
                CreditHours = e.Course.Credits
            });
        }

    }
}
