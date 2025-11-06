using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;


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
            // ExamQuestion → QuestionType - Restrict (preserve question types)
            builder.HasOne(eq => eq.QuestionType)
                .WithMany(qt => qt.ExamQuestions)
                .HasForeignKey(eq => eq.TypeId)
                .OnDelete(DeleteBehavior.Restrict);

            // ExamQuestion → TrueFalseQuestion (1-to-1) - Cascade OK
            builder.HasOne(eq => eq.TrueFalseQuestion)
                .WithOne(tf => tf.ExamQuestion)
                .HasForeignKey<TrueFalseQuestion>(tf => tf.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExamQuestion → MCQOptions - Cascade OK
            builder.HasMany(eq => eq.Options)
                .WithOne(o => o.ExamQuestion)
                .HasForeignKey(o => o.QuestionId)
                .OnDelete(DeleteBehavior.Cascade);

            // ExamQuestion → ExamAnswers - Restrict (preserve student answers)
            builder.HasMany(eq => eq.Answers)
                .WithOne(a => a.Question)
                .HasForeignKey(a => a.QuestionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
