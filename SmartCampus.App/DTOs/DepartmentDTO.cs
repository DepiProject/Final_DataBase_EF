using System.ComponentModel.DataAnnotations;

namespace SmartCampus.App.DTOs
{
    public class DepartmentDTO
    {
        [MaxLength(100)]
        public string Name { get; set; } = string.Empty;
        [MaxLength(50)]
        public string Building { get; set; } = string.Empty;
        [Required]
        public int? HeadId { get; set; }
    }


}
