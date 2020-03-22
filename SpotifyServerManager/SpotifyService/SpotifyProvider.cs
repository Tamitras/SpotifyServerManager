using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SpotifyService
{
    public class SpotifyProvider
    {
        public Process SpotifyProcess { get; set; }
        public SpotifyProvider()
        {
        }

        public string Connect()
        {
            var title = GetSpotifyProcess();
            GetWindowHandle();
            return title;
        }


        private string GetSpotifyProcess()
        {
            Thread.Sleep(100);
            Process[] processes = Process.GetProcessesByName("Spotify");
            
            if (processes.Length == 1)
            {
                Console.WriteLine("Process Spotify found");
            }
            else if (processes.Length > 1)
            {
                foreach (var item in processes)
                {
                    Console.WriteLine($"Prozess: {item.ProcessName}, ID: {item.Id}, Name: {item.MainWindowTitle}");
                    if (!string.IsNullOrEmpty(item.MainWindowTitle))
                    {
                        this.SpotifyProcess = item;
                        return item.MainWindowTitle.Contains("Spotify") ? "Pausiert" : item.MainWindowTitle;
                    }
                }
            }
            return "NotConnected";
        }


        /// <summary>
        /// Setzt die Sichtbarkeit des WIndows
        /// 0 = 0%
        /// 255 = 100%
        /// </summary>
        /// <param name="opacity"></param>
        public void SetOpacity(byte opacity = 255)
        {
            SetLayeredWindowAttributes(this.SpotifyProcess.MainWindowHandle, 0, opacity, LWA_ALPHA);
        }

        public void GetWindowHandle()
        {
            if (SpotifyProcess == null)
            {
                throw new Exception("Kein Spotify geöffnet");
            }
            IntPtr mainWindowHandle = SpotifyProcess.MainWindowHandle;

            SetForegroundWindow(mainWindowHandle);
            //ShowWindow(mainWindowHandle, WindowState.ShowDefault);
            //ClickOnPointTool.ClickOnPoint(mainWindowHandle, new Point(375, 340));
        }

        private void ShowWindow()
        {
            IntPtr mainWindowHandle = SpotifyProcess.MainWindowHandle;
            //SetLayeredWindowAttributes(this.SpotifyProcess.MainWindowHandle, 0, 0, LWA_ALPHA);

            WINDOWPLACEMENT param = new WINDOWPLACEMENT();
            param.rcNormalPosition = new Rectangle(new Point(0, -2000), new Size(0, 0));
            param.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            param.showCmd = (int)WindowState.ShowDefault;

            SetWindowPlacement(mainWindowHandle, ref param);
            Thread.Sleep(1000);
        }

        public void ResetWindow()
        {
            IntPtr mainWindowHandle = SpotifyProcess.MainWindowHandle;
            //SetLayeredWindowAttributes(this.SpotifyProcess.MainWindowHandle, 0, 255, LWA_ALPHA);
            SetForegroundWindow(mainWindowHandle);
            ShowWindow(mainWindowHandle, WindowState.ShowDefault);

            WINDOWPLACEMENT param = new WINDOWPLACEMENT();
            param.rcNormalPosition = new Rectangle(new Point(200, 200), new Size(600, 600));
            param.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            param.showCmd = (int)WindowState.ShowDefault;
            SetWindowPlacement(mainWindowHandle, ref param);
        }

        private void HideWindow() 
        {
            IntPtr mainWindowHandle = SpotifyProcess.MainWindowHandle;
            SetLayeredWindowAttributes(this.SpotifyProcess.MainWindowHandle, 0, 0, LWA_ALPHA);

            WINDOWPLACEMENT param = new WINDOWPLACEMENT();
            param.rcNormalPosition = new Rectangle(new Point(1500, 0), new Size(0, 0));
            param.length = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            param.showCmd = (int)WindowState.Minimize;
            SetWindowPlacement(mainWindowHandle, ref param);

            SetLayeredWindowAttributes(this.SpotifyProcess.MainWindowHandle, 0, 255, LWA_ALPHA);
            //Thread.Sleep(500);
        }

        public Task<string> PerformPlayAsync()
        {
            Task<string> ret = Task.Factory.StartNew(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                uint keydown = 0x0100;
                uint keyup = 0x0200;
                Process p = this.SpotifyProcess;

                this.ShowWindow();
                PostMessage(p.MainWindowHandle, keydown, (int)Keys.Space, 0);
                PostMessage(p.MainWindowHandle, keyup, (int)Keys.Space, 0);
                this.HideWindow();

                watch.Stop();
                Console.WriteLine("Ausführung PlayPause" + (watch.ElapsedMilliseconds /1000.0) + "s");

                return GetSpotifyProcess();
            });

            return ret;
        }

        public Task<string> PerformNextSongAsync()
        {
            Task<string> ret = Task.Factory.StartNew(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                uint WM_KEYDOWN = 0x0100;
                uint WM_KEYUP = 0x0101;
                int KEYEVENTF_KEYDOWN = 0x0000; // New definition
                int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
                int KEYEVENTF_KEYUP = 0x0002; //Key up flag
                int VK_LCONTROL = 0xA2; //Left Control key codes

                Process p = this.SpotifyProcess;
                //p.WaitForInputIdle();

                this.ShowWindow();

                keybd_event((byte)VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
                Thread.Sleep(50);
                PostMessage(p.MainWindowHandle, WM_KEYDOWN, (int)Keys.Right, 0);
                PostMessage(p.MainWindowHandle, WM_KEYUP, (int)Keys.Right, 0);
                keybd_event((byte)VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);

                this.HideWindow();

                watch.Stop();
                Console.WriteLine("Ausführung NextSong" + (watch.ElapsedMilliseconds / 1000.0) + "s");

                return GetSpotifyProcess();
            });

            return ret;
        }

        public Task<string> PerformPreviousSongAsync()
        {
            Task<string> ret = Task.Factory.StartNew(() =>
            {
                Stopwatch watch = new Stopwatch();
                watch.Start();

                uint WM_KEYDOWN = 0x0100;
                uint WM_KEYUP = 0x0101;
                int KEYEVENTF_KEYDOWN = 0x0000; // New definition
                int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
                int KEYEVENTF_KEYUP = 0x0002; //Key up flag
                int VK_LCONTROL = 0xA2; //Left Control key code

                Process p = this.SpotifyProcess;
                //p.WaitForInputIdle();

                this.ShowWindow();
                keybd_event((byte)VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);

                Thread.Sleep(50);
                PostMessage(p.MainWindowHandle, WM_KEYDOWN, (int)Keys.Left, 0);
                PostMessage(p.MainWindowHandle, WM_KEYUP, (int)Keys.Left, 0); 
                keybd_event((byte)VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);

                this.HideWindow();

                watch.Stop();
                Console.WriteLine("Ausführung Vorheriger Song" + (watch.ElapsedMilliseconds / 1000.0) + "s");

                return GetSpotifyProcess();
            });
            return ret;
        }

        public void PerformIncreaseVolume()
        {
            uint WM_KEYDOWN = 0x0100;
            uint WM_KEYUP = 0x0101;
            int KEYEVENTF_KEYDOWN = 0x0000; // New definition
            int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
            int KEYEVENTF_KEYUP = 0x0002; //Key up flag
            int VK_LCONTROL = 0xA2; //Left Control key code

            Process p = this.SpotifyProcess;
            //p.WaitForInputIdle();
            this.ShowWindow();
            keybd_event((byte)VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            Thread.Sleep(50);
            PostMessage(p.MainWindowHandle, WM_KEYDOWN, (int)Keys.Up, 0);
            PostMessage(p.MainWindowHandle, WM_KEYUP, (int)Keys.Up, 0);

            keybd_event((byte)VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);

            this.HideWindow();
        }

        public void PerformDecreaseVolume()
        {
            uint WM_KEYDOWN = 0x0100;
            uint WM_KEYUP = 0x0101;
            int KEYEVENTF_KEYDOWN = 0x0000; // New definition
            int KEYEVENTF_EXTENDEDKEY = 0x0001; //Key down flag
            int KEYEVENTF_KEYUP = 0x0002; //Key up flag
            int VK_LCONTROL = 0xA2; //Left Control key code

            Process p = this.SpotifyProcess;
            //p.WaitForInputIdle();
            this.ShowWindow();
            keybd_event((byte)VK_LCONTROL, 0, KEYEVENTF_KEYDOWN, 0);
            Thread.Sleep(50);
            PostMessage(p.MainWindowHandle, WM_KEYDOWN, (int)Keys.Down, 0);
            PostMessage(p.MainWindowHandle, WM_KEYUP, (int)Keys.Down, 0);
            keybd_event((byte)VK_LCONTROL, 0, KEYEVENTF_KEYUP, 0);

            this.HideWindow();
        }

        /// <summary>
        /// Erstellt ein Screenshot von dem aktuellen Prozess(Window)
        /// </summary>
        /// <param name="proc"></param>
        public Bitmap CaptureApplication()
        {
            Process proc = this.SpotifyProcess;

            var rect = new User32.Rect();
            User32.GetWindowRect(proc.MainWindowHandle, ref rect);

            int width = rect.right - rect.left;
            int height = rect.bottom - rect.top;

            var currentScreenImage = new Bitmap(width, height, PixelFormat.Format32bppArgb);

            Graphics g = Graphics.FromImage(currentScreenImage);
            g.CopyFromScreen(rect.left, rect.top, 0, 0, new Size(width, height), CopyPixelOperation.SourceCopy);

            return currentScreenImage;
        }

        public void ResetAndInitOpacity()
        {
            SetWindowLong(this.SpotifyProcess.MainWindowHandle, GWL_EXSTYLE, GetWindowLong(this.SpotifyProcess.MainWindowHandle, GWL_EXSTYLE) ^ WS_EX_LAYERED);
        }

        public void MakeExternalWindowBorderless()
        {
            var handle = this.SpotifyProcess.MainWindowHandle;
            int Style = 0;
            Style = GetWindowLong(handle, GWL_STYLE);
            Style = Style & ~WS_CAPTION;
            Style = Style & ~WS_SYSMENU;
            Style = Style & ~WS_THICKFRAME;
            Style = Style & ~WS_MINIMIZE;
            Style = Style & ~WS_MAXIMIZEBOX;
            SetWindowLong(handle, GWL_STYLE, Style);
            Style = GetWindowLong(handle, GWL_EXSTYLE);
            SetWindowLong(handle, GWL_EXSTYLE, Style | WS_EX_DLGMODALFRAME);

            //SetWindowPos(MainWindowHandle, new IntPtr(0), 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE | SWP_FRAMECHANGED);
        }

        //Import the DLL
        [DllImport("user32.dll")]
        static extern bool SetWindowPlacement(IntPtr hWnd,
        [In] ref WINDOWPLACEMENT lpwndpl);

        //Definition for Window Placement Structure
        [StructLayout(LayoutKind.Sequential)]
        private struct WINDOWPLACEMENT
        {
            public int length;
            public int flags;
            public int showCmd;
            public System.Drawing.Point ptMinPosition;
            public System.Drawing.Point ptMaxPosition;
            public System.Drawing.Rectangle rcNormalPosition;
        }

        // add this to your variable declaration area
        [DllImport("user32.dll")]
        public static extern bool PostMessage(IntPtr hWnd, uint Msg, int wParam, int lParam);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);


        //gets information about the windows
        [DllImport("user32.dll", SetLastError = true)]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        //sets bigflags that control the windows styles
        [DllImport("user32.dll")]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);

        //public const int GWL_EXSTYLE = -20;
        public const int WS_EX_LAYERED = 0x80000;
        public const int LWA_ALPHA = 0x2;
        public const int LWA_COLORKEY = 0x1;

        const int WS_BORDER = 8388608;
        const int WS_DLGFRAME = 4194304;
        const int WS_CAPTION = WS_BORDER | WS_DLGFRAME;
        const int WS_SYSMENU = 524288;
        const int WS_THICKFRAME = 262144;
        const int WS_MINIMIZE = 536870912;
        const int WS_MAXIMIZEBOX = 65536;
        const int GWL_STYLE = -16;
        const int GWL_EXSTYLE = -20;
        const int WS_EX_DLGMODALFRAME = 0x1;
        const int SWP_NOMOVE = 0x2;
        const int SWP_NOSIZE = 0x1;
        const int SWP_FRAMECHANGED = 0x20;
        const uint MF_BYPOSITION = 0x400;
        const uint MF_REMOVE = 0x1000;

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool SetForegroundWindow(IntPtr hWnd);

        public enum WindowState
        {
            Hide = 0,
            ShowNormal = 1,
            ShowMinimized = 2,
            ShowMaximized = 3,
            ShowNoActivate = 4,
            Show = 5,
            Minimize = 6,
            ShowMinNoActive = 7,
            ShowNA = 8,
            Restore = 9,
            ShowDefault = 10
        }

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, WindowState wFlags);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        public static extern int ShowWindow(IntPtr hWnd, int wFlags);

        // P/Invoke declarations
        [DllImport("gdi32.dll")]
        static extern bool BitBlt(IntPtr hdcDest, int xDest, int yDest, int
        wDest, int hDest, IntPtr hdcSource, int xSrc, int ySrc, CopyPixelOperation rop);
        [DllImport("user32.dll")]
        static extern bool ReleaseDC(IntPtr hWnd, IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteDC(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr DeleteObject(IntPtr hDc);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleBitmap(IntPtr hdc, int nWidth, int nHeight);
        [DllImport("gdi32.dll")]
        static extern IntPtr CreateCompatibleDC(IntPtr hdc);
        [DllImport("gdi32.dll")]
        static extern IntPtr SelectObject(IntPtr hdc, IntPtr bmp);
        [DllImport("user32.dll")]
        public static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        public static extern IntPtr GetWindowDC(IntPtr ptr);

    }
}
