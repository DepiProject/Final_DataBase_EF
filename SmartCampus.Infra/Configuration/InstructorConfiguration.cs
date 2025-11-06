using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;


namespace SmartCampus.Infra.Configuration
{
    public class InstructorConfiguration : IEntityTypeConfiguration<Instructor>
    {
        public void Configure(EntityTypeBuilder<Instructor> builder)
        {
            builder.ToTable("Instructors");
            builder.HasKey(i => i.InstructorId);
            builder.Property(i => i.FullName)
                .IsRequired()
                .HasMaxLength(150);

            builder.Property(i => i.ContactNumber)
                .HasMaxLength(20);

            builder.Property(i => i.CreatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(i => i.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(i => i.UserId)
                .IsUnique()
                .HasDatabaseName("IX_Instructors_UserId");

            // Relationships
            // Instructor → Grades - Restrict (preserve grades if instructor deleted)
            builder.HasMany(i => i.Grades)
                .WithOne(g => g.Instructor)
                .HasForeignKey(g => g.EnteredBy)
                .OnDelete(DeleteBehavior.Restrict);

            // Instructor → ExamSubmissions - SetNull (can remove grader reference)
            builder.HasMany(i => i.ExamSubmissions)
                .WithOne(es => es.Instructor)
                .HasForeignKey(es => es.GradedBy)
                .OnDelete(DeleteBehavior.SetNull);
        }
    }
}
