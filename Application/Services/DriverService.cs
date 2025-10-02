using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Dtos;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly IMapper _mapper;

        public DriverService(IDriverRepository driverRepository, IMapper mapper)
        {
            _driverRepository = driverRepository;
            _mapper = mapper;
        }

        public async Task<RegisterDriverResponse> RegisterDriverAsync(RegisterDriverRequest request)
        {
            var driver = _mapper.Map<Driver>(request);

            var savedDriver = await _driverRepository.AddAsync(driver);

            return _mapper.Map<RegisterDriverResponse>(savedDriver);
        }
    }
}
