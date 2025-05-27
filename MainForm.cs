using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenCvSharp;

namespace RFBot
{
    public partial class MainForm : Form
    {
        private bool _botActive = false;
        private Mat _prevGray = new Mat();
        private Rectangle _gameRect;

        public MainForm()
        {
            InitializeComponent();
            InitializeGameWindow();
        }

        private void InitializeComponent()
        {
            Width = 400;
            Height = 200;
            Text = "RF Bot";
        }

        private void InitializeGameWindow()
        {
            var hwnd = Win32.FindWindow(null, "[PREMIUM] RF Online");
            if (hwnd != IntPtr.Zero)
            {
                Win32.GetWindowRect(hwnd, out var rect);
                _gameRect = new Rectangle(rect.Left, rect.Top, rect.Right - rect.Left, rect.Bottom - rect.Top);
                Console.WriteLine($"Окно найдено: {_gameRect}");
            }
            else
            {
                Console.WriteLine("Окно не найдено — используется весь экран");
            }
        }

        public async Task RunAsync()
        {
            KeyboardHook.Start();

            while (true)
            {
                var frame = ScreenCapture.Capture(_gameRect);
                if (frame == null) continue;

                var mobs = MobDetector.Detect(frame, ref _prevGray);

                if (_botActive && mobs.Length > 0)
                    AttackMob(mobs[0]);

                await Task.Delay(10);
            }
        }

        private void AttackMob(OpenCvSharp.Point mobPos)
        {
            int x = mobPos.X + _gameRect.X;
            int y = mobPos.Y + _gameRect.Y;
            Win32.SetCursorPos(x, y);
            Win32.PressKey(Win32.VirtualKeyCodes.VK_SPACE);
        }
    }
}
