using System.Linq.Expressions;

namespace CollegeAppWebAPI.Models.Data.Repository
{
    public interface ICollegeRepository<T> 
    {
        Task<IEnumerable<T>> GetAllAsync();
        Task<T> GetAsync(Expression<Func<T, bool>> filter, bool trackEntity = false);        
        Task<T> AddAsync(T dbRecord);
        Task UpdateAsync(T dbRecord);
        Task DeleteAsync(T dbRecord);
    }

}
