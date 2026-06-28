using System.Windows;

namespace StockCap.Visual.Wpf
{
    public class CartesianCoordinates(FrameworkElement planeControl)
    {
        public double X { get; set; }
        public double Y { get; set; }

        protected FrameworkElement PlaneControl { get; init; } = planeControl;

        public double RealX => X;
        public double RealY => PlaneControl.ActualHeight - Y;

        public static FrameworkElement? DefaultPlaneControl { get; set; }

        public static implicit operator CartesianCoordinates((double, double) coordinates)
        {
            ArgumentNullException.ThrowIfNull(DefaultPlaneControl, nameof(DefaultPlaneControl));

            return new(DefaultPlaneControl)
            {
                X = coordinates.Item1,
                Y = coordinates.Item2
            };
        }
    }
}