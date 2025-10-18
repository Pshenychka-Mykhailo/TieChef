using System;
using TieChef.Models.Enums;

namespace TieChef.Models.DTOs
{
	public class TableViewDTO
	{
		public int tableId { get; set; }
		public string? staffName { get; set; } = string.Empty;
		public bool wasPaid { get; set; }
		public int dishCount { get; set; }
		public decimal? sum { get; set; }
		public DateTime? paymentDate { get; set; }
		public e_TableViewStatus status { get; set; } = e_TableViewStatus.Available;
		public string displayText { get; set; } = string.Empty;
	}
}

