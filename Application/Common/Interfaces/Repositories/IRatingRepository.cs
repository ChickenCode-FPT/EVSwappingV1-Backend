using Domain.Models;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Application.Common.Interfaces.Repositories;
public interface IRatingRepository
{

    public Task<Rating> AddAsync(Rating rating);
    public Task<Rating?> GetAsync(
    Expression<Func<Rating, bool>> predicate,
    Func<IQueryable<Rating>, IIncludableQueryable<Rating, object>>? include = null
    );
    public Task<IEnumerable<Rating>> GetAllAsync(
    Expression<Func<Rating, bool>>? predicate = null,
    Func<IQueryable<Rating>, IIncludableQueryable<Rating, object>>? include = null
    );
    public Task<Rating> UpdateAsync(Rating rating);

    public Task<bool> DeleteAsync(long ratingId);

}
