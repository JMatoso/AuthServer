using System.Linq.Expressions;
using jwt_identity_api.Helpers.Paging;
using jwt_identity_api.Models;

namespace jwt_identity_api.Data.Repositories.Interfaces
{
    public interface IGenericTypesRepo <T> where T : class
    {
        Task<Result> DeleteObjectAsync(Guid itemId); 
        Task<PagedList<T>> GetObjectsAsync(Parameters parameters, Expression<Func<T, bool>>? expression = null, IOrderedQueryable<T>? orderBy = null);
        Task<List<T>> GetObjectsAsync(Expression<Func<T, bool>>? expression = null, IOrderedQueryable<T>? orderBy = null);
        Task<PagedList<ApplicationUser>> GetUsersAsync(Parameters parameters, Expression<Func<ApplicationUser, bool>>? expression = null, IOrderedQueryable<T>? orderBy = null);
        Task<T?> GetObjectAsync(Expression<Func<T, bool>> expression);
        Task<Result> InsertObjectAsync(T entity, Expression<Func<T, bool>>? predicate = null);
        Task<Result> UpdateObjectAsync(T entity);
    }
}