using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using jwt_identity_api.Data.Repositories.Interfaces;
using Microsoft.AspNetCore.Identity;
using jwt_identity_api.Models;
using jwt_identity_api.Helpers.Paging;

namespace jwt_identity_api.Data.Repositories
{
    class GenericTypesRepo<T> : IGenericTypesRepo<T>
        where T : class
    {
        private readonly DbSet<T> _db;
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public GenericTypesRepo(
            ApplicationDbContext context, 
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _db = context.Set<T>();
            _userManager = userManager;
        }

        public async Task<Result> DeleteObjectAsync(Guid itemId)
        {
            var entity = await _db.FindAsync(itemId);
            if(entity is null) return ResultProvider.Set(404, "Not found.", false, new()
            {
                {
                    nameof(itemId),
                    "Not found."
                }
            });

            var result = _db.Remove(entity);
            await _context.SaveChangesAsync();
            return ResultProvider.Set(200, "", true);
        } 

        public async Task<List<T>> GetObjectsAsync(Expression<Func<T, bool>>? expression = null, 
            IOrderedQueryable<T>? orderBy = null)
        {
            IQueryable<T> query = _db;

            if (expression != null) query = query.Where(expression);
            return await query.AsNoTracking().ToListAsync();
        }

        public async Task<PagedList<T>> GetObjectsAsync(Parameters parameters, Expression<Func<T, bool>>? expression = null, 
            IOrderedQueryable<T>? orderBy = null)
        {
            IQueryable<T> query = _db;
            return expression != null 
                    ? PagedList<T>
                        .ToPagedList(await query
                            .Where(expression)
                            .ToListAsync(), parameters.PageNumber, parameters.PageSize)
                    : PagedList<T>
                        .ToPagedList(await query
                            .AsNoTracking()
                            .ToListAsync(), parameters.PageNumber, parameters.PageSize);
        }

        public async Task<T?> GetObjectAsync(Expression<Func<T, bool>> expression)
        {
            IQueryable<T> query = _db;

            var result =  await query.AsNoTracking().SingleOrDefaultAsync(expression);
            return result is null ? null : result;
        }

        public async Task<Result> InsertObjectAsync(T entity, Expression<Func<T, bool>>? predicate = null)
        {
            IQueryable<T> query = _db;
            if(predicate != null)
            { 
                if(await query.AnyAsync(predicate)) return ResultProvider.Set(409, "Already in use.", false, new()
                {
                    { 
                        nameof(predicate.Name), 
                        "Already chosen." 
                    }
                });
            }

            await _db.AddAsync(entity);
            await _context.SaveChangesAsync();
            return ResultProvider.Set(200, "", true);
        }

        public async Task<Result> UpdateObjectAsync(T entity)
        {
            _db.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;

            await _context.SaveChangesAsync();
            return ResultProvider.Set(200, "", true);
        }

        public async Task<PagedList<ApplicationUser>> GetUsersAsync(Parameters parameters, Expression<Func<ApplicationUser, bool>>? expression = null, IOrderedQueryable<T>? orderBy = null) 
        {
            if(expression is null)
            {
                return PagedList<ApplicationUser>
                    .ToPagedList(await _userManager.Users
                    .ToListAsync(),
                parameters.PageNumber, parameters.PageSize);
            }
            else
            {
                return PagedList<ApplicationUser>
                    .ToPagedList(await _userManager.Users
                    .Where(expression)
                    .ToListAsync(),
                parameters.PageNumber, parameters.PageSize);
            }
        }
    }
}