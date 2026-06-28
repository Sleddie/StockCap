using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace StockCap.Visual.Wpf
{
    public static class MeasureAxisControlExtension
    {
        public static void UpdateElementVisibility<TElement>(
            this TElement element,
            bool show)
            where TElement : UIElement
        {
            element.Visibility = show ? Visibility.Visible : Visibility.Collapsed;
        }

        public static void UpdateLineStyle(this Line line, MeasureAxisStyle newStyle)
        {
            line.Stroke = newStyle.LineBrush;
            line.StrokeThickness = newStyle.LineThickness;
        }

        public static IEnumerable<TElement> ChangeCount<TElement>(this IEnumerable<TElement> lines, int newCount)
            where TElement : new()
        {
            if (lines.Count() > newCount)
            {
                return lines.Take(newCount);
            }

            if (lines.Count() < newCount)
            {
                List<TElement> appendix = [];

                for (int i = lines.Count(); i < newCount; i++)
                {
                    appendix.Add(new());
                }

                return lines.Union(appendix);
            }

            return lines;
        }

        public static IEnumerable<TElement> UpdateElementCollection<TElement>(
            this IEnumerable<TElement> elements,
            Panel? panel,
            int newCount)
            where TElement : UIElement, new()
        {
            int drawnCount = elements.Count();
            IEnumerable<TElement> elementsNewCount = elements.ChangeCount(newCount);

            if (panel is null)
            {
                return elementsNewCount;
            }

            if (drawnCount < newCount)
            {
                int i = 0;
                foreach (TElement element in elementsNewCount)
                {
                    if (i >= newCount)
                        break;

                    if (drawnCount < i++)
                        continue;

                    panel.Children.Add(element);
                }
            }
            else if (drawnCount > newCount)
            {
                int i = 0;
                foreach (TElement element in elements)
                {
                    if (i >= drawnCount)
                        break;

                    if (newCount < i++)
                        continue;

                    panel.Children.Remove(element);
                }
            }

            return elementsNewCount;
        }

        public static void UpdateElementVisibility<TElement>(
            this IEnumerable<TElement> elements,
            bool show,
            bool showFirst,
            bool showLast)
            where TElement : UIElement
        {
            int i = 0;
            int count = elements.Count();

            foreach (TElement element in elements)
            {
                element.UpdateElementVisibility(
                    (i == 0)
                    ? showFirst
                    : ((i == count - 1)
                        ? showLast
                        : show));
                i++;
            }
        }

        public static void UpdateLineStyle(this IEnumerable<Line> lines, MeasureAxisStyle newStyle)
        {
            foreach (var line in lines)
            {
                line.UpdateLineStyle(newStyle);
            }
        }
    }
}