using System.Windows;
using System.Windows.Input;

namespace StockCap.Visual.Wpf
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private bool IsMoving { get; set; }

        private void SwitchMaximized() =>
            WindowState = WindowState is WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

        private void SwitchKeyDisplay() =>
            _keyDisplay.Visibility =
            _keyDisplay.Visibility is Visibility.Visible
                ? Visibility.Collapsed
                : Visibility.Visible;

        private void CheckMoving()
        {
            IsMoving = _keyDisplay.Modifiers is ModifierKeys.Control
                && _keyDisplay.LeftButtonState is MouseButtonState.Pressed;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            _keyDisplay.LastKeyBoard = e.Device as KeyboardDevice;
            ModifierKeys modifiers = _keyDisplay.LastKeyBoard is not null
                ? _keyDisplay.LastKeyBoard.Modifiers
                : ModifierKeys.None;
            _keyDisplay.LastKey = e.Key;
            _keyDisplay.UpdateKeysState();
            CheckMoving();

            switch (e.Key)
            {
                case Key.F11:
                    SwitchMaximized();
                    return;
            }

            switch (modifiers)
            {
                case ModifierKeys.Control:
                    switch (e.Key)
                    {
                        case Key.Enter:
                            SwitchMaximized();
                            return;
                    }
                    break;
                case ModifierKeys.Control | ModifierKeys.Shift:
                    switch (e.Key)
                    {
                        case Key.F1:
                            SwitchKeyDisplay();
                            return;
                    }
                    break;
            }
        }

        private void Window_KeyUp(object sender, KeyEventArgs e)
        {
            _keyDisplay.LastKeyBoard = e.Device as KeyboardDevice;
            _keyDisplay.LastKey = e.Key;
            _keyDisplay.UpdateKeysState();
            CheckMoving();
        }

        private void Window_MouseDown(object sender, MouseButtonEventArgs e)
        {
            _keyDisplay.LastMouse = e.Device as MouseDevice;
            _keyDisplay.UpdateKeysState();
            CheckMoving();
        }

        private void Window_MouseUp(object sender, MouseButtonEventArgs e)
        {
            _keyDisplay.LastMouse = e.Device as MouseDevice;
            _keyDisplay.UpdateKeysState();
            CheckMoving();
        }

        private void Window_MouseMove(object sender, MouseEventArgs e)
        {
            _keyDisplay.LastMouse = e.Device as MouseDevice;

            if (IsMoving)
            {
                if (IsMoving && WindowState is WindowState.Maximized)
                    SwitchMaximized();

                try
                {
                    DragMove();
                }
                catch (InvalidOperationException) { }
            }
        }
    }
}