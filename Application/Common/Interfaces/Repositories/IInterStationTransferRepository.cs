using Application.Dtos;
using Domain.Models;

namespace Application.Common.Interfaces.Repositories
{
    public interface IInterStationTransferRepository
    {
        Task<InterStationTransfer> CreateAsync(InterStationTransfer transfer);
        Task<InterStationTransfer?> GetByIdAsync(long id);
        Task<IEnumerable<InterStationTransfer>> GetByStationAsync(int stationId);
        Task UpdateAsync(InterStationTransfer transfer);
        Task<List<InterStationTransfer>> GetOutgoingTransfersAsync(int stationId);
        Task<List<InterStationTransfer>> GetIncomingTransfersAsync(int stationId);
        Task<IEnumerable<InterStationTransfer>> GetAllTransfersAsync();
        Task<InterStationTransfer?> CompletedTransferAsync(InterStationTransfer requestData, CompleteInterStationTransfer completeInterStationTransfer);
        Task<List<string>> GetEmptySlotsAsync(int stationId);
    }

}
