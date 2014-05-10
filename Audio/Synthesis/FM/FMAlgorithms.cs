using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.Synthesis.FM
{

    /// <summary>
    /// FM音源におけるアルゴリズムのデリゲートを定義します。
    /// </summary>
    /// <param name="op">オペレーターのリスト</param>
    /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
    /// <param name="state">合成状態の情報</param>
    /// <returns>状態</returns>
    public delegate double FMAlgorithmFunction(IList<FMOperator> op, ref object tag, FMSynthesisState state);

    /// <summary>
    /// FM音源のオペレータの組み合わせのアルゴリズムを提供します。
    /// </summary>
    public static class FMAlgorithms
    {
        /// <summary>
        /// 最初のチャンネルをキャリア、次のチャンネルをモジュレータとして変調します。
        /// それ以降のオペレータは無視されます。
        /// </summary>
        /// <param name="op">オペレーターのリスト</param>
        /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
        /// <param name="state">合成状態の情報</param>
        /// <returns>状態</returns>
        public static double PairModulationAlgorithm(IList<FMOperator> op, ref object tag, FMSynthesisState state)
        {
            var s = state;
            if (op.Count < 2) throw new ArgumentException("オペレータは最低2つ必要です");
            var mod = op[1].GetState(s);
            s.State = mod;
            return op[0].GetState(s) / op[0].ModulationIndex;
        }

        /// <summary>
        /// 全てのチャンネルを独立して計算し、
        /// 全て同じ比率でミックスします。
        /// <para>厳密にはこれはFM音源にはなりません。</para>
        /// </summary>
        /// <param name="op">オペレーターのリスト</param>
        /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
        /// <param name="state">合成状態の情報</param>
        /// <returns>状態</returns>
        public static double ParallelMixAlgorithm(IList<FMOperator> op, ref object tag, FMSynthesisState state)
        {
            var mix = 0.0;
            var max = op.Sum(p => p.ModulationIndex);
            foreach (var i in op)
            {
                mix += i.GetState(state);
            }
            return mix / max;
        }

        /// <summary>
        /// 先頭をキャリア、それ以降をモジュレータとして、
        /// 全て直列に接続して変調します。
        /// </summary>
        /// <param name="op">オペレーターのリスト</param>
        /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
        /// <param name="state">合成状態の情報</param>
        /// <returns>状態</returns>
        public static double SerialModulationAlgorithm(IList<FMOperator> op, ref object tag, FMSynthesisState state)
        {
            var s = state;
            var mix = 0.0;
            for (int i = op.Count - 1; i >= 0; i--)
            {
                mix = op[i].GetState(s);
                s.State = mix;
            }
            return mix / op[0].ModulationIndex;
        }

        #region フィードバック
        /// <summary>
        /// フィードバックを利用できるアルゴリズムを定義します。
        /// </summary>
        public static class Feedbacking
        {
            /// <summary>
            /// 単一のオペレータが自己フィードバックになる直列のアルゴリズムを定義します。
            /// <para>呼び出しごとに更新されるので、呼び出し間で大きく時刻が違う場合想定されない結果になる恐れがあります。</para>
            /// <para>常に一定時間前の状態を反映したい場合、ConstantFeedbackAlgorithmを使用してください。</para>
            /// </summary>
            public class SelfFeedbackAlgorithm
            {
                /// <summary>
                /// 自己フィードバックになるチャンネルを取得します。
                /// </summary>
                public int FeedbackChannel { get; protected set; }

                /// <summary>
                /// 自己フィードバックになっているチャンネルの前回の状態を取得します。
                /// </summary>
                public double PreviousFeedbackState { get; protected set; }

                /// <summary>
                /// 新しいインスタンスを初期化します。
                /// </summary>
                /// <param name="fbch">自己フィードバックするチャンネル</param>
                private SelfFeedbackAlgorithm(int fbch)
                {
                    FeedbackChannel = fbch;
                }

                /// <summary>
                /// SelfFeedbackAlgorithmを作成します。
                /// 指定したチャンネルが自己フィードバックになります。
                /// </summary>
                /// <param name="fbch">自己フィードバックするチャンネル</param>

                /// <returns>アルゴリズム</returns>
                public static SelfFeedbackAlgorithm Create(int fbch)
                {
                    return new SelfFeedbackAlgorithm(fbch);
                }

                /// <summary>
                /// 現在の状態でのアルゴリズムを定義します。
                /// </summary>
                /// <param name="op">オペレーターのリスト</param>
                /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
                /// <param name="state">合成状態の情報</param>
                /// <returns>状態</returns>
                public double Algorithm(IList<FMOperator> op, ref object tag, FMSynthesisState state)
                {
                    var s = state;
                    var mix = 0.0;
                    for (int i = op.Count - 1; i >= 0; i--)
                    {
                        if (i == FeedbackChannel)
                        {
                            s.State += PreviousFeedbackState;
                        }
                        mix = op[i].GetState(s);
                        if (i == FeedbackChannel)
                        {
                            PreviousFeedbackState = mix;
                        }
                        s.State = mix;
                    }
                    return mix / op[0].ModulationIndex;
                }
            }

            /// <summary>
            /// 常に一定時間前の状態を参照する自己フィードバックを含むアルゴリズムを定義します。
            /// </summary>
            public class ConstantSelfFeedbackAlgorithm
            {
                /// <summary>
                /// 自己フィードバックになるチャンネルを取得します。
                /// </summary>
                public int FeedbackChannel { get; protected set; }

                /// <summary>
                /// フィードバックを処理する際に一旦巻き戻す時間を取得します。
                /// </summary>
                public double FeedbackReferenceBackingtime { get; protected set; }

                /// <summary>
                /// 新しいインスタンスを初期化します。
                /// </summary>
                /// <param name="fbch">自己フィードバックするチャンネル</param>
                /// <param name="pref">参照するフィードバックの時間の差</param>
                private ConstantSelfFeedbackAlgorithm(int fbch, double pref)
                {
                    FeedbackChannel = fbch;
                }

                /// <summary>
                /// ConstantSelfFeedbackAlgorithmを作成します。
                /// 指定したチャンネルが自己フィードバックになります。
                /// </summary>
                /// <param name="fbch">自己フィードバックするチャンネル</param>
                /// <param name="pref">参照するフィードバックの時間の差</param>
                /// <returns>アルゴリズム</returns>
                public static ConstantSelfFeedbackAlgorithm Create(int fbch, double pref)
                {
                    return new ConstantSelfFeedbackAlgorithm(fbch, pref);
                }

                /// <summary>
                /// 現在の状態でのアルゴリズムを定義します。
                /// </summary>
                /// <param name="op">オペレーターのリスト</param>
                /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
                /// <param name="state">合成状態の情報</param>
                /// <returns>状態</returns>
                public double Algorithm(IList<FMOperator> op, ref object tag, FMSynthesisState state)
                {
                    var s = state;
                    var mix = 0.0;
                    for (int i = op.Count - 1; i >= 0; i--)
                    {
                        if (i == FeedbackChannel)
                        {
                            var fbs = state;
                            fbs.Time -= FeedbackReferenceBackingtime;
                            s.State += op[i].GetState(fbs);
                        }
                        mix = op[i].GetState(s);
                        s.State = mix;
                    }
                    return mix / op[0].ModulationIndex;
                }
            }

        }

        #endregion

        /// <summary>
        /// YAMAHA OPN系(OPNA含む)を再現したアルゴリズムを提供します。
        /// <para>
        /// オペレータの順番が逆になっている(キャリア0、モジュレータ1,2,3)ので、注意してください。
        /// </para>
        /// <para>
        /// 全て4オペレータ使用します。
        /// 足りない場合は例外、多い場合はその分が無視されます。
        /// </para>
        /// </summary>
        public static class Opn
        {
            static FMAlgorithmFunction _serial = Feedbacking.ConstantSelfFeedbackAlgorithm.Create(3, 0.001).Algorithm;

            /// <summary>
            /// 0番のアルゴリズムを定義します。
            /// </summary>
            /// <param name="op">オペレーターのリスト</param>
            /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
            /// <param name="state">合成状態の情報</param>
            /// <returns>状態</returns>
            public static double ZeroSerial(IList<FMOperator> op, ref object tag, FMSynthesisState state)
            {
                if (op.Count < 4) throw new InvalidOperationException("OPN系アルゴリズムではオペレータは4つ必要です");
                return _serial(op, ref tag, state);
            }

            /// <summary>
            /// 1番のアルゴリズムを定義します。
            /// </summary>
            /// <param name="op">オペレーターのリスト</param>
            /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
            /// <param name="state">合成状態の情報</param>
            /// <returns>状態</returns>
            public static double OneParalellAndSerial(IList<FMOperator> op, ref object tag, FMSynthesisState state)
            {
                if (op.Count < 4) throw new InvalidOperationException("OPN系アルゴリズムではオペレータは4つ必要です");
                var fbs = state;
                fbs.Time -= 0.001;
                var ch1 = op[3].GetState(fbs);
                var ch2 = op[2].GetState(state);
                state.State = (ch1 + ch2);
                var ch3 = op[1].GetState(state);
                state.State = ch3;
                return op[0].GetState(state);
            }

            /// <summary>
            /// 2番のアルゴリズムを定義します。
            /// </summary>
            /// <param name="op">オペレーターのリスト</param>
            /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
            /// <param name="state">合成状態の情報</param>
            /// <returns>状態</returns>
            public static double TwoFeedbackAndSerial(IList<FMOperator> op, ref object tag, FMSynthesisState state)
            {
                if (op.Count < 4) throw new InvalidOperationException("OPN系アルゴリズムではオペレータは4つ必要です");
                var fbs = state;
                fbs.Time -= 0.001;

                var ch2 = op[2].GetState(state);
                state.State = ch2;
                var ch3 = op[1].GetState(state);
                var ch1 = op[3].GetState(fbs);
                state.State = (ch1 + ch3);
                return op[0].GetState(state);
            }

            /// <summary>
            /// 3番のアルゴリズムを定義します。
            /// </summary>
            /// <param name="op">オペレーターのリスト</param>
            /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
            /// <param name="state">合成状態の情報</param>
            /// <returns>状態</returns>
            public static double ThreeSingleAndSerial(IList<FMOperator> op, ref object tag, FMSynthesisState state)
            {
                if (op.Count < 4) throw new InvalidOperationException("OPN系アルゴリズムではオペレータは4つ必要です");
                var fbs = state;
                fbs.Time -= 0.001;
                var ch3 = op[1].GetState(state);
                var ch1 = op[3].GetState(fbs);
                state.State = ch1;
                var ch2 = op[2].GetState(state);
                state.State = ch2+ch3;
                state.State = (ch2 + ch3);
                return op[0].GetState(state);
            }
        }

    }
}
