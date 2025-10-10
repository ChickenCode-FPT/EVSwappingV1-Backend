namespace Domain.Models
{
    public partial class ReservationAllocation
    {
        public long ReservationAllocationId { get; set; }

        public int ReservationId { get; set; }      
        public int BatteryId { get; set; }          
        public DateTime AllocatedAt { get; set; }   
        public DateTime HoldUntil { get; set; }     
        public string Status { get; set; } = "Active";

        public virtual Reservation Reservation { get; set; }
        public virtual Battery Battery { get; set; }
    }
}
