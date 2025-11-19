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

        // ============= BUSINESS RULES CONSTANTS =============
      
        private const int MAX_CREDITS_PER_SEMESTER = 21;
        private const int MAX_CREDITS_PER_YEAR = 36;
        private const int MIN_CREDITS_FOR_FULLTIME = 12;
        private const int MAX_COURSE_CAPACITY = 50;
        private const int MIN_STUDENTS_TO_RUN_COURSE = 5;
        private const int MAX_COURSES_PER_INSTRUCTOR = 2;
        private const int MAX_CREDIT_HOURS_PER_INSTRUCTOR = 12;

        //  Department Restriction: Students can only see/enroll in their department's courses
        private const bool ENFORCE_DEPARTMENT_RESTRICTION = true;

        public CourseService(ICourseRepository courseRepo, IUserRepository userRepo)
        public CourseService(ICourseRepository courseRepo, IUserRepository userRepo)
        {
            _courseRepo = courseRepo;
            _userRepo = userRepo;
        }

        // ============= COURSE MANAGEMENT =============

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
            if (course == null)
                return null;

            return new CourseDTO
            {
                Name = course.Name,
                CreditHours = course.Credits,
                InstructorId = course.InstructorId
            };
        }

        public async Task<CreateCourseDTO?> AddCourse(CreateCourseDTO courseDto)
        {
            // 1. Basic validation
            if (courseDto.InstructorId == 0)
                throw new ArgumentException("Instructor ID is required for creating a course.");

            // 2. Validate instructor exists
            var instructor = await _userRepo.GetInstructorById(courseDto.InstructorId);
            if (instructor == null)
                throw new InvalidOperationException($"Instructor with ID {courseDto.InstructorId} does not exist.");

            // 3.  Validate instructor teaching load
            await ValidateInstructorTeachingLoad(courseDto.InstructorId, courseDto.CreditHours);

            // 4. Validate course code uniqueness
            var existingCourse = await _courseRepo.GetAllCourses();
            if (existingCourse.Any(c => c.CourseCode == courseDto.CourseCode))
                throw new InvalidOperationException($"Course code '{courseDto.CourseCode}' already exists.");

            // 5. Create course
            var course = new Course
            {
                CourseCode = courseDto.CourseCode,
                Name = courseDto.Name,
                Credits = courseDto.CreditHours,
                InstructorId = courseDto.InstructorId,
                DepartmentId = courseDto.DepartmentId,
                IsDeleted = false
            };

            await _courseRepo.AddCourse(course);
            return courseDto;
        }

        public async Task<CourseDTO?> UpdateCourse(int id, CourseDTO courseDto)
        {
            // 1. Check if course exists
            var courseExist = await _courseRepo.GetCourseById(id);
            if (courseExist == null)
                return null;

            // 2. Validate new instructor exists
            var instructor = await _userRepo.GetInstructorById(courseDto.InstructorId);
            if (instructor == null)
                throw new InvalidOperationException($"Instructor with ID {courseDto.InstructorId} does not exist.");

            // 3. If instructor is changing, validate new instructor's load
            if (courseExist.InstructorId != courseDto.InstructorId)
            {
                await ValidateInstructorTeachingLoad(courseDto.InstructorId, courseDto.CreditHours, id);
            }

            // 4. Update course
            courseExist.Name = courseDto.Name;
            courseExist.Credits = courseDto.CreditHours;
            courseExist.InstructorId = courseDto.InstructorId;

            var updatedCourse = await _courseRepo.UpdateCourse(courseExist);
            if (updatedCourse == null)
                return null;

            return new CourseDTO
            {
                Name = updatedCourse.Name,
                CreditHours = updatedCourse.Credits,
                InstructorId = updatedCourse.InstructorId
                CreditHours = updatedCourse.Credits,
                InstructorId = updatedCourse.InstructorId
            };
        }

        public async Task<bool> DeleteCourse(int id)
        {
            var course = await _courseRepo.GetCourseById(id);
            if (course == null)
                return false;

            return await _courseRepo.DeleteCourse(id);
        }

        public async Task<bool> RestoreCourse(int id)
        {
            return await _courseRepo.RestoreCourse(id);
        }

        public async Task<bool> PermanentlyDeleteCourse(int id)
        {
            return await _courseRepo.PermanentlyDeleteCourse(id);
        }

        public async Task<IEnumerable<CourseDTO>> GetAllCoursesIncludingDeleted()
        {
            var courses = await _courseRepo.GetAllCoursesIncludingDeleted();
            return courses.Select(c => new CourseDTO
            {
                Name = c.Name,
                CreditHours = c.Credits,
                InstructorId = c.InstructorId
            });
        }

        // ============= ENROLLMENT WITH BUSINESS RULES =============

        public async Task<CreateEnrollmentDTO?> AddEnrollCourse(CreateEnrollmentDTO enrollCourseDto)
        {
            // 1. Basic validation
            if (enrollCourseDto.StudentId == 0)
                throw new ArgumentException("Student ID is required for enrollment.");

            if (enrollCourseDto.CourseId == 0)
                throw new ArgumentException("Course ID is required for enrollment.");

            // 2. Get the course
            var course = await _courseRepo.GetCourseById(enrollCourseDto.CourseId);
            if (course == null)
                throw new InvalidOperationException("Course does not exist or has been deleted.");

            // 3. Validate department restriction
            await ValidateDepartmentRestriction(enrollCourseDto.StudentId, enrollCourseDto.CourseId);

            // 4. Check if already enrolled
            var existingEnrollment = await _courseRepo.GetEnrollmentByStudentIdAndCourseId(
                enrollCourseDto.StudentId,
                enrollCourseDto.CourseId);

            if (existingEnrollment != null)
                throw new InvalidOperationException("Student is already enrolled in this course.");

            //  5. Validate course capacity
            //await ValidateCourseCapacity(enrollCourseDto.CourseId);

            //  6. Validate student credit limits (semester & year)
            await ValidateStudentCreditLimits(enrollCourseDto.StudentId, course.Credits);

            //  7. Validate prerequisites (if any)
            await ValidatePrerequisites(enrollCourseDto.StudentId, course);

            // 7. Create enrollment with course snapshot
            var enrollment = new Enrollment
            {
                CourseId = enrollCourseDto.CourseId,
                StudentId = enrollCourseDto.StudentId,
                EnrollmentDate = DateTime.UtcNow,
                Status = "Enrolled",

                // Snapshot: preserve course data at enrollment time
                CourseName = course.Name,
                CourseCode = course.CourseCode,
                CreditHours = course.Credits,
                DepartmentName = course.Department?.Name ?? "Unknown"
            };

            await _courseRepo.AddEnrollCourse(enrollment);

            // 8. Return enriched DTO
            return new CreateEnrollmentDTO
            {
                StudentId = enrollCourseDto.StudentId,
                CourseId = enrollCourseDto.CourseId,
                CourseCode = course.CourseCode,
                CourseName = course.Name,
                CreditHours = course.Credits
            };
        }

        public async Task<bool> RemoveEnrollCourse(int enrollmentId)
        {
            return await _courseRepo.RemoveEnrollCourse(enrollmentId);
        }

        public async Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentsByStudentId(int studentId)
        {
            var enrollments = await _courseRepo.GetEnrollmentsByStudentId(studentId);
            return enrollments.Select(e => new studentEnrollmentDTO
            {
                courseName = e.CourseName,
                courseCode = e.CourseCode,
                CreditHours = e.CreditHours,
                DepartmentName = e.DepartmentName,
                studentName = e.Student?.FullName ?? "Unknown"
            });
        }

        public async Task<IEnumerable<studentEnrollmentDTO>> GetEnrollmentStudentsByCourseID(int courseId)
        {
            var course = await _courseRepo.GetEnrollmentStudentsByCourseID(courseId);
            if (course == null || course.Enrollments == null)
                return new List<studentEnrollmentDTO>();

            return course.Enrollments.Select(e => new studentEnrollmentDTO
            {
                courseName = e.CourseName,
                courseCode = e.CourseCode,
                CreditHours = e.CreditHours,
                DepartmentName = e.DepartmentName,
                studentName = e.Student?.FullName ?? "Unknown"
            });
        }

        // ============= QUERY METHODS =============

        public async Task<IEnumerable<EnrollCourseDTO>> GetAllCoursesByDepartmentID(int departmentId)
        {
            var courses = await _courseRepo.GetAllCoursesByDepartmentID(departmentId);
            return courses.Select(c => new EnrollCourseDTO
            {
                CreditHours = c.Credits,
                CourseName = c.Name,
                courseCode = c.CourseCode,
                DepartmentName = c.Department?.Name ?? "Unknown"
            });
        }

       
        ///   Get available courses for a specific student (filtered by their department)
        /// This ensures students only see courses from their own department
     
        public async Task<IEnumerable<EnrollCourseDTO>> GetAvailableCoursesForStudent(int studentId)
        {
            // Get student to find their department
            var student = await _userRepo.GetStudentById(studentId);
            if (student == null)
                throw new InvalidOperationException("Student not found.");

            if (!student.DepartmentId.HasValue)
                throw new InvalidOperationException("Student is not assigned to any department.");

            // Get courses only from student's department
            var courses = await _courseRepo.GetCoursesByDepartmentForStudent(student.DepartmentId.Value);

            return courses.Select(c => new EnrollCourseDTO
            {
                CreditHours = c.Credits,
                CourseName = c.Name,
                courseCode = c.CourseCode,
                DepartmentName = c.Department?.Name ?? "Unknown"
            });
        }

        public async Task<IEnumerable<InstructorCoursesDTO>> GetCoursesByInstructorId(int instructorId)
        public async Task<IEnumerable<InstructorCoursesDTO>> GetCoursesByInstructorId(int instructorId)
        {
            var courses = await _courseRepo.GetCoursesByInstructorId(instructorId);
            return courses.Select(c => new InstructorCoursesDTO
            {
            {
                CourseName = c.Name,
                CourseCode = c.CourseCode,
                DepartmentName = c.Department?.Name ?? "Unknown",
                CreditHours = c.Credits,
                InstructorName = c.Instructor?.FullName ?? "Unknown"
            });
        }

        // ============= BUSINESS RULES VALIDATION METHODS =============

           // التحقق من أن الطالب يسجل فقط في كورسات قسمه
            private async Task ValidateDepartmentRestriction(int studentId, int courseId)
        {
            if (!ENFORCE_DEPARTMENT_RESTRICTION)
                return; // Feature disabled

            // Get the student to check their department
            var student = await _userRepo.GetStudentById(studentId);
            if (student == null)
                throw new InvalidOperationException("Student not found.");

            if (!student.DepartmentId.HasValue)
                throw new InvalidOperationException("Student is not assigned to any department.");

            // Check if course belongs to student's department
            var courseBelongsToDepartment = await _courseRepo.IsCourseBelongsToDepartment(
                courseId,
                student.DepartmentId.Value);

            if (!courseBelongsToDepartment)
            {
                throw new InvalidOperationException(
                    $"Access denied: This course is not available for your department. " +
                    $"You can only enroll in courses from your department.");
            }
        }

        /// التحقق من حد الساعات للطالب (فصل دراسي وسنة أكاديمية)
       
        private async Task ValidateStudentCreditLimits(int studentId, int newCourseCredits)
        {
            // حساب الساعات في الفصل الحالي
            var semesterStartDate = GetCurrentSemesterStartDate();
            var semesterCredits = await _courseRepo.GetStudentCurrentSemesterCredits(studentId, semesterStartDate);

            if (semesterCredits + newCourseCredits > MAX_CREDITS_PER_SEMESTER)
            {
                throw new InvalidOperationException(
                    $"Cannot enroll: Semester credit limit exceeded. " +
                    $"Current: {semesterCredits} credits, " +
                    $"New course: {newCourseCredits} credits, " +
                    $"Maximum allowed: {MAX_CREDITS_PER_SEMESTER} credits per semester.");
            }

            // حساب الساعات في السنة الأكاديمية
            var yearStartDate = GetCurrentAcademicYearStartDate();
            var yearCredits = await _courseRepo.GetStudentCurrentYearCredits(studentId, yearStartDate);

            if (yearCredits + newCourseCredits > MAX_CREDITS_PER_YEAR)
            {
                throw new InvalidOperationException(
                    $"Cannot enroll: Annual credit limit exceeded. " +
                    $"Current year: {yearCredits} credits, " +
                    $"New course: {newCourseCredits} credits, " +
                    $"Maximum allowed: {MAX_CREDITS_PER_YEAR} credits per year.");
            }

            // تحذير: إذا كان الطالب أقل من الحد الأدنى (اختياري - للعرض فقط)
            if (semesterCredits + newCourseCredits < MIN_CREDITS_FOR_FULLTIME)
            {
                // يمكنك تسجيل تحذير أو إرساله للـ user
                Console.WriteLine($"⚠️ Warning: Student will have {semesterCredits + newCourseCredits} credits, " +
                    $"below full-time minimum of {MIN_CREDITS_FOR_FULLTIME} credits.");
            }
        }

        /// <summary>
        ///  التحقق من سعة الكورس (عدد الطلاب المسجلين)
        /// </summary>
        //private async Task ValidateCourseCapacity(int courseId)
        //{
        //    var enrolledCount = await _courseRepo.GetActiveEnrollmentCountByCourseId(courseId);

        //    if (enrolledCount >= MAX_COURSE_CAPACITY)
        //    {
        //        throw new InvalidOperationException(
        //            $"Cannot enroll: Course is full. " +
        //            $"Current enrollment: {enrolledCount}/{MAX_COURSE_CAPACITY} students.");
        //    }

        //    // تحذير: إذا كان قريب من الامتلاء (اختياري)
        //    if (enrolledCount >= MAX_COURSE_CAPACITY - 5)
        //    {
        //        var remaining = MAX_COURSE_CAPACITY - enrolledCount;
        //        Console.WriteLine($"⚠️ Warning: Only {remaining} seats remaining in this course.");
        //    }
        //}

        ///  التحقق من حمل التدريس للأستاذ
   
        private async Task ValidateInstructorTeachingLoad(int instructorId, int newCourseCredits, int? excludeCourseId = null)
        {
            // عدد الكورسات الحالية
            var courseCount = await _courseRepo.GetInstructorActiveCourseCount(instructorId);

            // إذا كان update، لا نحسب الكورس الحالي
            if (excludeCourseId.HasValue)
            {
                var currentCourse = await _courseRepo.GetCourseById(excludeCourseId.Value);
                if (currentCourse?.InstructorId == instructorId)
                {
                    courseCount--;
                }
            }

            // التحقق من عدد الكورسات
            if (courseCount >= MAX_COURSES_PER_INSTRUCTOR)
            {
                throw new InvalidOperationException(
                    $"Cannot assign instructor: Maximum course limit reached. " +
                    $"Instructor is already teaching {courseCount} courses. " +
                    $"Maximum allowed: {MAX_COURSES_PER_INSTRUCTOR} courses.");
            }

            // مجموع ساعات التدريس
            var totalHours = await _courseRepo.GetInstructorTotalCreditHours(instructorId);

            // إذا كان update، نطرح ساعات الكورس الحالي
            if (excludeCourseId.HasValue)
            {
                var currentCourse = await _courseRepo.GetCourseById(excludeCourseId.Value);
                if (currentCourse?.InstructorId == instructorId)
                {
                    totalHours -= currentCourse.Credits;
                }
            }

            // التحقق من ساعات التدريس
            if (totalHours + newCourseCredits > MAX_CREDIT_HOURS_PER_INSTRUCTOR)
            {
                throw new InvalidOperationException(
                    $"Cannot assign instructor: Maximum teaching hours exceeded. " +
                    $"Current: {totalHours} hours, " +
                    $"New course: {newCourseCredits} hours, " +
                    $"Maximum allowed: {MAX_CREDIT_HOURS_PER_INSTRUCTOR} hours.");
            }
        }

  
        ///  التحقق من المتطلبات السابقة
       
        private async Task ValidatePrerequisites(int studentId, Course course)
        {
            // إذا لا توجد متطلبات سابقة، نسمح بالتسجيل
            if (string.IsNullOrEmpty(course.Prerequisites))
                return;

            // تحليل الـ prerequisites (مثال: "CS101,CS102")
            var prerequisiteCodes = course.Prerequisites
                .Split(',', StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            if (!prerequisiteCodes.Any())
                return;

            // الحصول على الكورسات المكتملة للطالب
            var completedCourses = await _courseRepo.GetStudentCompletedCourseCodes(studentId);

            // التحقق من الكورسات الناقصة
            var missingPrerequisites = prerequisiteCodes
                .Where(prereq => !completedCourses.Contains(prereq))
                .ToList();

            if (missingPrerequisites.Any())
            {
                throw new InvalidOperationException(
                    $"Cannot enroll: Missing required prerequisites. " +
                    $"Required courses: {string.Join(", ", missingPrerequisites)}");
            }
        }

        // ============= HELPER METHODS =============

 
        /// تحديد تاريخ بداية الفصل الحالي
        /// Fall: Sep-Dec, Spring: Jan-May, Summer: Jun-Aug
     
        private DateTime GetCurrentSemesterStartDate()
        {
            var now = DateTime.UtcNow;

            if (now.Month >= 9 && now.Month <= 12) // Fall
                return new DateTime(now.Year, 9, 1);
            else if (now.Month >= 1 && now.Month <= 5) // Spring
                return new DateTime(now.Year, 1, 1);
            else // Summer
                return new DateTime(now.Year, 6, 1);
        }

 
        /// تحديد تاريخ بداية السنة الأكاديمية (عادة سبتمبر)
    
        private DateTime GetCurrentAcademicYearStartDate()
        {
            var now = DateTime.UtcNow;

            if (now.Month < 9)
                return new DateTime(now.Year - 1, 9, 1);
            else
                return new DateTime(now.Year, 9, 1);
        }

        // ============= ADDITIONAL UTILITY METHODS =============

   
        /// التحقق من إمكانية تشغيل الكورس (الحد الأدنى من الطلاب)
  
        public async Task<bool> CanCourseRun(int courseId)
        {
            var enrolledCount = await _courseRepo.GetActiveEnrollmentCountByCourseId(courseId);
            return enrolledCount >= MIN_STUDENTS_TO_RUN_COURSE;
        }


        /// الحصول على عدد المقاعد المتبقية في الكورس

        //public async Task<int> GetAvailableSeats(int courseId)
        //{
        //    var enrolledCount = await _courseRepo.GetActiveEnrollmentCountByCourseId(courseId);
        //    return Math.Max(0, MAX_COURSE_CAPACITY - enrolledCount);
        //}
    }
}
