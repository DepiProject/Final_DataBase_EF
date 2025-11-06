using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;


namespace SmartCampus.Infra.Configuration
{
    public class StudentConfiguration : IEntityTypeConfiguration<Student>
    {
        public void Configure(EntityTypeBuilder<Student> builder)
        {
            builder.ToTable("Students");
            builder.HasKey(s => s.StudentId);
            builder.Property(s => s.FullName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(s => s.StudentCode)
                .IsRequired()
                .HasMaxLength(20);

            builder.HasIndex(s => s.StudentCode)
                .IsUnique();

            builder.Property(s => s.ContactNumber)
                 .HasMaxLength(20);

            builder.Property(s => s.Level)
                .IsRequired()
                .HasMaxLength(20);

            builder.Property(s => s.GPA)
                .HasColumnType("decimal(3, 3)");

            // prevent duplication
            builder.HasAlternateKey(s => s.StudentCode)
                   .HasName("AK_StudentCode");

            // Relationships
            // Student → Enrollments - NO CASCADE (would conflict with Course cascade)
            builder.HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student → Attendances - NO CASCADE (would conflict with Course cascade)
            builder.HasMany(s => s.Attendances)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student → ExamSubmissions - NO CASCADE (would conflict with Exam cascade)
            builder.HasMany(s => s.ExamSubmissions)
                .WithOne(es => es.Student)
                .HasForeignKey(es => es.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
