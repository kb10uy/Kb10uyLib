using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;

namespace Kb10uy.DxLib
{
    /// <summary>
    /// ちょっと便利に
    /// </summary>
    public static class ExtraDxLib
    {
        /// <summary>
        /// 白でちゃっちゃと文字列を描画します。
        /// </summary>
        /// <param name="x">開始X</param>
        /// <param name="y">開始Y</param>
        /// <param name="str">文字列</param>
        public static void DrawStrings(int x, int y, params string[] str)
        {
            for (int i = 0; i < str.Length; i++)
            {
                DX.DrawString(x, y + 16 * i, str[i], DX.GetColor(255,255,255));
            }
        }

        /// <summary>
        /// 元のハンドルの画像から、指定範囲を切り出してあたらしいハンドルを生成します。
        /// </summary>
        /// <param name="org">元ハンドル</param>
        /// <param name="r">切り出す範囲</param>
        /// <returns>新しいハンドル</returns>
        public static int DerivationGraphFromRegion(int org, Region r)
        {
            return DX.DerivationGraph((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height, org);
        }

    }

}
