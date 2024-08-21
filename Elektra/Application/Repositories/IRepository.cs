using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OCPP.Core.Application.Repositories
{
    public interface IRepository<TEntity> where TEntity : class
    {
        Task<TEntity> GetAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true, params string[] includes);
        IQueryable<TEntity> GetAllAsync(Expression<Func<TEntity, bool>> expression, bool tracking = true, params string[] includes);
        Task<bool> IsAny(Expression<Func<TEntity, bool>> expression);

        Task InsertAsync(TEntity entity);
        Task InsertRangeAsync(List<TEntity> entities);

        Task<bool> RemoveById(Expression<Func<TEntity, bool>> expression);
        bool Remove(TEntity entity);
        void RemoveRange(List<TEntity> entities);
    }
}
