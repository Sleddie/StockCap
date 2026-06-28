using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace StockCap.Visual.Wpf
{
    public class MeasureAxisControlCollection
    {
        public Line XAxis { get; private init; } = new();
        public Line YAxis { get; private init; } = new();

        public int XCount
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
                UpdateXCountControls();
            }
        }
        public int YCount
        {
            get;
            set
            {
                ArgumentOutOfRangeException.ThrowIfNegative(value);
                field = value;
                UpdateYCountControls();
            }
        }

        public IEnumerable<Line> XNotches { get; private set; } = [];
        public IEnumerable<Line> YNotches { get; private set; } = [];

        public IEnumerable<Line> GridVerticalLines { get; private set; } = [];
        public IEnumerable<Line> GridHorizontalLines { get; private set; } = [];

        public TextBlock ZeroNumber { get; private init; } = new();
        public TextBlock MaxValueXAxis { get; private init; } = new();
        public TextBlock MaxValueYAxis { get; private init; } = new();

        public Panel? ControlPanel { get; init; }

        public void Draw()
        {
            ControlPanel?.Children.Add(XAxis);
            ControlPanel?.Children.Add(YAxis);
            DrawUIElementEnumerable(XNotches);
            DrawUIElementEnumerable(YNotches);
            DrawUIElementEnumerable(GridVerticalLines);
            DrawUIElementEnumerable(GridHorizontalLines);
            ControlPanel?.Children.Add(ZeroNumber);
            ControlPanel?.Children.Add(MaxValueXAxis);
            ControlPanel?.Children.Add(MaxValueYAxis);
        }

        private void DrawUIElementEnumerable(IEnumerable<UIElement> uiElements)
        {
            if (ControlPanel is null)
                return;

            foreach (UIElement uiElement in uiElements)
            {
                if (ControlPanel.Children.Contains(uiElement))
                    continue;

                ControlPanel.Children.Add(uiElement);
            }
        }

        private void UpdateXCountControls()
        {
            XNotches = XNotches.UpdateElementCollection(ControlPanel, XCount);
            GridVerticalLines = GridVerticalLines.UpdateElementCollection(ControlPanel, XCount);
        }

        private void UpdateYCountControls()
        {
            YNotches = YNotches.UpdateElementCollection(ControlPanel, YCount);
            GridHorizontalLines = GridHorizontalLines.UpdateElementCollection(ControlPanel, YCount);
        }
    }
}