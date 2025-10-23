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
    public class QuestionTypeConfiguration : IEntityTypeConfiguration<QuestionType>
    {
        public void Configure(EntityTypeBuilder<QuestionType> builder)
        {
            builder.ToTable("QuestionTypes");
            builder.HasKey(qt => qt.TypeId);
            builder.Property(qt => qt.Name)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(qt => qt.Description)
                .HasMaxLength(200);

            // Relationships
            builder.HasMany(qt => qt.ExamQuestions)
                .WithOne(eq => eq.QuestionType)
                .HasForeignKey(eq => eq.TypeId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
