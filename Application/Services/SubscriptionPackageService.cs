using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class SubscriptionPackageService : ISubscriptionPackageService
    {
        private readonly ISubscriptionPackageRepository _packageRepository;
        private readonly IMapper _mapper;

        public SubscriptionPackageService(ISubscriptionPackageRepository packageRepository, IMapper mapper)
        {
            _packageRepository = packageRepository;
            _mapper = mapper;
        }

        public async Task<List<SubscriptionPackageDto>> GetAllAsync()
        {
            var packages = await _packageRepository.GetAllAsync();
            return _mapper.Map<List<SubscriptionPackageDto>>(packages);
        }

        public async Task<SubscriptionPackageDto> CreateAsync(CreatePackageRequest request)
        {
            var entity = _mapper.Map<SubscriptionPackage>(request);

            await _packageRepository.AddAsync(entity);

            return _mapper.Map<SubscriptionPackageDto>(entity);
        }
    }
}
