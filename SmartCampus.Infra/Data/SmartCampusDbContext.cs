using Microsoft.EntityFrameworkCore;
using SmartCampus.Core.Entities;
using SmartCampus.Infra.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartCampus.Infra.Data
{
    public class SmartCampusDbContext :DbContext
    {
        public SmartCampusDbContext(DbContextOptions<SmartCampusDbContext> options)
           : base(options)
        {
        }
        // DbSets - Identity & Academic
        public DbSet<User> Users { get; set; }
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

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Apply all configurations
            modelBuilder.ApplyConfiguration(new UserConfiguration());
            modelBuilder.ApplyConfiguration(new DepartmentConfiguration());
            modelBuilder.ApplyConfiguration(new StudentConfiguration());
            modelBuilder.ApplyConfiguration(new InstructorConfiguration());
            modelBuilder.ApplyConfiguration(new CourseConfiguration());
            modelBuilder.ApplyConfiguration(new EnrollmentConfiguration());
            modelBuilder.ApplyConfiguration(new GradeConfiguration());
            modelBuilder.ApplyConfiguration(new AttendanceConfiguration());
            modelBuilder.ApplyConfiguration(new NotificationConfiguration());

            // Exam Configurations 
            modelBuilder.ApplyConfiguration(new QuestionTypeConfiguration());
            modelBuilder.ApplyConfiguration(new ExamConfiguration());
            modelBuilder.ApplyConfiguration(new ExamQuestionConfiguration());
            modelBuilder.ApplyConfiguration(new MCQOptionConfiguration());
            modelBuilder.ApplyConfiguration(new TrueFalseQuestionConfiguration());

            modelBuilder.ApplyConfiguration(new ExamSubmissionConfiguration());
            modelBuilder.ApplyConfiguration(new ExamAnswerConfiguration());

            modelBuilder.Entity<User>().HasData(new User
            {
                UserId = 1,
                Username = "admin",
                Email = "admin@smartcampus.com",
                PasswordHash = "AQAAAAEAACcQAAAAEB8",
                Role = "Admin"
            });
        }
    }
}
