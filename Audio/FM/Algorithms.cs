using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.FM
{

    /// <summary>
    /// FM音源におけるアルゴリズムのデリゲートを定義します。
    /// </summary>
    /// <param name="op">オペレーターのリスト</param>
    /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
    /// <returns>状態</returns>
    public delegate double AlgorithmFunction(IList<Operator> op,ref object tag);

    /// <summary>
    /// FM音源のオペレータの組み合わせのアルゴリズムを提供します。
    /// </summary>
    public static class Algorithms
    {
        public static double 
    }
}
