using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using SmartCampus.Core.Entities;


namespace SmartCampus.Infra.Configuration
{
    public class DepartmentConfiguration : IEntityTypeConfiguration<Department>
    {
        public void Configure(EntityTypeBuilder<Department> builder)
        {
            builder.ToTable("Departments");
            builder.HasKey(d => d.DepartmentId);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Building)
                .IsRequired()
                .HasMaxLength(50);

            // Indexes
            builder.HasIndex(d => d.Name)
               .IsUnique()
               .HasDatabaseName("IX_Departments_Name");

            // Relationships
            builder.HasOne(d => d.Instructor)
                .WithOne(i => i.HeadOfDepartment)
                .HasForeignKey<Department>(d => d.HeadId)
                .IsRequired(false) 
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Students)
                .WithOne(s => s.Department)
                .HasForeignKey(s => s.DepartmentId);

            builder.HasMany(d => d.Instructors)
                .WithOne(i => i.Department!) 
                .HasForeignKey(i => i.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);

            builder.HasMany(d => d.Courses)
                .WithOne(c => c.Department!) 
                .HasForeignKey(c => c.DepartmentId)
                .OnDelete(DeleteBehavior.Restrict);
        }
    }
}
