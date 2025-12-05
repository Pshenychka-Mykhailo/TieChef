using TieChef.Data;
using TieChef.Models.Entities;

namespace TieChef.Repositories
{
    public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
    {
        public ReceiptRepository(ApplicationDbContext context) : base(context)
        {
        }
    }
}
