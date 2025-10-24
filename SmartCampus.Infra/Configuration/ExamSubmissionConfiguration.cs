using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;


namespace SmartCampus.Infra.Configuration
{
    public class ExamSubmissionConfiguration : IEntityTypeConfiguration<ExamSubmission>
    {
        public void Configure(EntityTypeBuilder<ExamSubmission> builder)
        {
            builder.ToTable("ExamSubmissions");
            builder.HasKey(es => es.SubmissionId);
            builder.Property(es => es.StartedAt)
                .IsRequired();
            builder.Property(es => es.Score)
                .HasPrecision(5, 2);

            // Indexes
            builder.HasIndex(s => new { s.ExamId, s.StudentId })
                .IsUnique()
                .HasDatabaseName("IX_ExamSubmissions_Exam_Student");

            builder.HasIndex(s => s.StudentId)
                .HasDatabaseName("IX_ExamSubmissions_StudentId");
        }
    }
}
