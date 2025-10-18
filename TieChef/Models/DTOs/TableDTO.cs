using System;
using System.Collections.Generic;
using TieChef.Models.Enums;

namespace TieChef.Models.DTOs;

public class TableDTO
{
    public int tableId { get; set; }
    public int? staffId { get; set; }
    public int? checkId { get; set; }
    public bool wasPaid { get; set; } = false;
    public List<int?> dishId { get; set; } = new List<int?>();
    public decimal? sum { get; set; }
    public DateTime? paymentDate { get; set; }
}