using TieChef.Data;
using TieChef.Models.Entities;

namespace TieChef.Repositories
{
    public class DishRepository : Repository<Dish>, IDishRepository
    {
        public DishRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
