using Entities.Models; using CMS.API;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Contracts.IServices
{
    public interface IGenericRepository<T> where T : class
    {
        IQueryable<T> FindAll(bool trackChanges, string[]? includes);
        IQueryable<T> FindByCondition(Expression<Func<T, bool>> expression, bool trackChanges, string[]? includes);
        Task<T> GetById(int id);
        int GetMaxPK(Func<T, int> columnSelector);
        Task Create(T entity);
        void Update(T entity);
        void Delete(T entity);
        void AddRange(IEnumerable<T> entities);
        void RemoveRange(IEnumerable<T> entities);
        Task Commit();
    }
}
