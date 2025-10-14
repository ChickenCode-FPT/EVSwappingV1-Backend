using Application.Common.Exceptions;
using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Models;
using Microsoft.AspNetCore.Identity;

namespace Application.Services;

public class RatingService : IRatingService
{
    private readonly IRatingRepository _ratingRepository;
    private readonly IMapper _mapper;
    private readonly IStationRepository _stationRepository;
    private readonly UserManager<User> _userManager;

    public RatingService(
        IRatingRepository repository,
        IMapper mapper,
        IStationRepository stationRepository,
        UserManager<User> userService)
    {
        _ratingRepository = repository;
        _mapper = mapper;
        _stationRepository = stationRepository;
        _userManager = userService;
    }

    public async Task<RatingDto> CreateRating(RatingCreateRequest request)
    {
        if (!string.IsNullOrEmpty(request.UserId))
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                throw new NotFoundException($"User with id {request.UserId} not found");
            }
        }

        if (request.StationId.HasValue)
        {
            var station = await _stationRepository.GetById(request.StationId.Value);
            if (station == null)
            {
                throw new NotFoundException($"Station with id {request.StationId} not found");
            }
        }

        var rating = _mapper.Map<Rating>(request);
        rating.CreatedAt = DateTime.UtcNow;

        await _ratingRepository.AddAsync(rating);

        return _mapper.Map<RatingDto>(rating);
    }

    public async Task DeleteRating(long ratingId)
    {
        var rating = await _ratingRepository.GetAsync(r => r.RatingId == ratingId);
        if (rating == null)
        {
            throw new NotFoundException($"Rating with id {ratingId} not found");
        }
        await _ratingRepository.DeleteAsync(ratingId);
    }

    public async Task<IEnumerable<RatingDto>> GetAllRatings()
    {
        var result = await _ratingRepository.GetAllAsync();
        return _mapper.Map<IEnumerable<RatingDto>>(result);
    }

    public async Task<IEnumerable<RatingDto>> GetStationRatings(int stasionId)
    {
        var result = await _ratingRepository.GetAllAsync(r => r.StationId == stasionId);
        return _mapper.Map<IEnumerable<RatingDto>>(result);
    }

    public async Task<RatingDto> UpdateRating(RatingDto ratingDto)
    {
        var rating = await _ratingRepository.GetAsync(r => r.RatingId == ratingDto.RatingId);
        if (rating == null)
        {
            throw new NotFoundException($"Rating with id {ratingDto.RatingId} not found");
        }

        rating.Score = ratingDto.Score;
        rating.Comment = ratingDto.Comment;

        var updatedRating = await _ratingRepository.UpdateAsync(rating);
        return _mapper.Map<RatingDto>(updatedRating);
    }
}

