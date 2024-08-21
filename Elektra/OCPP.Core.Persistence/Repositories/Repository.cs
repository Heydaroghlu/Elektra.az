using Microsoft.EntityFrameworkCore;
using OCPP.Core.Application.Repositories;
using OCPP.Core.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Persistence.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        readonly DbSet<TEntity> Table;
        public Repository(OCPPCoreContext mainContext)
        {
            Table = mainContext.Set<TEntity>();
        }

        public IQueryable<TEntity> GetAllAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true, params string[] includes)
        {
            var query = AsTracking(Table.Where(expression), tracking);
            query = AsTracking(query, tracking);
            query = Include(query, includes);
            return query;
        }

        public async Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true, params string[] includes)
        {
            var query = AsTracking(Table.AsQueryable(), tracking);
            query = Include(query, includes);
            return await query.FirstOrDefaultAsync(expression);
        }

        public async Task InsertAsync(TEntity entity)
        {
            await Table.AddAsync(entity);
        }

        public async Task InsertRangeAsync(List<TEntity> entities)
        {
            await Table.AddRangeAsync(entities);
        }

        public async Task<bool> IsAny(Expression<Func<TEntity, bool>> expression) => await Table.AnyAsync(expression);

        public bool Remove(TEntity entity)
        {
            var entry = Table.Remove(entity);
            if (entry.State == EntityState.Deleted) return true;
            else return false;
        }

        public async Task<bool> RemoveById(Expression<Func<TEntity, bool>> expression)
        {
            var entity = await GetAsync(expression);
            var check = Remove(entity);
            if (check == true) return true;
            else return false;
        }

        public void RemoveRange(List<TEntity> entities)
        {
            Table.RemoveRange(entities);
        }

        private IQueryable<TEntity> AsTracking(IQueryable<TEntity> query, bool tracking) => tracking ? query : query.AsNoTracking();
        private IQueryable<TEntity> Include(IQueryable<TEntity> query, params string[] includes)
        {
            if (includes != null)
                foreach (var include in includes) query = query.Include(include);
            return query;
        }

    }
}
