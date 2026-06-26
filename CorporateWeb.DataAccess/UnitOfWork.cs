using System.Threading.Tasks;
using CorporateWeb.Entities; // Bu satır eksikti

namespace CorporateWeb.DataAccess {
    public class UnitOfWork : IUnitOfWork {
        private readonly CorporateDbContext _context;
        public IRepository<Service> Services { get; private set; }
        
        public UnitOfWork(CorporateDbContext context) {
            _context = context;
            Services = new Repository<Service>(_context);
        }
        
        public async Task<int> SaveAsync() => await _context.SaveChangesAsync();
        public void Dispose() => _context.Dispose();
    }
}