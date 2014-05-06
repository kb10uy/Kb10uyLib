using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.FM
{
    /// <summary>
    /// FM音源におけるオペレータを定義します。
    /// </summary>
    public class FMOperator
    {
        #region プロパティ
        /// <summary>
        /// 使用されるオシレータを取得・設定します。
        /// </summary>
        public FMOscillatorFunction Oscillator { get; set; }

        /// <summary>
        /// 使用されるエンベロープを取得・設定します。
        /// </summary>
        public Envelope Envelope { get; set; }

        /// <summary>
        /// 変調指数、もしくは振幅を取得・設定します。
        /// </summary>
        public double ModulationIndex { get; set; }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// Operatorクラスの新しいインスタンスを初期化します。
        /// オシレータは正弦波、エンベロープはデフォルトになります。
        /// </summary>
        public FMOperator()
        {
            Oscillator = FMOscillators.Sine;
            Envelope = Envelope.Default;
            ModulationIndex = 1.0;
        }

        /// <summary>
        /// Operatorクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="osc">オシレータ</param>
        public FMOperator(FMOscillatorFunction osc)
            : this()
        {
            Oscillator = osc;
        }

        /// <summary>
        /// Operatorクラスの新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="osc">オシレータ</param>
        /// <param name="index">変調指数・振幅</param>
        public FMOperator(FMOscillatorFunction osc, double index)
            : this(osc)
        {
            ModulationIndex = index;
        }
        #endregion

        /// <summary>
        /// 現在のオペレータの情報を取得します。
        /// 固定長発音関係は取得できません。
        /// </summary>
        /// <returns>取得される情報</returns>
        public OperatorInfomation GetInfomation()
        {
            return new OperatorInfomation
            {
                Oscillator = this.Oscillator,
                Envelope = this.Envelope,
                ModulationIndex = this.ModulationIndex
            };
        }

        /// <summary>
        /// 指定された情報を、現在のオペレータに適用します。
        /// <para>nullのプロパティは反映されません。</para>
        /// </summary>
        /// <param name="info">オペレータ情報</param>
        public void SetInfomation(OperatorInfomation info)
        {
            Oscillator = info.Oscillator ?? this.Oscillator;
            Envelope = info.Envelope ?? this.Envelope;
            ModulationIndex = info.ModulationIndex ?? this.ModulationIndex;
        }

        /// <summary>
        /// ホールドし始めた時刻を0として、
        /// 指定した時刻のオペレータ出力を取得します。
        /// </summary>
        /// <param name="state">合成状態の情報</param>
        /// <returns>オペレータ出力</returns>
        public double GetState(FMSynthesisState state)
        {
            var env = GetEnvelopeState(state.Time, state.IsHolding);
            var cycle = 1.0 / state.Frequency;
            var pos = (state.Time % cycle) / cycle;
            return ModulationIndex * Oscillator(pos) * env;
        }

        /// <summary>
        /// 指定した時刻のエンベロープの状態を取得します。
        /// holdがfalseの場合、Release以降の時として計算されます。
        /// </summary>
        /// <param name="t">時刻</param>
        /// <param name="hold">発音の状態</param>
        /// <returns>エンベロープの状態</returns>
        public double GetEnvelopeState(double t, bool hold)
        {
            if (t < Envelope.Attack)
            {
                return t / Envelope.Attack;
            }
            else if (t < Envelope.Attack + Envelope.Decay)
            {
                var a = Envelope.Decay / (Envelope.Sustain - 1.0);
                var b = 1.0 - (Envelope.Attack * a);
                return t * a + b;
            }
            else
            {
                if (hold)
                {
                    return Envelope.Sustain;
                }
                else
                {
                    return t < Envelope.Release ? -(Envelope.Sustain / Envelope.Release) * t : 0.0;
                }
            }
        }
    }

    /// <summary>
    /// 一般的なエンベロープを定義します。
    /// </summary>
    public struct Envelope
    {
        /// <summary>
        /// アタック
        /// </summary>
        public double Attack;

        /// <summary>
        /// ディケイ
        /// </summary>
        public double Decay;

        /// <summary>
        /// サステイン
        /// </summary>
        public double Sustain;

        /// <summary>
        /// リリース
        /// </summary>
        public double Release;

        /// <summary>
        /// 音量変化がなく、ホールド時の音量が最大なエンベロープを取得します。
        /// </summary>
        public static Envelope Default = new Envelope { Attack = 0, Decay = 0, Sustain = 1, Release = 0 };
    }


    /// <summary>
    /// オペレータの情報のセットを定義します。
    /// </summary>
    public class OperatorInfomation
    {

        /// <summary>
        /// 変調指数を取得・設定します。
        /// </summary>
        public double? ModulationIndex { get; set; }

        /// <summary>
        /// オシレータを取得・設定します。
        /// </summary>
        public FMOscillatorFunction Oscillator { get; set; }

        /// <summary>
        /// エンベロープを取得・設定します。
        /// </summary>
        public Envelope? Envelope { get; set; }

        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        public OperatorInfomation()
        {
            ModulationIndex = null;
            Oscillator = null;
            Envelope = null;
        }
    }

}
