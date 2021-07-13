using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Firenet
{
    public interface IFireQuery<TEntity> where TEntity : class
    {
        TEntity[] ToArray();
        List<TEntity> ToList();
        bool Any(Expression<Func<TEntity, bool>> expression);
        TEntity Last(Expression<Func<TEntity, bool>> expression);
        TEntity LastOrDefault(Expression<Func<TEntity, bool>> expression);
        TEntity First(Expression<Func<TEntity, bool>> expression);
        TEntity FirstOrDefault(Expression<Func<TEntity, bool>> expression);

        Task<TEntity[]> ToArrayAsync();
        Task<List<TEntity>> ToListAsync();
        Task<bool> AnyAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> LastAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> LastOrDefaultAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> FirstAsync(Expression<Func<TEntity, bool>> expression);
        Task<TEntity> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> expression);

        IEnumerable<TEntity> ToEnumerable();

        IFireQuery<TEntity> OrderBy(Expression<Func<TEntity, object>> expression);
        IFireQuery<TEntity> OrderByDescending(Expression<Func<TEntity, object>> expression);
        IFireQuery<TEntity> Where(Expression<Func<TEntity, bool>> expression);
    }
}
