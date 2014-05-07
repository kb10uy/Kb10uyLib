using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kb10uy.Audio.Synthesis
{
    /// <summary>
    /// Kb10uy.Audio.Synthesis名前空間で利用できるシンセサイザーの
    /// 共通インターフェースを定義します。
    /// </summary>
    public interface ISynthesisable
    {
        /// <summary>
        /// シンセサイザーの指定時刻の状態を取得します。
        /// </summary>
        /// <param name="t">時刻</param>
        /// <returns>-1.0~1.0の状態。</returns>
        double GetState(double t);

        /// <summary>
        /// 発音を開始します。
        /// </summary>
        /// <param name="basefreq">基本周波数</param>
        void Attack(double basefreq);

        /// <summary>
        /// 発音を停止します。
        /// </summary>
        void Release();
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
