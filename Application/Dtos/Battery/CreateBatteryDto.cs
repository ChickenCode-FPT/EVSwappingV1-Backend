namespace Application.Dtos.Battery
{
    public class CreateBatteryDto
    {
        public int ModelId { get; set; }
        public string Status { get; set; }
        public int Capacity { get; set; }
    }

}
