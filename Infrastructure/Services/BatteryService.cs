namespace Infrastructure.Services
{
    public class BatteryService
    {
        //private readonly EVSwappingV2Context _db;
        //public BatteryService(EVSwappingV2Context db) => _db = db;

        //public async Task<Battery?> GetById(int id, CancellationToken ct) =>
        //    await _db.Batteries.Include(b => b.BatteryModel).FirstOrDefaultAsync(b => b.BatteryId == id, ct);

        //public async Task<List<Battery>> GetByIds(IEnumerable<int> ids, CancellationToken ct) =>
        //    await _db.Batteries.Include(b => b.BatteryModel).Where(b => ids.Contains(b.BatteryId)).ToListAsync(ct);

        //public async Task<List<Battery>> GetByFilter(string? modelCode, decimal? minCapacity, decimal? maxCapacity, string? status, int? stationId, CancellationToken ct)
        //{
        //    var q = _db.Batteries.Include(b => b.BatteryModel).AsQueryable();
        //    if (!string.IsNullOrEmpty(modelCode))
        //        q = q.Where(b => b.BatteryModel != null && b.BatteryModel.ModelCode == modelCode);
        //    if (minCapacity.HasValue)
        //        q = q.Where(b => b.BatteryModel != null && b.BatteryModel.CapacityKwh >= minCapacity.Value);
        //    if (maxCapacity.HasValue)
        //        q = q.Where(b => b.BatteryModel != null && b.BatteryModel.CapacityKwh <= maxCapacity.Value);
        //    if (!string.IsNullOrEmpty(status))
        //        q = q.Where(b => b.Status == status);

        //    if (stationId.HasValue)
        //    {
        //        q = q.Where(b => _db.StationInventories.Any(si => si.BatteryId == b.BatteryId && si.StationId == stationId.Value));
        //    }

        //    return await q.ToListAsync(ct);
        //}

        //public async Task<bool> UpdateStatus(int id, string newStatus, CancellationToken ct)
        //{
        //    var b = await _db.Batteries.FindAsync(new object[] { id }, ct);
        //    if (b == null) return false;
        //    b.Status = newStatus;
        //    _db.Batteries.Update(b);
        //    await _db.SaveChangesAsync(ct);
        //    return true;
        //}

        //public async Task AddHealthLog(BatteryHealthLog log, CancellationToken ct)
        //{
        //    _db.BatteryHealthLogs.Add(log);
        //    await _db.SaveChangesAsync(ct);
        //}

        //public async Task SaveChanges(CancellationToken ct) => await _db.SaveChangesAsync(ct);
    }
}
