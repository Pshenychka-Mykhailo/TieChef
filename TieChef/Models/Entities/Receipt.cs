using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TieChef.Models.Entities;

[Table("Receipts")]
public class Receipt
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ReceiptId { get; set; }

    public int TableId { get; set; }

    public int? StaffId { get; set; }

    public int? CheckId { get; set; }

    public bool WasPaid { get; set; } = false;

    [Column(TypeName = "integer[]")]
    public List<int>? DishIds { get; set; } = new List<int>();

    [Column(TypeName = "decimal(18,2)")]
    public decimal? Sum { get; set; }

    public DateTime? PaymentDate { get; set; }
}
