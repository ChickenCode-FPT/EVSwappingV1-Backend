namespace Domain.Enums
{
    public static class StationInventoryStatus
    {
        public const string Full = "Full";  // đủ đk sử dụng (pin còn trong slot + pin đầy + ko ai đang giữ)
        public const string Held = "Held";  // có user đang giữ pin
        public const string Empty = "Empty";    // ko có pin trong slot  
    }
}
