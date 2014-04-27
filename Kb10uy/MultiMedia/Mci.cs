using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Kb10uy.MultiMedia
{
    /// <summary>
    /// Media Control Interfaceへのアクセスをサポートします。
    /// </summary>
    public static class Mci
    {
        [DllImport("winmm.dll",CharSet=CharSet.Auto,SetLastError=true)]
        static extern uint mciSendString(string cmd, IntPtr buf, uint ret, IntPtr hwnd);

        /// <summary>
        /// コマンド文字列を送信します。
        /// </summary>
        /// <param name="cmd">コマンド文字列</param>
        public static void SendString(string cmd)
        {
            mciSendString(cmd, IntPtr.Zero, 0, IntPtr.Zero);
        }

        /// <summary>
        /// コマンド文字列を送信します。
        /// コマンドに"notify"が含まれる場合、指定のハンドルのウィンドウに
        /// メッセージが送信されます。
        /// </summary>
        /// <param name="cmd">コマンド文字列</param>
        /// <param name="hwnd">ウィンドウハンドル</param>
        public static void SendString(string cmd, IntPtr hwnd)
        {
            mciSendString(cmd, IntPtr.Zero, 0, hwnd);
        }

    }
}
