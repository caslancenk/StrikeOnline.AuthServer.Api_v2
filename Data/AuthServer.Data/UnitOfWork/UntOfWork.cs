using AuthServer.Core.UnitOfWork;
using AuthServer.Data.DBContext;
using Microsoft.EntityFrameworkCore;

namespace AuthServer.Data.UnitOfWork
{
    public class UntOfWork : IUnitOfWork
    {
        private readonly DbContext _context;

        public UntOfWork(AppDbContext context)
        {
            _context = context;
        }
    
        public void Commit()
        {
            _context.SaveChanges(); 
        }

        public async Task CommitAsync()
        {
            await _context.SaveChangesAsync();
        }
    }
}
