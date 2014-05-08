using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.InteropServices;

namespace Kb10uy.Windows
{
    /// <summary>
    /// Microsoft Visual Cランタイムライブラリの機能を提供します。
    /// </summary>
    public static class Msvcrt
    {
        /// <summary>
        /// 数学関係の関数(math.h)を提供します。
        /// </summary>
        public static class Math
        {
            /// <summary>
            /// 指定された角度のサインを返します。
            /// </summary>
            /// <param name="t">位相</param>
            /// <returns>値</returns>
            [DllImport("msvcrt.dll", EntryPoint = "sin", CallingConvention = CallingConvention.Cdecl)]
            public static extern double Sin(double t);
        }
    }
}
