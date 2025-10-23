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
    public class AttendanceConfiguration : IEntityTypeConfiguration<Attendance>
    {
        public void Configure(EntityTypeBuilder<Attendance> builder)
        {
            builder.ToTable("Attendances");
            builder.HasKey(a => a.AttendanceId);
            builder.Property(a => a.Date)
                .IsRequired();

            builder.Property(a => a.Status)
                .IsRequired()
                .HasMaxLength(20);

            //Prevents duplicate attendance
            builder.HasIndex(a => new { a.StudentId, a.CourseId, a.Date })
                 .IsUnique()
                 .HasDatabaseName("IX_Attendances_UniqueRecord");
        }
    }
}
