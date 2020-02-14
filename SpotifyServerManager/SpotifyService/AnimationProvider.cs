using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace SpotifyService
{
    /// <summary>
    /// This Provider handles the Windows Animations
    /// </summary>
    public class AnimationProvider
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct ANIMATIONINFO
        {
            public uint cbSize;
            public int iMinAnimate;
        };

        [DllImport("user32", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool SystemParametersInfo(uint uiAction, uint uiParam, ref ANIMATIONINFO pvParam, uint fWinIni);

        public static uint SPIF_SENDCHANGE = 0x02;
        public static uint SPI_SETANIMATION = 0x0049;

        public void DiableAnimation()
        {
            ANIMATIONINFO ai = new ANIMATIONINFO();
            ai.cbSize = (uint)Marshal.SizeOf(ai);
            ai.iMinAnimate = 0;   // turn all animation off
            SystemParametersInfo(SPI_SETANIMATION, 0, ref ai, SPIF_SENDCHANGE);
        }

        public void EnableAnimation()
        {
            ANIMATIONINFO ai = new ANIMATIONINFO();
            ai.cbSize = (uint)Marshal.SizeOf(ai);
            ai.iMinAnimate = 1;   // turn all animation off
            SystemParametersInfo(SPI_SETANIMATION, 0, ref ai, SPIF_SENDCHANGE);
        }

    }
}
