using System;
using System.Collections.Generic;
using Domain.Enums;
using Domain.Models;

namespace Tests.Mocks
{
    public static class MockDataHelper
    {
        // 🪫 1️⃣ Tạo mock Battery
        public static Battery CreateBattery(
            int id = 1,
            int modelId = 100,
            string status = BatteryStatus.Full,
            decimal? soh = 98.5m,
            int? cycles = 200)
        {
            return new Battery
            {
                BatteryId = id,
                BatteryModelId = modelId,
                SerialNumber = $"BAT-{id:D4}",
                Status = status,
                CurrentSoH = soh,
                CycleCount = cycles
            };
        }

        // ⚙️ 2️⃣ Tạo mock Station
        public static Station CreateStation(
            int id = 1,
            string name = "Test Station",
            int capacity = 10,
            string address = "123 Test St",
            decimal latitude = 10.762622m,
            decimal longitude = 106.660172m)
        {
            return new Station
            {
                StationId = id,
                Name = name,
                Capacity = capacity,
                Address = address,
                Latitude = latitude,
                Longitude = longitude,
                Phone = "0123456789",
                Status = 1,
                StationInventories = new List<StationInventory>()
            };
        }

        // 🔋 3️⃣ Tạo mock StationInventory
        public static StationInventory CreateStationInventory(
            int id = 1,
            int stationId = 1,
            int batteryId = 1,
            string status = StationInventoryStatus.Full)
        {
            return new StationInventory
            {
                StationInventoryId = id,
                StationId = stationId,
                BatteryId = batteryId,
                Status = status,
                Battery = CreateBattery(batteryId)
            };
        }

        // 🚗 4️⃣ Tạo mock Vehicle
        public static Vehicle CreateVehicle(
            int id = 1,
            string userId = "user1",
            string vin = "VIN123456789",
            string make = "VinFast",
            string model = "VF3",
            int? year = 2025)
        {
            return new Vehicle
            {
                VehicleId = id,
                UserId = userId,
                Vin = vin,
                Make = make,
                Model = model,
                Year = year,
                BatteryModelPreferenceId = 100
            };
        }

        // 📅 5️⃣ Tạo mock Reservation
        public static Reservation CreateReservation(
            int id = 1,
            string userId = "user1",
            int stationId = 1,
            int? vehicleId = 1,
            string status = ReservationStatus.Pending,
            DateTime? from = null,
            DateTime? to = null)
        {
            var start = from ?? DateTime.UtcNow.AddMinutes(10);
            var end = to ?? start.AddMinutes(30);

            return new Reservation
            {
                ReservationId = id,
                UserId = userId,
                StationId = stationId,
                VehicleId = vehicleId,
                ReservedFrom = start,
                ReservedTo = end,
                Status = status,
                ReservedBatteryModelId = 100,
                CreatedAt = DateTime.UtcNow
            };
        }

        // 🔐 6️⃣ Tạo mock ReservationAllocation
        public static ReservationAllocation CreateAllocation(
            int id = 1,
            int reservationId = 1,
            int batteryId = 1,
            string status = ReservationAllocationStatus.Active,
            DateTime? holdUntil = null)
        {
            return new ReservationAllocation
            {
                ReservationAllocationId = id,
                ReservationId = reservationId,
                BatteryId = batteryId,
                AllocatedAt = DateTime.UtcNow,
                HoldUntil = holdUntil ?? DateTime.UtcNow.AddMinutes(15),
                Status = status
            };
        }

        // 👩 7️⃣ Tạo danh sách Reservation để test trùng giờ
        public static IEnumerable<Reservation> CreateOverlappingReservations(
            string userId = "user1",
            DateTime? now = null)
        {
            var baseTime = now ?? DateTime.UtcNow;

            return new List<Reservation>
            {
                CreateReservation(
                    id: 1,
                    userId: userId,
                    from: baseTime.AddMinutes(0),
                    to: baseTime.AddMinutes(30),
                    status: ReservationStatus.Pending
                ),
                CreateReservation(
                    id: 2,
                    userId: userId,
                    from: baseTime.AddMinutes(45),
                    to: baseTime.AddMinutes(75),
                    status: ReservationStatus.Completed
                )
            };
        }

        // 🧩 8️⃣ Tạo pin hết hạn
        public static ReservationAllocation CreateExpiredAllocation(
            int id = 1,
            int batteryId = 1,
            int reservationId = 1)
        {
            return new ReservationAllocation
            {
                ReservationAllocationId = id,
                BatteryId = batteryId,
                ReservationId = reservationId,
                AllocatedAt = DateTime.UtcNow.AddHours(-2),
                HoldUntil = DateTime.UtcNow.AddHours(-1),
                Status = ReservationAllocationStatus.Active
            };
        }
    }
}
