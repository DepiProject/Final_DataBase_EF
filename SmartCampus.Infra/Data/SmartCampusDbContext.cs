using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using SmartCampus.Core.Entities;

namespace SmartCampus.Infra.Data
{
    public class SmartCampusDbContext : IdentityDbContext<AppUser, IdentityRole<int>, int>
    {
        public SmartCampusDbContext(DbContextOptions<SmartCampusDbContext> options)
           : base(options)
        {
        }
        // DbSets - Identity & Academic
        public DbSet<Department> Departments { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<Instructor> Instructors { get; set; }
        public DbSet<Course> Courses { get; set; }

        // DbSets - Enrollment & Grades
        public DbSet<Enrollment> Enrollments { get; set; }
        public DbSet<Grade> Grades { get; set; }

        // DbSets - Exams 
        public DbSet<QuestionType> QuestionTypes { get; set; }
        public DbSet<Exam> Exams { get; set; }
        public DbSet<ExamQuestion> ExamQuestions { get; set; }
        public DbSet<MCQOption> MCQOptions { get; set; }
        public DbSet<TrueFalseQuestion> TrueFalseQuestions { get; set; }

        public DbSet<ExamSubmission> ExamSubmissions { get; set; }
        public DbSet<ExamAnswer> ExamAnswers { get; set; }

        // DbSets - Tracking
       
        public DbSet<Attendance> Attendances { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.ApplyConfigurationsFromAssembly(typeof(SmartCampusDbContext).Assembly);
        }
    }
}
