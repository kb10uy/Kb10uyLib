using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.Synthesis.FM
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

        /// <summary>
        /// 発音時の周波数に対して実際に生成する周波数の比率(デチューン)を取得・設定します。
        /// </summary>
        public double Detune { get; set; }
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
            Detune = 1.0;
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
        /// <param name="info">オペレータの情報</param>
        public FMOperator(FMOperatorInfomation info)
            : this()
        {
            SetInfomation(info);
        }
        #endregion

        /// <summary>
        /// 現在のオペレータの情報を取得します。
        /// 固定長発音関係は取得できません。
        /// </summary>
        /// <returns>取得される情報</returns>
        public FMOperatorInfomation GetInfomation()
        {
            return new FMOperatorInfomation
            {
                Oscillator = this.Oscillator,
                Envelope = this.Envelope,
                ModulationIndex = this.ModulationIndex,
                Detune = this.Detune,
            };
        }

        /// <summary>
        /// 指定された情報を、現在のオペレータに適用します。
        /// <para>nullのプロパティは反映されません。</para>
        /// </summary>
        /// <param name="info">オペレータ情報</param>
        public void SetInfomation(FMOperatorInfomation info)
        {
            Oscillator = info.Oscillator ?? this.Oscillator;
            Envelope = info.Envelope ?? this.Envelope;
            ModulationIndex = info.ModulationIndex ?? this.ModulationIndex;
            Detune = info.Detune ?? this.Detune;
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
            var myfr = state.Frequency * Detune;
            var ctime = 1.0 / myfr;
            var pos = ((state.Time) % ctime) / ctime;
            return ModulationIndex * Oscillator(pos, state.State) * env;
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
            if (hold)
            {
                if (t < Envelope.Attack)
                {
                    return t / Envelope.Attack;
                }
                else if (t < Envelope.Attack + Envelope.Decay)
                {
                    var d = 1.0 - Envelope.Sustain;
                    return 1.0 - ((t - Envelope.Attack) / Envelope.Decay * d);
                }
                else
                {
                    return Envelope.Sustain;
                }
            }
            else
            {
                return t < Envelope.Release ? Envelope.Sustain - (Envelope.Sustain / Envelope.Release) * t : 0.0;
            }
        }
    }


    /// <summary>
    /// オペレータの情報のセットを定義します。
    /// </summary>
    public class FMOperatorInfomation
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
        /// デチューンを取得・設定します。
        /// </summary>
        public double? Detune { get; set; }

        /// <summary>
        /// クラスの新しいインスタンスを初期化します。
        /// </summary>
        public FMOperatorInfomation()
        {
            ModulationIndex = null;
            Oscillator = null;
            Envelope = null;
            Detune = null;
        }
    }

}
