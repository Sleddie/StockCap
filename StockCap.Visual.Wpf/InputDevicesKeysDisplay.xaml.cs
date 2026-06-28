using System.Windows.Controls;
using System.Windows.Input;

namespace StockCap.Visual.Wpf
{
    public partial class InputDevicesKeysDisplay : UserControl
    {
        public InputDevicesKeysDisplay()
        {
            InitializeComponent();
        }

        public KeyboardDevice? LastKeyBoard { get; set; }

        public MouseDevice? LastMouse { get; set; }

        public ModifierKeys Modifiers =>
            LastKeyBoard?.Modifiers ?? ModifierKeys.None;

        public Key LastKey
        {
            get;
            set
            {
                field = value;
                UpdateKeysState();
            }
        }

        public MouseButtonState LeftButtonState =>
            LastMouse?.LeftButton ?? MouseButtonState.Released;

        public MouseButtonState RightButtonState =>
            LastMouse?.RightButton ?? MouseButtonState.Released;

        public MouseButtonState MiddleButtonState =>
            LastMouse?.MiddleButton ?? MouseButtonState.Released;

        public void UpdateKeysState()
        {
            _modifierKeyTextBlock.Text = Modifiers.ToString();
            _keyTextBlock.Text = LastKey.ToString();
            _mouseLeftButtonTextBlock.Text = LeftButtonState.ToString();
            _mouseRightButtonTextBlock.Text = RightButtonState.ToString();
            _mouseMiddleButtonTextBlock.Text = MiddleButtonState.ToString();
        }
    }
}