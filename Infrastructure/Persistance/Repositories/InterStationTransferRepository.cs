using Application.Common.Interfaces.Repositories;
using Application.Dtos;
using Domain.Models;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistance.Repositories
{
    public class InterStationTransferRepository : IInterStationTransferRepository
    {
        private readonly EVSwappingV2Context _context;
        public InterStationTransferRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<InterStationTransfer> CreateAsync(InterStationTransfer transfer)
        {
            var station = _context.Stations.FirstOrDefault(s => s.StationId == transfer.FromStationId);
            var battery = _context.Batteries.FirstOrDefault(b => b.BatteryId == transfer.BatteryId);
            var inventorySation = _context.StationInventories.FirstOrDefault(s => s.StationId == transfer.FromStationId && s.BatteryId == transfer.BatteryId);

            if (station == null)
                throw new ArgumentException("FromStationId không hợp lệ.");
            if (battery == null)
                throw new ArgumentException("BatteryId không hợp lệ.");
            if (inventorySation == null)
                throw new ArgumentException("Pin không có trong kho của trạm gửi.");

            if (inventorySation.Status != "Full")
            {
                throw new InvalidOperationException("Pin không sẵn sàng trong kho để xuất đi.");
            }
            inventorySation.Status = "Transfering";

            _context.InterStationTransfers.Add(transfer);
            await _context.SaveChangesAsync();
            return transfer;
        }

        public async Task<InterStationTransfer?> GetByIdAsync(long id)
        {
            return await _context.InterStationTransfers
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.Battery)
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .FirstOrDefaultAsync(t => t.TransferId == id);
        }

        public async Task<IEnumerable<InterStationTransfer>> GetByStationAsync(int stationId)
        {
            return await _context.InterStationTransfers
                .Include(t => t.Battery)
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .Where(t => t.FromStationId == stationId || t.ToStationId == stationId)
                .OrderByDescending(t => t.RequestedAt)
                .ToListAsync();
        }

        public async Task UpdateAsync(InterStationTransfer transfer)
        {
            _context.InterStationTransfers.Update(transfer);
            await _context.SaveChangesAsync();
        }

        public async Task<List<InterStationTransfer>> GetOutgoingTransfersAsync(int stationId)
        {
            return await _context.InterStationTransfers
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.Battery)
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .Where(t => t.FromStationId == stationId)
                .OrderByDescending(t => t.RequestedAt)
                .ToListAsync();
        }

        public async Task<List<InterStationTransfer>> GetIncomingTransfersAsync(int stationId)
        {
            return await _context.InterStationTransfers
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.Battery)
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .Where(t => t.ToStationId == stationId)
                .OrderByDescending(t => t.RequestedAt)
                .ToListAsync();
        }

        public async Task<IEnumerable<InterStationTransfer>> GetAllTransfersAsync()
        {
            return await _context.InterStationTransfers
                .Include(t => t.RequestedByUser)
                .Include(t => t.ApprovedByUser)
                .Include(t => t.FromStation)
                .Include(t => t.ToStation)
                .Include(t => t.Battery)
                .OrderByDescending(t => t.RequestedAt)
                .ToListAsync();
        }

        public int ParseSlotId(string input)
        {
            return int.Parse(input.Substring(1));
        }

        public async Task<List<string>> GetEmptySlotsAsync(int stationId)
        {
            var station = await _context.Stations.FirstOrDefaultAsync(s => s.StationId == stationId);

            if (station == null) throw new Exception("Station not found");

            int maxCapacity = 20;

            var occupiedSlots = await _context.StationInventories
                                              .Where(s => s.StationId == stationId)
                                              .Select(s => s.SlotNumber)
                                              .ToListAsync();
            var occupiedNumbers = new HashSet<int>();
            foreach (var slotStr in occupiedSlots)
            {
                occupiedNumbers.Add(ParseSlotId(slotStr));
            }
            List<string> emptySlots = new List<string>();

            for (int i = 1; i <= maxCapacity; i++)
            {
                if (!occupiedNumbers.Contains(i))
                {
                    emptySlots.Add("S" + i); 
                }
            }
            return emptySlots;
        }

        public async Task<InterStationTransfer?> CompletedTransferAsync(InterStationTransfer requestData, CompleteInterStationTransfer completeInterStationTransfer)
        {
            var transfer = await _context.InterStationTransfers
                .Include(t => t.Battery)
                .FirstOrDefaultAsync(t => t.TransferId == requestData.TransferId);

            var battery = await _context.Batteries
                .FirstOrDefaultAsync(b => b.BatteryId == requestData.BatteryId);
            if (transfer == null) throw new KeyNotFoundException("Không tìm thấy lệnh điều chuyển.");

            var oldInventory = await _context.StationInventories
                .FirstOrDefaultAsync(s => s.StationId == transfer.FromStationId && s.BatteryId == transfer.BatteryId);
            string targetSlot = completeInterStationTransfer.TargetSlot ?? "Unknown";

            var newInventory = new StationInventory
            {
                StationId = transfer.ToStationId, 
                BatteryId = transfer.BatteryId,
                SlotNumber = targetSlot,          
                Status = "Full",
                CheckedAt = DateTime.UtcNow
            };
            if (oldInventory != null)
            {
                _context.StationInventories.Remove(oldInventory);
            }

            await _context.StationInventories.AddAsync(newInventory);

            transfer.Status = "Completed";
            transfer.CompletedAt = DateTime.UtcNow;

            if (transfer.Battery != null)
            {
                transfer.Battery.Status = "Full"; 
            }
            await _context.SaveChangesAsync();

            return transfer;
        }

    }
}
