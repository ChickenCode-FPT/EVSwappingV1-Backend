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

        public async Task<List<SubscriptionPackageDto>> GetAll()
        {
            var packages = await _packageRepository.GetAll();
            return _mapper.Map<List<SubscriptionPackageDto>>(packages);
        }

        public async Task<SubscriptionPackageDto> Create(CreatePackageRequest request)
        {
            var entity = _mapper.Map<SubscriptionPackage>(request);

            await _packageRepository.Add(entity);

            return _mapper.Map<SubscriptionPackageDto>(entity);
        }

        public async void Update(int id, UpdatePackageRequest package)
        {
            var packages = await _packageRepository.GetById(id);
            if (packages == null)
            {
                throw new Exception("Packge not found");
            }
            packages.Price = package.Price;
            packages.Name = package.Name;
            _packageRepository.Update(packages);
        }
    }
}
