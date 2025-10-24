using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using SmartCampus.Core.Entities;


namespace SmartCampus.Infra.Configuration
{
    public class MCQOptionConfiguration : IEntityTypeConfiguration<MCQOption>
    {
        public void Configure(EntityTypeBuilder<MCQOption> builder)
        {
            builder.ToTable("MCQOptions");
            builder.HasKey(o => o.OptionId);
            builder.Property(o => o.OptionText)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(o => o.IsCorrect)
                .IsRequired();

            builder.Property(o => o.OrderNumber)
                .IsRequired();
        }
    }
}
