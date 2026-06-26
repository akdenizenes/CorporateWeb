using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
namespace CorporateWeb.DataAccess {
    public class Repository<T> : IRepository<T> where T : class {
        private readonly CorporateDbContext _context;
        private readonly DbSet<T> _dbSet;
        public Repository(CorporateDbContext context) {
            _context = context;
            _dbSet = _context.Set<T>();
        }
        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? filter = null) {
            IQueryable<T> query = _dbSet;
            if (filter != null) query = query.Where(filter);
            return await query.ToListAsync();
        }
        public async Task<T?> GetByIdAsync(int id) => await _dbSet.FindAsync(id);
        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);
        public void Update(T entity) => _dbSet.Update(entity);
        public void Delete(T entity) => _dbSet.Remove(entity);
    }
}