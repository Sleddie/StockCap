namespace StockCap.Functions.Model
{
    public abstract class Asset
    {
        public abstract AssetType Type { get; }
        public required string Name { get; init; }
        public string? Description { get; init; }
        public required float Value { get; set; }

        public abstract float GetValue(DateTime timeStamp);

        public float UpdateValue() =>
            Value = GetValue(DateTime.UtcNow);
    }
}