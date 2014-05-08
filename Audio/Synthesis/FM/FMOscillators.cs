using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kb10uy.Extension;

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
        const double DoublePI = Math.PI * 2;

        /// <summary>
        /// 正弦波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Sine(double t, double p)
        {
            return Math.Sin(DoublePI * t + p);
        }

        /// <summary>
        /// 矩形波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Square(double t, double p)
        {
            return Math.Sign(Math.Sin(DoublePI * t + p));
        }

        /// <summary>
        /// 正弦波を利用した擬似的な矩形波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double SquareBySine(double t, double p)
        {
            return MathEx.Square(DoublePI * t + p);
        }

        /// <summary>
        /// 正弦波を利用した擬似的な矩形波オシレータを定義します。
        /// </summary>
        /// <param name="kmax">級数の展開回数</param>
        /// <returns>オシレータのデリゲート</returns>
        public static FMOscillatorFunction SquareBySine(int kmax)
        {
            var m = kmax;
            return (t, p) => MathEx.Square(DoublePI * t + p, m);
        }

        /// <summary>
        /// 三角波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double Triangle(double t, double p)
        {
            var hp = Math.PI / 2.0;
            var pos = (t * DoublePI + p) % DoublePI;
            pos = pos < 0 ? pos + DoublePI : pos;

            if (pos <= hp)
            {
                return (pos / DoublePI) * 4.0;
            }
            else if (pos <= hp * 3)
            {
                return (pos / DoublePI) * -4.0 + 2.0;
            }
            else if (pos <= hp * 4)
            {
                return (pos / DoublePI) * 4.0 - 4.0;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 正弦波を利用した擬似的な三角波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double TriangleBySine(double t, double p)
        {
            return MathEx.Triangle(DoublePI * t + p);
        }

        /// <summary>
        /// 正弦波を利用した擬似的な三角波オシレータを定義します。
        /// </summary>
        /// <param name="kmax">級数の展開回数</param>
        /// <returns>オシレータのデリゲート</returns>
        public static FMOscillatorFunction TriangleBySine(int kmax)
        {
            var m = kmax;
            return (t, p) => MathEx.Triangle(DoublePI * t + p, m);
        }

        /// <summary>
        /// 上行形ノコギリ波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double UpSaw(double t, double p)
        {
            return ((t * DoublePI + p) % DoublePI) * 2.0 - 1.0;
        }

        /// <summary>
        /// 下行形ノコギリ波オシレータを定義します。
        /// </summary>
        /// <param name="t">周期内ポジション</param>
        /// <param name="p">初期位相</param>
        /// <returns>-1.0~1.0までの範囲の値</returns>
        public static double DownSaw(double t, double p)
        {
            return ((t * DoublePI + p) % DoublePI) * -2.0 + 1.0;
        }

    }
}
