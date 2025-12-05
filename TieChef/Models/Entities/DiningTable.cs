using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TieChef.Models.Entities;

[Table("DiningTables")]
public class DiningTable
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int DiningTableId { get; set; }

    public int TableNumber { get; set; }

    public int Seats { get; set; }

    public int? X { get; set; }

    public int? Y { get; set; }

    public int? StaffId { get; set; }

    public int Width { get; set; } = 100;

    public int Height { get; set; } = 100;
}
