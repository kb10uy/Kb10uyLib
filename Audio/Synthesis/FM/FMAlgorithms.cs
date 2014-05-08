﻿using System;
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
        /*
        /// <summary>
        /// 2チャンネル1組として、
        /// 偶数チャンネルをキャリア、奇数チャンネルをモジュレータとして変調し、
        /// 全て同じ比率でミックスします。
        /// <para>オペレーター数が奇数だった場合、最後のオペレーターは省かれます。</para>
        /// </summary>
        /// <param name="op">オペレーターのリスト</param>
        /// <param name="tag">フィードバックの保持など自由に使えるタグオブジェクト</param>
        /// <param name="state">合成状態の情報</param>
        /// <returns>状態</returns>
        public static double DoubleMixAlgorithm(IList<FMOperator> op, ref object tag, FMSynthesisState state)
        {
            int mc = op.Count / 2;
            double mix = 0.0;
            for (int i = 0; i < mc; i++)
            {
                var cs = SerialModulation(state, op[i * 2], op[i * 2 + 1]);
                mix += (cs / op[i * 2].ModulationIndex);
            }
            return mix / mc;
        }
        */
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
            /// <para>常に一定時間前の状態を反映したい場合、SingleConstantFeedbackSerialAlgorithmを使用してください。</para>
            /// </summary>
            public class SingleFeedbackSerialAlgorithm
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
                public SingleFeedbackSerialAlgorithm(int fbch)
                {
                    FeedbackChannel = fbch;
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
        }

        #endregion

    }
}
