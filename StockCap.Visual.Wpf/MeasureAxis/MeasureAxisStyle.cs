using System.Windows.Media;

namespace StockCap.Visual.Wpf
{
    public class MeasureAxisStyle
    {
        public required double LineThickness { get; set; }
        public required Color LineColor { get; set; }
        public Brush LineBrush =>
            new SolidColorBrush(LineColor);
    }
}