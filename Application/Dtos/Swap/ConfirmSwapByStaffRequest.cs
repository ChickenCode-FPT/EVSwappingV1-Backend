namespace Application.Dtos.Swap
{
    public class ConfirmSwapByStaffRequest
    {
        public long SwapTransactionId { get; set; }
        public string StaffUserId { get; set; } = string.Empty;

        public int OutgoingBatteryId { get; set; }

        public string? Notes { get; set; }
    }
}
