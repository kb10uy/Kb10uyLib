using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.Synthesis.FM
{

    /// <summary>
    /// オシレータのデリゲートを定義します。
    /// </summary>
    /// <param name="t">0~1の周期内のポジション</param>
    /// <param name="p">初期位相</param>
    /// <returns>-1~+1の値</returns>
    public delegate double FMOscillatorFunction(double t, double p);

    /// <summary>
    /// FM音源におけるオシレータ関数を定義します。
    /// 渡す値は必ず0~1である必要があります。
    /// </summary>
    public static class FMOscillators
    {
        /// <summary>
        /// 正弦波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Sine(double t, double p)
        {
            return Math.Sin(Math.PI * 2.0 * t + p);
        }

        /// <summary>
        /// 矩形波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Square(double t, double p)
        {
            return ((t + p) < 0.5 ? 1.0 : -1.0);
        }

        /// <summary>
        /// 三角波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Triangle(double t, double p)
        {
            if ((t + p) < 0.0)
            {
                throw new ArgumentException("TriangleAbsoluteの有効範囲は0≦t≦1です");
            }
            else if ((t + p) <= 0.25)
            {
                return t * 4.0;
            }
            else if ((t + p) <= 0.75)
            {
                return t * -4.0 + 2.0;
            }
            else if ((t + p) <= 1.0)
            {
                return (t + p) * 4.0 - 4.0;
            }
            else
            {
                throw new ArgumentException("TriangleAbsoluteの有効範囲は0≦t≦1です");
            }
        }

        /// <summary>
        /// 上行形ノコギリ波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double UpSaw(double t, double p)
        {
            return (t + p) * 2.0 - 1.0;
        }

        /// <summary>
        /// 下行形ノコギリ波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double DownSaw(double t, double p)
        {
            return (t + p) * -2.0 + 1.0;
        }

    }
}
