namespace Domain.Enums
{
    public static class BatteryStatus
    {
        public const string Full = "Full";  // đầy pin
        public const string Held = "Held";  // đang được giữ cho 1 user
        public const string InUse = "InUse";    // đang dc sử dụng
        public const string Empty = "Empty";    // hết pin
    }
}
