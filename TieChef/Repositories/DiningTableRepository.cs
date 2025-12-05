using TieChef.Data;
using TieChef.Models.Entities;

namespace TieChef.Repositories;

public class DiningTableRepository : Repository<DiningTable>, IDiningTableRepository
{
    public DiningTableRepository(ApplicationDbContext context) : base(context)
    {
    }
}
