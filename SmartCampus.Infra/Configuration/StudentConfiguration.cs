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

            // FIXED: Changed from decimal(3,3) to decimal(3,2)
            // This allows values from 0.00 to 9.99
            // If you need 0.00 to 4.00, use decimal(3,2)
            // If you need 0.000 to 4.000, use decimal(4,3)
            builder.Property(s => s.GPA)
                .HasColumnType("decimal(3, 2)");

            // Prevent duplication - use HasIndex with IsUnique instead of HasAlternateKey
            // HasAlternateKey is typically for foreign key references
            builder.HasIndex(s => s.StudentCode)
                .IsUnique()
                .HasDatabaseName("IX_Students_StudentCode");

            // Relationships
            // Student → Enrollments - NO CASCADE
            builder.HasMany(s => s.Enrollments)
                .WithOne(e => e.Student)
                .HasForeignKey(e => e.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student → Attendances - NO CASCADE
            builder.HasMany(s => s.Attendances)
                .WithOne(a => a.Student)
                .HasForeignKey(a => a.StudentId)
                .OnDelete(DeleteBehavior.Restrict);

            // Student → ExamSubmissions - NO CASCADE
            builder.HasMany(s => s.ExamSubmissions)
                .WithOne(es => es.Student)
                .HasForeignKey(es => es.StudentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}