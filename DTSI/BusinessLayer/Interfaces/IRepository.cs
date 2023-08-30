using System.Linq.Expressions;

namespace BusinessLayer.Interfaces
{

    public interface IRepository<T> where T : class
    {

        Task<IEnumerable<T>> GetAll();
        Task<T> GetByIdAsync(Expression<Func<T, bool>> wherecondition);
        Task<IEnumerable<T>> GetByQueryAsync(Expression<Func<T, bool>> wherecondition);
        bool Add(T t);
        bool Update(T t);
        bool Delete(T t);
        bool Save();
        Task<T> GetById(int id);
        Task<T> GetByIdString(string id);
        Task<T> GetUserByIdNoTracking(Expression<Func<T, bool>> wherecondition);
    }
}
