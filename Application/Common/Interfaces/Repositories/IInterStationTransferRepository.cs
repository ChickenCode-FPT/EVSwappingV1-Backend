using Domain.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }

}
