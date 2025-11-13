using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace SmartCampus.App.DTOs
{
    public class MarkAttendanceDto
    {
        public int StudentId{ get; set; }
        public int CourseId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = "Present";
    }
    public class AttendanceDto
    { public int AttendanceId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = string.Empty;
        public int CourseId { get; set; }
    }
}