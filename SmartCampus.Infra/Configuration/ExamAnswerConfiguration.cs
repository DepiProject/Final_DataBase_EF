using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;


namespace SmartCampus.Infra.Configuration
{
    public class ExamAnswerConfiguration : IEntityTypeConfiguration<ExamAnswer>
    {
        public void Configure(EntityTypeBuilder<ExamAnswer> builder)
        {
            builder.ToTable("ExamAnswers");
            builder.HasKey(ea => ea.AnswerId);
            builder.Property(ea => ea.IsCorrect)
                .HasDefaultValue(false);

            builder.Property(ea => ea.PointsAwarded)
                .HasPrecision(5, 2);

            // Indexes
            builder.HasIndex(a => new { a.SubmissionId, a.QuestionId })
                .IsUnique()
                .HasDatabaseName("IX_ExamAnswers_Submission_Question");

            // Relationships
            builder.HasOne(ea => ea.Submission)
                .WithMany(es => es.Answers)
                .HasForeignKey(ea => ea.SubmissionId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne<MCQOption>()
                .WithMany()
                .HasForeignKey(ea => ea.SelectedOptionId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
