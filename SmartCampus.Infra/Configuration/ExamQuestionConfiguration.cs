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
    public class ExamQuestionConfiguration : IEntityTypeConfiguration<ExamQuestion>
    {
        public void Configure(EntityTypeBuilder<ExamQuestion> builder)
        {
            builder.ToTable("ExamQuestions");
            builder.HasKey(q => q.QuestionId);
            builder.Property(q => q.QuestionText)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(q => q.Score)
                .IsRequired()
                .HasColumnType("decimal(5,2)");

            builder.Property(q => q.OrderNumber)
                .IsRequired();

            // Relationships
            builder.HasOne(q => q.TrueFalseQuestion)
                .WithOne(tf => tf.ExamQuestion)
                .HasForeignKey<TrueFalseQuestion>(tf => tf.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Options)
                .WithOne(o => o.ExamQuestion)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(q => q.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
