using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
            builder.HasMany(i => i.Courses)
                .WithOne(c => c.Instructor)
                .HasForeignKey(c => c.InstructorId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(g => g.Grades)
                .WithOne(i => i.Instructor)
                .HasForeignKey(i => i.EnteredBy)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(i => i.ExamSubmissions)
                .WithOne(es => es.Instructor)
                .HasForeignKey(es => es.GradedBy)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
