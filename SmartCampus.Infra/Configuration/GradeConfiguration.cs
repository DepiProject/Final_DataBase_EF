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
    public class GradeConfiguration : IEntityTypeConfiguration<Grade>
    {
        public void Configure(EntityTypeBuilder<Grade> builder)
        {
            builder.ToTable("Grades");
            builder.HasKey(g => g.GradeId);
            builder.Property(g => g.Score)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(g => g.GradeLetter)
                .IsRequired()
                .HasMaxLength(5);

            builder.Property(g => g.EnteredAt)
                .HasDefaultValueSql("GETUTCDATE()");

            builder.Property(g => g.UpdatedAt)
                .HasDefaultValueSql("GETUTCDATE()");

            // Indexes
            builder.HasIndex(g => g.EnrollmentId)
                .IsUnique()
                .HasDatabaseName("IX_Grades_EnrollmentId");
        }
    }
}
