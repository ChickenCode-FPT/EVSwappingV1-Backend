using Application.Common.Interfaces.Repositories;
using Domain.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Infrastructure.Persistance.Repositories
{
    internal class StationStaffRepository : IStationStaffRepository
    {
        private readonly EVSwappingV2Context _context;

        public StationStaffRepository(EVSwappingV2Context context)
        {
            _context = context;
        }

        public async Task<IEnumerable<StationStaff>> GetAllAsync()
        {
            return await _context.StationStaffs
                .Include(ss => ss.User)
                .Include(ss => ss.Station)
                .ToListAsync();
        }

        public async Task<IEnumerable<StationStaff>> GetByStationIdAsync(int stationId)
        {
            return await _context.StationStaffs
                .Include(ss => ss.User)
                .Where(ss => ss.StationId == stationId && ss.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<StationStaff>> GetByUserIdAsync(string userId)
        {
            return await _context.StationStaffs
                .Include(ss => ss.Station)
                .Where(ss => ss.UserId == userId && ss.IsActive)
                .ToListAsync();
        }

        public async Task<StationStaff?> GetByIdAsync(int stationStaffId)
        {
            return await _context.StationStaffs
                .Include(ss => ss.User)
                .Include(ss => ss.Station)
                .FirstOrDefaultAsync(ss => ss.StationStaffId == stationStaffId);
        }

        public async Task AssignStaffAsync(int stationId, string userId, string role)
        {
            var record = new StationStaff
            {
                StationId = stationId,
                UserId = userId,
                Role = role,
                IsActive = true
            };

            _context.StationStaffs.Add(record);
            await _context.SaveChangesAsync();
        }

        public async Task DeactivateStaffAsync(int stationStaffId)
        {
            var staff = await _context.StationStaffs.FindAsync(stationStaffId);
            if (staff != null)
            {
                staff.IsActive = false;
                await _context.SaveChangesAsync();
            }
        }

        public async Task RemoveStaffAsync(int stationStaffId)
        {
            var staff = await _context.StationStaffs.FindAsync(stationStaffId);
            if (staff != null)
            {
                _context.StationStaffs.Remove(staff);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<IEnumerable<StationStaff>> GetByStationCodeAsync(string stationCode)
        {
            return await _context.StationStaffs
                .Include(ss => ss.User)
                .Include(ss => ss.Station)
                .Where(ss => ss.Station.Code == stationCode && ss.IsActive)
                .ToListAsync();
        }

        public async Task<IEnumerable<StationStaff>> GetByStationNameAsync(string stationName)
        {
           return await _context.StationStaffs
                .Include(ss => ss.User)
                .Include(ss => ss.Station)
                .Where(ss => ss.Station.Name == stationName && ss.IsActive)
                .ToListAsync();
        }
    }
}
