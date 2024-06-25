 using Entities.Models; using Entities.Models; using CMS.API;
using Contracts.IServices;
using Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Linq.Expressions;
using Entities.Models;

namespace Repository.Repositories
{
    public abstract class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        protected CmsContext _cmsContext;
        public GenericRepository(CmsContext cmsContext)
        {
            _cmsContext = cmsContext;
        }
        //public IQueryable<T> FindAll(bool trackChanges) =>
        //!trackChanges ?
        //_cmsContext.Set<T>()
        //.AsNoTracking() :
        //_cmsContext.Set<T>();
        public IQueryable<T> FindAll(bool trackChanges, string[]? includes)
        {
            var result = trackChanges ? _cmsContext.Set<T>() : _cmsContext.Set<T>().AsNoTracking();

            if (includes != null)
            {
                foreach (var property in includes)
                {
                    result = result.Include(property);
                }
            }

            return result;
        }

        //public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges) =>
        //  !trackChanges ?
        //  _cmsContext.Set<T>()
        //  .Where(expression)
        //  .AsNoTracking() :
        //  _cmsContext.Set<T>()
        //  .Where(expression);

        public IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges, string[]? includes)
        {
            var result = !trackChanges ? _cmsContext.Set<T>().Where(expression).AsNoTracking().IgnoreAutoIncludes() : _cmsContext.Set<T>().Where(expression);

            if (includes != null)
            {
                foreach (var property in includes)
                {
                    result = result.Include(property);
                }
            }

            return result;
        }
        public int GetMaxPK(Func<T, int> columnSelector)
        {
            int maxID = 0;
            if (columnSelector == null)
            {
                throw new ArgumentNullException(nameof(columnSelector));
            }

            var data = _cmsContext.Set<T>();
            if (data.Any())
            {
                maxID = data.Max(columnSelector);
            }

            return maxID + 1;
        }

        public async virtual Task<T> GetById(int id)
        {
            return await _cmsContext.Set<T>().FindAsync(id);
        }

        public async Task Create(T entity) => await _cmsContext.Set<T>().AddAsync(entity);

        public void Update(T entity) => _cmsContext.Set<T>().Update(entity);

        public void Delete(T entity) =>  _cmsContext.Set<T>().Remove(entity);
        

        public void AddRange(IEnumerable<T> entities)
        {
            _cmsContext.Set<T>().AddRange(entities);
        }

        public void RemoveRange(IEnumerable<T> entities)
        {
            _cmsContext.Set<T>().RemoveRange(entities);
        }

        public async Task Commit()
        {
            await _cmsContext.SaveChangesAsync();
        }
    }

}
