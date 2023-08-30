using BusinessLayer.Interfaces;
using DataAccessLayer.Database;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace BusinessLayer.Implementations
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly DatabaseEntity context;
        private readonly DbSet<T> table = null;

        public Repository(DatabaseEntity _context)
        {
            context = _context;
            table = context.Set<T>();
        }
        public bool Add(T t)
        {
            table.Add(t);
            return Save();
        }

        public bool Delete(T t)
        {
            table.Remove(t);
            return Save();
        }


        public async Task<IEnumerable<T>> GetAll()
        {
            return await table.ToListAsync();
        }


        public  bool Save()
        {
            var isSaved = context.SaveChanges();
            return isSaved > 0;
        }


        public bool Update(T t)
        {
            table.Attach(t);
            context.Entry(t).State = EntityState.Modified;
            return Save();
        }

        public async Task<T> GetByIdAsync(Expression<Func<T, bool>> wherecondition)
        {
            T? t = await table.Where(wherecondition).FirstOrDefaultAsync();
            return t;
        }

        public async Task<IEnumerable<T>> GetByQueryAsync(Expression<Func<T, bool>> wherecondition)
        {
           IEnumerable<T?> t = await table.Where(wherecondition).ToListAsync();
            return t;
        }

        public async Task<T> GetById(int id)
        {
            T? t = await table.FindAsync(id);
            return t;
        }

        public async Task<T> GetByIdString(string id)
        {
            T? t = await table.FindAsync(id);
            return t;
        }

        public async Task<T> GetUserByIdNoTracking(Expression<Func<T, bool>> wherecondition)
        {
            T? t = await table.Where(wherecondition).AsNoTracking().FirstOrDefaultAsync();
            return t;
        }

       
    }
}
