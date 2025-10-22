using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly ISubscriptionRepository _subscriptionRepository;
        private readonly ISubscriptionPackageRepository _packageRepository;
        private readonly IMapper _mapper;

        public SubscriptionService(
            ISubscriptionRepository subscriptionRepository,
            ISubscriptionPackageRepository packageRepository,
            IMapper mapper)
        {
            _subscriptionRepository = subscriptionRepository;
            _packageRepository = packageRepository;
            _mapper = mapper;
        }

        public async Task<RegisterSubscriptionResponse> RegisterSubscription(RegisterSubscriptionRequest request)
        {
            var package = await _packageRepository.GetById(request.PackageId);
            if (package == null)
                throw new Exception("Package not found");

            var subscription = _mapper.Map<Subscription>(request);
            subscription.RemainingSwaps = package.IncludedSwaps;

            var saved = await _subscriptionRepository.Add(subscription);

            var response = _mapper.Map<RegisterSubscriptionResponse>(saved);
            response.PackageName = package.Name;
            response.Price = package.Price;
            response.IncludedSwaps = package.IncludedSwaps;

            return response;
        }

        public async Task<List<SubscriptionPackageDto>> GetAllPackages()
        {
            var packages = await _packageRepository.GetAll();
            return _mapper.Map<List<SubscriptionPackageDto>>(packages);
        }
    }
}
