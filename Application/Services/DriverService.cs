using Application.Common.Interfaces.Repositories;
using Application.Common.Interfaces.Services;
using Application.Common.Interfaces.Services.Application.Common.Interfaces.Services;
using Application.Dtos.Driver;
using AutoMapper;
using Domain.Models;

namespace Application.Services
{
    public class DriverService : IDriverService
    {
        private readonly IDriverRepository _driverRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IMapper _mapper;

        public DriverService(
            IDriverRepository driverRepository,
            ICurrentUserService currentUser,
            IMapper mapper)
        {
            _driverRepository = driverRepository;
            _currentUser = currentUser;
            _mapper = mapper;
        }

        public async Task<RegisterDriverResponse> RegisterDriver(RegisterDriverRequest request)
        {
            var userId = _currentUser.UserId;

            if (_currentUser.UserId == null)
            {
                throw new UnauthorizedAccessException("Ko xác định dc ng dùng.");
            }

            var eDriver = await _driverRepository.GetByUserId(userId);
                                                                                    
            if (eDriver != null)
            {
                throw new InvalidOperationException("Bạn đã đk làm tài xế trc đó.");
            }

            var driver = new Driver
            {
                UserId = userId,
                PreferredPaymentMethod = request.PreferredPaymentMethod,
                TotalSwaps = 0,
                CreatedAt = DateTime.UtcNow
            };

            var savedDriver = await _driverRepository.Add(driver);

            return _mapper.Map<RegisterDriverResponse>(savedDriver);
        }
    }
}
