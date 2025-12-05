using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using TieChef.Models.Enums;

namespace TieChef.Models.Entities;

[Table("Staffs")]
public class Staff
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int StaffId { get; set; }

    public e_StaffType Type { get; set; }

    public e_StaffRole Role { get; set; }

    public string FullName { get; set; } = string.Empty;

    public int PhoneNumber { get; set; }

    public string Email { get; set; } = string.Empty;

    public DateTime StartWorkDate { get; set; }

    public int? ScheduleId { get; set; }

    [Column(TypeName = "decimal(18,2)")]
    public decimal Salary { get; set; }

    public string? KPI { get; set; }
}
