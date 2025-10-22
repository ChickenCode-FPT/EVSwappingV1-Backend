namespace Application.Dtos
{

    public class SwapTransactionDto
    {
        public int TransactionId { get; set; }
        public int DriverId { get; set; }
        public int OldBatteryId { get; set; }
        public int NewBatteryId { get; set; }
        public DateTime SwappedAt { get; set; }
        public decimal Amount { get; set; }
    }

    public class SwapTranscationFullDto
    {
        public long SwapTransactionId { get; set; }

        public int? ReservationId { get; set; }

        public int StationId { get; set; }

        public string CustomerUserId { get; set; }

        public string CustomerName { get; set; }

        public string? StaffUserId { get; set; }
        public string? StaffName { get; set; }

        public int? OutgoingBatteryId { get; set; }

        public int? IncomingBatteryId { get; set; }

        public DateTime SwapStartedAt { get; set; }

        public DateTime? SwapFinishedAt { get; set; }

        public string SwapStatus { get; set; }

        public decimal Price { get; set; }

        public string Notes { get; set; }

        public DateTime CreatedAt { get; set; }

        public StationInSwapDto Station { get; set; }

        public ReverInSwapDto Reservation { get; set; }
    }

    //station
    public class StationInSwapDto
    {
        public int StationId { get; set; }

        public string Code { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public decimal? Latitude { get; set; }

        public decimal? Longitude { get; set; }

        public int Capacity { get; set; }

        public string Phone { get; set; }

        public byte Status { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }

    public class ReverInSwapDto
    {
        public int ReservationId { get; set; }

        public string UserId { get; set; }

        public int StationId { get; set; }

        public int? VehicleId { get; set; }

        public DateTime ReservedFrom { get; set; }

        public DateTime ReservedTo { get; set; }

        public string Status { get; set; }

        public int? ReservedBatteryModelId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}
