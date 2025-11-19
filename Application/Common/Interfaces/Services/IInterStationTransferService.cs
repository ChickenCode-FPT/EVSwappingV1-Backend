using Application.Dtos;

namespace Application.Common.Interfaces.Services
{
    public interface IInterStationTransferService
    {
        Task<InterStationTransferDto> CreateTransferAsync(CreateTransferDto dto);
        Task<bool> ApproveTransferAsync(long transferId, string approvedBy);
        Task<bool> CompleteTransferAsync(long transferId, CompleteInterStationTransfer completeInterStationTransfer);
        Task<IEnumerable<InterStationTransferDto>> GetTransfersByStationAsync(int stationId);
        Task<List<GetInterStationTransferDto>> GetOutgoingTransfersAsync(string userId);
        Task<List<GetInterStationTransferDto>> GetIncomingTransfersAsync(string userId);
        Task<List<InterStationTransferAdminDto>> GetAllTransfersAsync();
        Task<List<String>> GetAvaiableSlot(int stationId);
    }

}
