using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Contracts;
using Microsoft.EntityFrameworkCore;

namespace Entities
{
    /// <summary>
    /// The implemenation of the interface (IRepositoryBase)
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public abstract class RepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        protected RepositoryContext _repoCtx;
        public RepositoryBase(RepositoryContext repoCtx)
        {
            _repoCtx = repoCtx;
        }
        /// <summary>
        /// Once used in a DbSet it automatically using the dbset to find all items in a db set.
        /// </summary>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public IQueryable<T> FindAll(bool trackChanges) => !trackChanges ? _repoCtx.Set<T>().AsNoTracking() : _repoCtx.Set<T>();
        /// <summary>
        /// Finds items in a table by condition
        /// </summary>
        /// <param name="exp"></param>
        /// <param name="trackChanges"></param>
        /// <returns></returns>
        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> exp, bool trackChanges)
        {
            return !trackChanges 
                ?
                    _repoCtx.Set<T>()
                        .Where(exp)
                        .AsNoTracking()
                :
                    _repoCtx.Set<T>()
                        .Where(exp);
        }
            
        public void Create(T entity) => _repoCtx.Set<T>().Add(entity);
        public void Update(T entity) => _repoCtx.Set<T>().Update(entity);
        public void Delete(T entity) => _repoCtx.Set<T>().Remove(entity);
    }
}
