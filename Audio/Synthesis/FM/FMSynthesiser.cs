using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.Synthesis.FM
{
    /// <summary>
    /// FM音源のシンセサイザーを定義します。
    /// </summary>
    public class FMSynthesiser : ISynthesisable
    {
        FMSynthesisState _state;
        object _tag;
        #region プロパティ
        /// <summary>
        /// デフォルトのオペレータ数を取得します。
        /// </summary>
        public static int DefaultOperatorCount = 4;

        /// <summary>
        /// 現在使用可能なオペレーターのリストを取得・設定します。
        /// </summary>
        public IList<FMOperator> Operators { get; set; }

        /// <summary>
        /// 現在合成されるアルゴリズムを取得・設定します。
        /// </summary>
        public FMAlgorithmFunction Algorithm { get; set; }

        /// <summary>
        /// アルゴリズムに付随するタグオブジェクトを取得・設定します。
        /// </summary>
        public object AlgorithmTag
        {
            get { return _tag; }
            set { _tag = value; }
        }

        /// <summary>
        /// リリースした際に自然につながるように、最終取得時の状態を取得・設定します。
        /// <para></para>
        /// 通常は最後のGetStateが設定されますが、最終実行と最終状態が一致しない場合、
        /// 手動で設定することも出来ます。
        /// </para>
        /// </summary>
        public double LastState { get; set; }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// FMSynthesiserクラスの新しいインスタンスを初期化します。
        /// 4オペレータ、Algorithms.ParallelMixが初期値となります。
        /// </summary>
        public FMSynthesiser()
        {
            Operators = new List<FMOperator>();
            for (int i = 0; i < DefaultOperatorCount; i++)
            {
                Operators.Add(new FMOperator());
            }
            Algorithm = FMAlgorithms.ParallelMixAlgorithm;
        }

        /// <summary>
        /// FMSynthesiserクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="ops">オペレータ数</param>
        /// <param name="algo">アルゴリズム</param>
        public FMSynthesiser(int ops, FMAlgorithmFunction algo)
        {
            Operators = new List<FMOperator>();
            for (int i = 0; i < ops; i++)
            {
                Operators.Add(new FMOperator());
            }
            Algorithm = algo;
        }

        /// <summary>
        /// FMSynthesiserクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="opinfo">オペレータ情報のリスト。このリストの個数がオペレータ数になります。</param>
        /// <param name="algo"></param>
        public FMSynthesiser(IList<FMOperatorInfomation> opinfo, FMAlgorithmFunction algo)
            : this(opinfo.Count, algo)
        {
            for (int i = 0; i < opinfo.Count; i++)
            {
                Operators[i].SetInfomation(opinfo[i]);
            }
        }
        #endregion

        /// <summary>
        /// 発音開始します。
        /// </summary>
        /// <param name="basefreq">基本周波数</param>
        public void Attack(double basefreq)
        {
            _state.Frequency = basefreq;
            _state.IsHolding = true;
            _state.Time = 0.0;
            //_state.ReleaseTime = 0.0;
        }

        /// <summary>
        /// 発音を停止します。
        /// </summary>
        public void Release()
        {
            _state.IsHolding = false;
            _state.Time = 0.0;
        }

        /// <summary>
        /// 指定時刻の状態を取得します。
        /// </summary>
        /// <param name="t">
        /// 時刻。
        /// </param>
        /// <returns>-1.0~+1.0の状態。</returns>
        public double GetState(double t)
        {
            _state.Time = t;
            return Algorithm(Operators, ref _tag, _state);
        }


    }

    /// <summary>
    /// FM音源の合成の状態を定義します。
    /// </summary>
    public struct FMSynthesisState
    {
        /// <summary>
        /// 時刻
        /// <para>このフィールドにオペレータ出力を加算すると、FM合成になります。</para>
        /// </summary>
        internal double Time;

        /// <summary>
        /// 変調の状態
        /// <para>初期状態ではTimeと同値です。</para>
        /// </summary>
        public double State;

        /// <summary>
        /// 周波数
        /// </summary>
        internal double Frequency;

        /// <summary>
        /// ホールド(発音状態)
        /// </summary>
        internal bool IsHolding;
    }

}
