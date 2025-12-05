using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TieChef.Models.Entities
{
    [Table("dishes")]
    public class Dish
    {
        [Key]
        [Column("dish_id")]
        public int DishId { get; set; }

        [Required]
        [Column("name")]
        public string Name { get; set; }

        [Column("description")]
        public string? Description { get; set; }

        [Required]
        [Column("price")]
        public decimal Price { get; set; }
    }
}
