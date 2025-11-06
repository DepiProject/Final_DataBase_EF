using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;


namespace SmartCampus.Infra.Configuration
{
    public class EnrollmentConfiguration : IEntityTypeConfiguration<Enrollment>
    {
        public void Configure(EntityTypeBuilder<Enrollment> builder)
        {
            builder.ToTable("Enrollments");
            builder.HasKey(e => e.EnrollmentId);
            builder.Property(e => e.Status)
                .IsRequired()
                .HasMaxLength(20)
                .HasDefaultValue("Enrolled");

            builder.Property(e => e.EnrollmentDate)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(e => new { e.StudentId, e.CourseId })
               .IsUnique()
               .HasDatabaseName("IX_Enrollments_Student_Course");

            // Relationships
            // Enrollment → Grade (1-to-1) - Cascade OK (only path to Grade)
            builder.HasOne(e => e.Grade)
                .WithOne(g => g.Enrollment)
                .HasForeignKey<Grade>(g => g.EnrollmentId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
