namespace StockCap.Functions.Model
{
    public abstract class StockAsset : Asset
    {
        public override AssetType Type => AssetType.Stock;
    }
}