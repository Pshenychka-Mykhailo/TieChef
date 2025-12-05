using TieChef.Data;
using TieChef.Models.Entities;

namespace TieChef.Repositories
{
    public class StaffRepository : Repository<Staff>, IStaffRepository
    {
        public StaffRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
