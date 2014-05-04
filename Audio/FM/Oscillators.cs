using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.FM
{

    /// <summary>
    /// オシレータの
    /// </summary>
    /// <param name="t"></param>
    /// <returns></returns>
    public delegate double OscillatorFunction(double t);

    /// <summary>
    /// FM音源におけるオシレータ関数を定義します。
    /// 渡す値は必ず0~1である必要があります。
    /// </summary>
    public static class Oscillators
    {
        /// <summary>
        /// 正弦波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Sine(double t)
        {
            return Math.Sin(Math.PI * t);
        }

        /// <summary>
        /// 矩形波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Square(double t)
        {
            return (t < 0.5 ? 1.0 : -1.0);
        }

        /// <summary>
        /// 三角波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Triangle(double t)
        {
            if (t < 0.0)
            {
                throw new ArgumentException("TriangleAbsoluteの有効範囲は0≦t≦1です");
            }
            else if (t <= 0.25)
            {
                return t * 4.0;
            }
            else if (t <= 0.75)
            {
                return t * -4.0 + 2.0;
            }
            else if (t <= 1.0)
            {
                return t * 4.0 - 4.0;
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
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double UpSaw(double t)
        {
            return t * 2.0 - 1.0;
        }

        /// <summary>
        /// 下行形ノコギリ波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double DownSaw(double t)
        {
            return t * -2.0 + 1.0;
        }

    }
}
