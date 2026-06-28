using System.Drawing.Printing;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace StockCap.Visual.Wpf
{
    public partial class MeasureAxis : UserControl
    {
        public MeasureAxis()
        {
            IsHandlersEnabled = false;
            InitializeComponent();
            IsHandlersEnabled = true;
            CartesianCoordinates.DefaultPlaneControl = _measureAxisGrid;
            Controls = new()
            {
                ControlPanel = _measureAxisGrid
            };
            XAxisMaxCount = 20;
            YAxisMaxCount = 12;
        }

        private bool IsHandlersEnabled { get; set; }

        public double ViewPadding
        {
            get;
            set
            {
                field = value > 0 ? value : 0;
                Update();
            }
        } = 32;

        public double ViewHeight =>
            _measureAxisGrid.ActualHeight - 2 * ViewPadding;

        public double ViewWidth =>
            _measureAxisGrid.ActualWidth - 2 * ViewPadding;

        public int XAxisMaxCount
        {
            get => Controls.XCount - 1;
            set
            {
                Controls.XCount = value > 0 ? value + 1 : 0;

                if (MaxXValue == 0)
                    MaxXValue = XAxisMaxCount;

                Update();
            }
        }

        public int YAxisMaxCount
        {
            get => Controls.YCount - 1;
            set
            {
                Controls.YCount = value > 0 ? value + 1 : 0;

                if (MaxYValue == 0)
                    MaxYValue = YAxisMaxCount;

                Update();
            }
        }

        public double MaxXValue { get; set; }

        public double MaxYValue { get; set; }

        public byte Precision { get; set; }

        public MeasureAxisControlCollection Controls { get; init; }

        public MeasureAxisStyle StrongStyle { get; init; } =
            new()
            {
                LineThickness = 2,
                LineColor = Colors.Black
            };

        public MeasureAxisStyle WeakStyle { get; init; } =
            new()
            {
                LineThickness = 1,
                LineColor = Color.FromArgb(80, 0, 100, 200)
            };

        public void Draw()
        {
            Controls.Draw();
            DrawAxes();
            DrawNotches();
            DrawGrid();
            DrawZeroNumber();
            DrawAxesMaxValues();
        }

        public void Update()
        {
            UpdateAxes();
            UpdateNotches();
            UpdateGrid();
        }

        public void Clear() =>
            _measureAxisGrid.Children.Clear();

        #region Initial lines drawing

        private void DrawAxes()
        {
            UpdateAxes(full: true);
            Controls.XAxis.UpdateLineStyle(StrongStyle);
            Controls.YAxis.UpdateLineStyle(StrongStyle);
        }

        private void DrawNotches()
        {
            UpdateNotches(full: true);
            Controls.XNotches.UpdateElementVisibility(true, false, false);
            Controls.YNotches.UpdateElementVisibility(true, false, false);
            Controls.XNotches.UpdateLineStyle(StrongStyle);
            Controls.YNotches.UpdateLineStyle(StrongStyle);
        }

        private void DrawGrid()
        {
            UpdateGrid(full: true);
            Controls.GridVerticalLines.UpdateElementVisibility(true, false, true);
            Controls.GridHorizontalLines.UpdateElementVisibility(true, false, true);
            Controls.GridVerticalLines.UpdateLineStyle(WeakStyle);
            Controls.GridHorizontalLines.UpdateLineStyle(WeakStyle);
        }

        private Line GetAxisLine(bool verticalAxis) =>
            GetStraightLine(
                verticalAxis,
                (verticalAxis ? ViewHeight : ViewWidth),
                ViewPadding,
                ViewPadding);

        private void DrawAxisNotches(bool verticalAxis, bool displayNumbers = true)
        {
            int notchesCount = (verticalAxis
                ? YAxisMaxCount
                : XAxisMaxCount) + 1;

            DrawCycledLines(
                !verticalAxis,
                notchesCount,
                verticalAxis ? ViewHeight : ViewPadding / 2,
                verticalAxis ? ViewPadding / 2 : ViewWidth,
                verticalAxis ? ViewPadding / 2 : ViewPadding,
                verticalAxis ? ViewPadding : ViewPadding / 2,
                false,
                false,
                StrongStyle.LineBrush,
                StrongStyle.LineThickness);
        }

        private void DrawGridLines(bool vertical)
        {
            int notchesCount = (vertical
                ? XAxisMaxCount
                : YAxisMaxCount) + 1;

            DrawCycledLines(
                vertical,
                notchesCount,
                ViewHeight,
                ViewWidth,
                vertical ? ViewPadding : ViewPadding * 1.5,
                vertical ? ViewPadding * 1.5 : ViewPadding,
                false,
                true,
                WeakStyle.LineBrush,
                WeakStyle.LineThickness);
        }

        private void DrawCycledLines(
            bool vertical,
            int count,
            double height,
            double width,
            double offsetLeft,
            double offsetBottom,
            bool showFirst,
            bool showLast) =>
            DrawCycledLines(
                vertical,
                count,
                height,
                width,
                offsetLeft,
                offsetBottom,
                showFirst,
                showLast,
                StrongStyle.LineBrush,
                StrongStyle.LineThickness);

        private void DrawCycledLines(
            bool vertical,
            int count,
            double height,
            double width,
            double offsetLeft,
            double offsetBottom,
            bool showFirst,
            bool showLast,
            Brush brush,
            double thickness)
        {
            int devider = count - 1;

            for (int i = 0; i < count; i++)
            {
                Line cycledLine = GetStraightLine(
                    vertical,
                    vertical
                        ? height
                        : width,
                    vertical
                        ? offsetLeft + i * (width / devider)
                        : offsetLeft,
                    vertical
                        ? offsetBottom
                        : offsetBottom + i * (height / devider),
                    brush,
                    thickness);

                if (!showFirst && i == 0
                    || !showLast && i == count - 1)
                {
                    cycledLine.Visibility = Visibility.Collapsed;
                }

                _measureAxisGrid.Children.Add(cycledLine);
            }
        }

        private Line GetStraightLine(
            bool vertical,
            double length,
            double offsetLeft,
            double offsetBottom) =>
            GetStraightLine(
                vertical,
                length,
                offsetLeft,
                offsetBottom,
                StrongStyle.LineBrush,
                StrongStyle.LineThickness);

        private Line GetStraightLine(
            bool vertical,
            double length,
            double offsetLeft,
            double offsetBottom,
            Brush brush,
            double thickness)
        {
            double x1 = offsetLeft;
            double y1 = (ViewHeight + 2 * ViewPadding) - offsetBottom;
            return new()
            {
                X1 = x1,
                Y1 = y1,
                X2 = vertical ? x1 : x1 + length,
                Y2 = vertical ? y1 - length : y1,
                Stroke = brush,
                StrokeThickness = thickness
            };
        }

        #endregion

        #region Lines updating

        private void UpdateAxes(bool full = false)
        {
            if (full)
            {
                MoveLinePoint(
                    Controls.YAxis,
                    (ViewPadding, ViewPadding + ViewHeight),
                    false);
            }

            MoveLinePoint(
                Controls.XAxis,
                (ViewPadding, ViewPadding),
                true);
            UpdateStraightLineLength(Controls.XAxis, ViewWidth, false);
            UpdateStraightLineLength(Controls.YAxis, ViewHeight, true);
        }

        private void UpdateNotches(bool full = false)
        {
            UpdateAxisNotches(Controls.XNotches, full, false);
            UpdateAxisNotches(Controls.YNotches, full, true);
        }

        private void UpdateGrid(bool full = false)
        {
            UpdateGridLines(Controls.GridVerticalLines, full, true);
            UpdateGridLines(Controls.GridHorizontalLines, full, false);
        }

        private void UpdateAxisNotches(
            IEnumerable<Line> lines,
            bool full,
            bool verticalAxis)
        {
            UpdateCycledLines(
                lines,
                full,
                !verticalAxis,
                verticalAxis ? ViewHeight : ViewPadding / 2,
                verticalAxis ? ViewPadding / 2 : ViewWidth,
                verticalAxis ? ViewPadding / 2 : ViewPadding,
                verticalAxis ? ViewPadding : ViewPadding / 2);
        }

        private void UpdateGridLines(
            IEnumerable<Line> lines,
            bool full,
            bool vertical)
        {
            UpdateCycledLines(
                lines,
                full,
                vertical,
                ViewHeight,
                ViewWidth,
                vertical ? ViewPadding : ViewPadding * 1.5,
                vertical ? ViewPadding * 1.5 : ViewPadding);
        }

        private void UpdateCycledLines(
            IEnumerable<Line> lines,
            bool full,
            bool vertical,
            double height,
            double width,
            double offsetLeft,
            double offsetBottom)
        {
            int i = 0;
            int lineCount = lines.Count();
            int devider = lineCount - 1;

            foreach (Line line in lines)
            {
                /// if vertical =>
                ///     1. move end point for each line except the FIRST
                ///     2. update length for each line
                /// if horizontal =>
                ///     1. move start point for each line except the LAST
                ///     2. update length for each line

                (double, double) offsetLeftBottom =
                    (vertical
                        ? offsetLeft + i * (width / devider)
                        : offsetLeft,
                    vertical
                        ? offsetBottom + height
                        : offsetBottom + i * (height / devider));

                if (full
                    || (!vertical || i != 0)
                    && (vertical || i != lineCount - 1))
                {
                    MoveLinePoint(line, offsetLeftBottom, !vertical);
                }

                UpdateStraightLineLength(
                    line,
                    vertical ? height : width,
                    vertical);
                i++;
            }
        }

        private void ShiftLine(
            Line line,
            double horizontalShift,
            double verticalShift)
        {
            MoveLinePoint(
                line,
                (line.X1 + horizontalShift, line.Y1 - verticalShift),
                true);
            MoveLinePoint(
                line,
                (line.X2 + horizontalShift, line.Y2 - verticalShift),
                false);
        }

        private void MoveLinePoint(
            Line line,
            CartesianCoordinates newPosition,
            bool startPoint)
        {
            if (startPoint)
            {
                line.X1 = newPosition.RealX;
                line.Y1 = newPosition.RealY;
            }
            else
            {
                line.X2 = newPosition.RealX;
                line.Y2 = newPosition.RealY;
            }
        }

        private void UpdateStraightLineLength(
            Line line,
            double newLength,
            bool verticalLine)
        {
            if (verticalLine)
            {
                line.X1 = line.X2;
                line.Y1 = line.Y2 + newLength;
            }
            else
            {
                line.X2 = line.X1 + newLength;
                line.Y2 = line.Y1;
            }
        }

        #endregion

        #region Initial numbers drawing

        private void DrawZeroNumber() =>
            DrawAxisNumber(
                Controls.ZeroNumber,
                (HorizontalAlignment.Left, VerticalAlignment.Bottom),
                (ViewPadding, ViewPadding),
                0);

        private void DrawAxesMaxValues()
        {
            DrawAxisNumber(
                Controls.MaxValueXAxis,
                (HorizontalAlignment.Right, VerticalAlignment.Bottom),
                (ViewPadding, ViewPadding),
                MaxXValue,
                precision: Precision);
            DrawAxisNumber(
                Controls.MaxValueYAxis,
                (HorizontalAlignment.Left, VerticalAlignment.Top),
                (ViewPadding, ViewPadding),
                MaxYValue,
                precision: Precision);
        }

        private void DrawAxisNumber(
            TextBlock numberBlock,
            (HorizontalAlignment, VerticalAlignment) alignment,
            CartesianCoordinates position,
            double value,
            int precision = 0)
        {
            SetNumberBlockParams(numberBlock);
            UpdateNumberValue(numberBlock, value, precision: precision);
            UpdateNumberPosition(
                numberBlock,
                alignment,
                position,
                (HorizontalAlignment.Right, VerticalAlignment.Top));
        }

        private TextBlock GetNumber(
            CartesianCoordinates position,
            (HorizontalAlignment, VerticalAlignment) positionRelativeAlignment,
            double value,
            int precision = 0) =>
            GetNumber(
                (HorizontalAlignment.Left, VerticalAlignment.Bottom),
                position,
                positionRelativeAlignment,
                value,
                precision);

        private TextBlock GetNumber(
            (HorizontalAlignment, VerticalAlignment) alignment,
            CartesianCoordinates position,
            (HorizontalAlignment, VerticalAlignment) positionRelativeAlignment,
            double value,
            int precision = 0)
        {
            string stringValue = value.ToString($"N{Math.Abs(precision)}");
            CartesianCoordinates offsetLeftBottom = GetTextOffset(
                alignment,
                position,
                positionRelativeAlignment,
                stringValue,
                minWidth: ViewPadding);
            Thickness padding = new(FontSize / 4);
            Thickness margin = new(
                offsetLeftBottom.X - padding.Left,
                offsetLeftBottom.Y - padding.Top,
                offsetLeftBottom.X - padding.Right,
                offsetLeftBottom.Y - padding.Bottom);

            TextBlock numberBlock = new()
            {
                Text = stringValue,
                MinWidth = ViewPadding,
                Margin = margin,
                Padding = padding,
                HorizontalAlignment = alignment.Item1,
                VerticalAlignment = alignment.Item2,
                TextAlignment = TextAlignment.Right
            };
            return numberBlock;
        }

        private void SetNumberBlockParams(TextBlock numberBlock)
        {
            numberBlock.MinWidth = ViewPadding;
            numberBlock.Padding = new(FontSize / 4);
            numberBlock.TextAlignment = TextAlignment.Right;
        }

        private CartesianCoordinates GetTextOffset(
            (HorizontalAlignment, VerticalAlignment) alignment,
            CartesianCoordinates relativeOffset,
            (HorizontalAlignment, VerticalAlignment) relativeAlignment,
            string text,
            double minHeight = 0,
            double minWidth = 0)
        {
            DpiScale dpi = VisualTreeHelper.GetDpi(this);
            FormattedText formattedText = new(
                text,
                CultureInfo.InvariantCulture,
                FlowDirection.LeftToRight,
                new(FontFamily, FontStyle, FontWeight, FontStretch),
                FontSize,
                StrongStyle.LineBrush,
                dpi.PixelsPerDip);
            double textHeight = formattedText.Height >= minHeight
                ? formattedText.Height
                : minHeight;
            double textWidth = formattedText.Width >= minWidth
                ? formattedText.Width
                : minWidth;
            CartesianCoordinates offset = (relativeOffset.X, relativeOffset.Y);

            if (alignment.Item1 is not HorizontalAlignment.Center
                and not HorizontalAlignment.Stretch)
            {
                if (relativeAlignment.Item1 != alignment.Item1)
                {
                    switch (relativeAlignment.Item1)
                    {
                        case HorizontalAlignment.Left:
                        case HorizontalAlignment.Right:
                            offset.X -= textWidth;
                            break;
                        case HorizontalAlignment.Center:
                            offset.X -= textWidth / 2;
                            break;
                    }
                }
            }

            if (alignment.Item2 is not VerticalAlignment.Center
                and not VerticalAlignment.Stretch)
            {
                if (relativeAlignment.Item2 != alignment.Item2)
                {
                    switch (relativeAlignment.Item2)
                    {
                        case VerticalAlignment.Bottom:
                        case VerticalAlignment.Top:
                            offset.Y -= textHeight;
                            break;
                        case VerticalAlignment.Center:
                            offset.Y -= textHeight / 2;
                            break;
                    }
                }
            }

            return offset;
        }

        private Thickness GetTextMargin(
            (HorizontalAlignment, VerticalAlignment) alignment,
            CartesianCoordinates relativeOffset,
            (HorizontalAlignment, VerticalAlignment) relativeAlignment,
            string text,
            Thickness padding,
            double minHeight = 0,
            double minWidth = 0)
        {
            CartesianCoordinates offset = GetTextOffset(
                alignment,
                relativeOffset,
                relativeAlignment,
                text,
                minHeight: minHeight,
                minWidth: minWidth);
            Thickness margin = new(offset.X, offset.Y, offset.X, offset.Y);

            if (alignment.Item1 is not HorizontalAlignment.Center
                and not HorizontalAlignment.Stretch)
            {
                if (relativeAlignment.Item1 != alignment.Item1)
                {
                    switch (relativeAlignment.Item1)
                    {
                        case HorizontalAlignment.Left:
                            margin.Right -= padding.Left + padding.Right;
                            break;
                        case HorizontalAlignment.Right:
                            margin.Left -= padding.Left + padding.Right;
                            break;
                        case HorizontalAlignment.Center:
                            margin.Left -= padding.Left;
                            margin.Right -= padding.Right;
                            break;
                    }
                }
            }

            if (alignment.Item2 is not VerticalAlignment.Center
                and not VerticalAlignment.Stretch)
            {
                if (relativeAlignment.Item2 != alignment.Item2)
                {
                    switch (relativeAlignment.Item2)
                    {
                        case VerticalAlignment.Bottom:
                            margin.Top -= padding.Bottom + padding.Top;
                            break;
                        case VerticalAlignment.Top:
                            margin.Bottom -= padding.Bottom + padding.Top;
                            break;
                        case VerticalAlignment.Center:
                            margin.Bottom -= padding.Bottom;
                            margin.Top -= padding.Top;
                            break;
                    }
                }
            }

            return margin;
        }

        #endregion

        #region Numbers updating

        private void UpdateNumberValue(
            TextBlock numberBlock,
            double value,
            int precision = 0)
        {
            numberBlock.Text = value.ToString($"N{Math.Abs(precision)}");
        }

        private void UpdateNumberPosition(
            TextBlock numberBlock,
            (HorizontalAlignment, VerticalAlignment) alignment,
            CartesianCoordinates position,
            (HorizontalAlignment, VerticalAlignment) positionRelativeAlignment)
        {
            numberBlock.HorizontalAlignment = alignment.Item1;
            numberBlock.VerticalAlignment = alignment.Item2;
            numberBlock.Margin = GetTextMargin(
                alignment,
                position,
                positionRelativeAlignment,
                numberBlock.Text,
                numberBlock.Padding,
                minWidth: ViewPadding);
        }

        #endregion

        private void UserControl_Loaded(object sender, RoutedEventArgs e)
        {
            if (!IsHandlersEnabled)
                return;

            Draw();
        }

        private void UserControl_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            if (!IsHandlersEnabled)
                return;

            Update();
        }
    }
}