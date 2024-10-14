using AuthServer.Core.Repositories;
using AuthServer.Data.Context;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace AuthServer.Data.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {

        private readonly DbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(AppDbContext context)
        {
            _context = context; 
            _dbSet = context.Set<T>();  
        }

        public async Task AddAsync(T entity)
        {
            await _dbSet.AddAsync(entity);
        }

        public void Remove(T entity)
        {
            _dbSet.Remove(entity); // memoryden bu entity silindi databaseye yansımadı
        }

        //public async Task<IEnumerable<T>> GetAllAsync()
        //{
        //   return await _dbSet.ToListAsync();
        //}

        //public IQueryable<T> GetAllAsync() 
        //{
        //    return _dbSet.AsQueryable();    
        //} 

        public async Task<T> GetByIdAsync(int id)
        {
            var entity = await _dbSet.FindAsync(id);

            if (entity != null)
            {
                //_dbSet.Entry(entity).State = EntityState.Detached;
                _context.Entry(entity).State = EntityState.Detached;
            }

            return entity;
        }

        public T Update(T entity)
        {
            //asenkron bir işlem değil var olan entitylerin stateleri değiştirilyor
            _context.Entry(entity).State = EntityState.Modified;

            return entity;  
        }

        public IQueryable<T> Where(Expression<Func<T, bool>> predicate)
        {
            return _dbSet.Where(predicate);
        }

        public async Task<IQueryable<T>> GetAllAsync()
        {
            var result = await _dbSet.ToListAsync();
            return result.AsQueryable();
        }
    }
}
