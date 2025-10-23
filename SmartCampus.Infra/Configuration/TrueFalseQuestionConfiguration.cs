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
