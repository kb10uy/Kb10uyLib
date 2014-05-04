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
    public class Operator
    {
        #region プロパティ
        /// <summary>
        /// 使用されるオシレータを取得・設定します。
        /// </summary>
        public OscillatorFunction Oscillator { get; set; }

        /// <summary>
        /// 使用されるエンベロープを取得・設定します。
        /// </summary>
        public Envelope Envelope { get; set; }

        /// <summary>
        /// <para>
        /// ホールドされる長さを一定にしたい場合、trueにします。
        /// この時、HoldingLengthプロパティも設定する必要があります。
        /// </para>
        /// <para>
        /// 逆に、ホールドされる長さが不定で、
        /// Attack・Releaseメソッドで制御する場合は
        /// falseにします。
        /// </para>
        /// </summary>
        public bool IsConstantHolding { get; set; }

        /// <summary>
        /// IsConstantHoldingプロパティがtrueの場合、
        /// ホールドされる長さ(ASD間の長さ)を取得・設定します。
        /// </summary>
        /// <remarks>
        /// IsConstantHoldingプロパティがfalseの場合、
        /// このプロパティは参照されません。
        /// </remarks>
        public double HoldingLength { get; set; }

        /// <summary>
        /// 現在のオシレータの発振周波数を取得・設定します。
        /// </summary>
        public double Frequency { get; set; }

        /// <summary>
        /// 現在発音されているかを取得します。
        /// </summary>
        public bool IsHolding { get; protected set; }
        #endregion

        #region コンストラクタ
        /// <summary>
        /// Operatorクラスの新しいインスタンスを初期化します。
        /// オシレータは正弦波、エンベロープはデフォルトになります。
        /// </summary>
        public Operator()
        {
            Oscillator = Oscillators.Sine;
            Envelope = Envelope.Default;
            IsConstantHolding = false;
            HoldingLength = 0;
        }

        /// <summary>
        /// Operatorクラスの新しいインスタンスを初期化します。
        /// オシレータは正弦波、エンベロープはデフォルトになります。
        /// </summary>
        public Operator(OscillatorFunction osc,double freq)
        {
            Oscillator = Oscillators.Sine;
            Envelope = Envelope.Default;
            IsConstantHolding = false;
            HoldingLength = 0;
        }
        #endregion

        /// <summary>
        /// 発音可能な状態に移行します。
        /// </summary>
        public void Attack()
        {
            if (IsConstantHolding) throw new InvalidOperationException("IsConstanrtHoldingプロパティがtrueの状態でAttackメソッドが呼び出されました");
            IsHolding = true;
        }

        /// <summary>
        /// 発音可能な状態に移行します。
        /// </summary>
        /// <param name="freq">周波数</param>
        public void Attack(double freq)
        {
            if (IsConstantHolding) throw new InvalidOperationException("IsConstanrtHoldingプロパティがtrueの状態でAttackメソッドが呼び出されました");
            Frequency = freq;
            IsHolding = true;
        }

        /// <summary>
        /// 発音を停止し、エンベロープのリリース状態に移行します。
        /// </summary>
        /// <returns>
        /// リリースエンベロープの時間の基準値。
        /// 実際には、EnvelopeプロパティののAttackとDecayを加算した値。
        /// <para>
        /// 例えば0.5秒後のリリースの状態を取得したい場合、
        /// このメソッドの返り値に0.5を加えた値をGet(Envelope)Stateメソッドに渡します。
        /// </para>
        /// </returns>
        public double Release()
        {
            if (IsConstantHolding) throw new InvalidOperationException("IsConstanrtHoldingプロパティがtrueの状態でReleaseメソッドが呼び出されました");
            IsHolding = false;
            return Envelope.Attack + Envelope.Decay;
        }

        /// <summary>
        /// 指定した時刻のオペレータ出力を取得します。
        /// </summary>
        /// <param name="t">時刻</param>
        /// <returns>オペレータ出力</returns>
        public double GetState(double t)
        {
            var env = GetEnvelopeState(t);
            var cycle = 1.0 / Frequency;
            var pos = (t % cycle) / cycle;
            return Oscillator(pos) * env;
        }

        /// <summary>
        /// 指定した時刻のエンベロープの状態を取得します。
        /// </summary>
        /// <param name="t">時刻</param>
        /// <returns>エンベロープの状態</returns>
        public double GetEnvelopeState(double t)
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
                if (IsConstantHolding)
                {
                    if (t < HoldingLength)
                    {
                        return Envelope.Sustain;
                    }
                    else
                    {
                        var rt = t - HoldingLength;
                        return rt < Envelope.Release ? -(Envelope.Sustain / Envelope.Release) * rt : 0.0;
                    }
                }
                else
                {
                    if (IsHolding)
                    {
                        return Envelope.Sustain;
                    }
                    else
                    {
                        var rt = t - (Envelope.Attack + Envelope.Decay);
                        return rt < Envelope.Release ? -(Envelope.Sustain / Envelope.Release) * rt : 0.0;
                    }
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

}
