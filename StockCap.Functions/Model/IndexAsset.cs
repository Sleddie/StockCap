using System.Collections.ObjectModel;

namespace StockCap.Functions.Model
{
    public abstract class IndexAsset : Asset
    {
        public override AssetType Type => AssetType.Index;
        public required ReadOnlyCollection<Asset> Assets { get; init; }
    }
}