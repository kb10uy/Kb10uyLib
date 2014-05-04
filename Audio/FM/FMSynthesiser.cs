using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.FM
{
    /// <summary>
    /// FM音源のシンセサイザーを定義します。
    /// </summary>
    public class FMSynthesiser
    {
        /// <summary>
        /// 現在使用可能なオペレーターのリストを取得・設定します。
        /// </summary>
        public IList<Operator> Operators { get; set; }

        /// <summary>
        /// 現在合成されるアルゴリズムを取得・設定します。
        /// </summary>
        public AlgorithmFunction Algorithm { get; set; }

        /// <summary>
        /// FMSynthesiserクラスの新しいインスタンスを初期化します。
        /// 8オペレーター
        /// </summary>
        public FMSynthesiser()
        {

        }
    }
}
