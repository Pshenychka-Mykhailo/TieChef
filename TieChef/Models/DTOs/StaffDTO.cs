using System;
using TieChef.Models.Enums;

namespace TieChef.Models.DTOs;

public class StaffDTO
{
    public int staffId { get; set; }
    public e_StaffType type { get; set; }
    public e_StaffRole role { get; set; }
    public string fullName { get; set; } = string.Empty;
    public int phoneNumber { get; set; }
    public string Email { get; set; } = string.Empty;
    public DateTime startWorkDate { get; set; }
    public int? scheduleId { get; set; }
    public decimal salary { get; set; }
    public string? KPI { get; set; }
}