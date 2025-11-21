namespace Application.Dtos.Battery
{
    public class BatteriesDto
    {
        public int BatteryId { get; set; }
        public string SerialNumber { get; set; }
        public int BatteryModelId { get; set; }
        public decimal? CurrentSoH { get; set; }
        public int? CycleCount { get; set; }
        public string Status { get; set; }
        public DateTime? LastMaintenance { get; set; }
        public DateTime CreatedAt { get; set; }

        public BatteryModelDto BatteryModel { get; set; }
    }

    public class BatteryModelDto
    {
        public int BatteryModelId { get; set; }
        public string ModelCode { get; set; } = string.Empty;
        public string Manufacturer { get; set; } = string.Empty;
        public decimal CapacityKwh { get; set; }
        public string Chemistry { get; set; } = string.Empty;
        public string CompatibleVehicleTypes { get; set; } = string.Empty;
        public decimal ReservationDepositFee { get; set; }
        public DateTime CreatedAt { get; set; }
        public string DisplayName { get; set; } = string.Empty;
        public int CapacityWh { get; set; }
    }
}
