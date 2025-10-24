using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;


namespace SmartCampus.Infra.Configuration
{
    public class TrueFalseQuestionConfiguration : IEntityTypeConfiguration<TrueFalseQuestion>
    {
        public void Configure(EntityTypeBuilder<TrueFalseQuestion> builder)
        {
            builder.ToTable("TrueFalseQuestions");
            builder.HasKey(tf => tf.QuestionId);
            builder.Property(tf => tf.IsCorrect)
                .IsRequired();
        }
    }
}
