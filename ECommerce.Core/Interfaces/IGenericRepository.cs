using ECommerce.Core.Result_Pattern;
using System.Linq.Expressions;

namespace ECommerce.Core.Interfaces
{
    public interface IGenericRepository<T> 
    {
        Task<Result<ICollection<T>>> GetAllAsync();
        Task<Result<ICollection<T>>> FindAllAsync(Expression<Func<T, bool>> expression);
        Task<Result<T>> FindAsync(Expression<Func<T,bool>> expression);

        Task<Result<T>> AddAsync(T item);
        Result<T> Update(T item);
        Result Delete(T item);
        Task<Result> ClearAsync(Expression<Func<T, bool>> expression);
        public Task<Result<T>> GetWithIncludeAsync(
            Expression<Func<T, bool>> expression,
            params Expression<Func<T, object>>[] includes);
        public Task<Result<ICollection<T>>> FindAllWithIncludeAsync(
    Expression<Func<T, bool>> expression,
    params Expression<Func<T, object>>[] includes);
    }
}
