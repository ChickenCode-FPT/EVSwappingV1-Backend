using Application.Dtos;

namespace Application.Common.Interfaces.Services;

public interface IRatingService
{
    Task<RatingDto> CreateRating(RatingCreateRequest request);
    Task<IEnumerable<RatingDto>> GetAllRatings();
    Task<IEnumerable<RatingDto>> GetStationRatings(int stasionId);
    Task<RatingDto> UpdateRating(RatingDto ratingDto);
    Task DeleteRating(long ratingId);
}
