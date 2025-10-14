using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;
using System.Linq.Expressions;

namespace Infrastructure.Persistance.Repositories;
public class RatingRepository : IRatingRepository
{
    private readonly EVSwappingV2Context _context;
    private readonly DbSet<Rating> _ratingSet;

    public RatingRepository(EVSwappingV2Context context)
    {
        _context = context;
        _ratingSet = _context.Ratings;
    }

    public async Task<Rating> AddAsync(Rating rating)
    {
        await _ratingSet.AddAsync(rating);
        await _context.SaveChangesAsync();

        return rating;
    }

    public async Task<Rating?> GetAsync(
        Expression<Func<Rating, bool>> predicate,
        Func<IQueryable<Rating>, IIncludableQueryable<Rating, object>>? include = null
        )
    {
        IQueryable<Rating> query = _ratingSet;

        if (include != null)
        {
            query = include(query);
        }

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task<IEnumerable<Rating>> GetAllAsync(
    Expression<Func<Rating, bool>>? predicate = null,
    Func<IQueryable<Rating>, IIncludableQueryable<Rating, object>>? include = null
    )
    {
        IQueryable<Rating> query = _ratingSet;

        if (include != null)
        {
            query = include(query);
        }

        if (predicate != null)
        {
            query = query.Where(predicate);
        }

        return await query.ToListAsync();
    }


    public async Task<Rating> UpdateAsync(Rating rating)
    {
        _ratingSet.Update(rating);
        await _context.SaveChangesAsync();
        return rating;
    }

    public async Task<bool> DeleteAsync(long ratingId)
    {
        var rating = await _ratingSet.FirstOrDefaultAsync(r => r.RatingId == ratingId);
        if (rating is null)
        {
            return false;
        }

        _ratingSet.Remove(rating);
        await _context.SaveChangesAsync();

        return true;
    }
}
