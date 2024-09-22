using AuthServer.SharedLibrary.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Core.Repositories
{
    public interface IGenericRepository<T> where T : class 
    {
        Task<T> GetByIdAsync(int id);

        //Task<IEnumerable<T>> GetAllAsync();
        Task<IQueryable<T>> GetAllAsync(); 

        IQueryable<T> Where(Expression<Func<T, bool>> predicate);
        
        Task AddAsync(T entity);    
        T Update(T entity); // _context.Entity(entity).state = EntityState.Modify // entitinin memory de durumu değişir
        void Remove(T entity);
    }
    
}
