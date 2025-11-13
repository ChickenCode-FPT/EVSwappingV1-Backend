using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using Application.Dtos.Subscription;
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

        public async Task Update(int id, UpdatePackageRequest package)
        {
            var packages = await _packageRepository.GetById(id);
            if (packages == null)
            {
                throw new Exception("Packge not found");
            }
            packages.Price = package.Price;
            packages.Name = package.Name;
           await _packageRepository.Update(packages);
        }

        public async Task<List<SubscriptionPackageDto>> GetActivePackages()
        {
            var activePackages = await _packageRepository.GetActivePackages();
            return _mapper.Map<List<SubscriptionPackageDto>>(activePackages);
        }

        public async Task InactivePackage(int id)
        {
            var pkg = await _packageRepository.GetById(id);
            if (pkg == null)
                throw new Exception("Package not found");

            if (pkg.Status != "Active")
                throw new Exception("Only active packages can be inactivated");

            pkg.Status = "Inactive";
            await _packageRepository.Update(pkg);
        }
        public async Task ReactivatePackage(int id)
        {
            var pkg = await _packageRepository.GetById(id);
            if (pkg == null)
                throw new Exception("Package not found");

            if (pkg.Status != "Inactive")
                throw new Exception("Only inactive packages can be reactivated");

            pkg.Status = "Active";
            await _packageRepository.Update(pkg);
        }

        public async Task PublishPackage(int id)
        {
            var pkg = await _packageRepository.GetById(id);
            if (pkg == null)
                throw new Exception("Package not found");

            if (pkg.Status != "Draft")
                throw new Exception("Only draft packages can be published");

            pkg.Status = "Active";
            await _packageRepository.Update(pkg);
        }


    }
}
