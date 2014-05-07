using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using Kb10uy.IO;
using Kb10uy.Scripting;
using Kb10uy.Extension;
using Kb10uy.MultiMedia;
using Kb10uy.Scripting.Text;
using Kb10uy.Audio.Synthesis;
using Kb10uy.Audio.Synthesis.FM;

namespace Kb10uyCUISample
{
    class Program
    {
        static void Main(string[] args)
        {
            //HSPとかで簡単に描画できるように
            //1/640秒ごとに計算して出力します
            var synth = new FMSynthesiser(new FMOperatorInfomation[] 
            {
                new FMOperatorInfomation{Envelope=Envelope.Default,Oscillator=FMOscillators.Sine},
                new FMOperatorInfomation{Envelope=Envelope.Default,Oscillator=FMOscillators.Sine,Detune=0.75,ModulationIndex=2},
            }, FMAlgorithms.PairMixAlgorithm);
            var str = "";
            var freq = 4.0;
            str += String.Format("キャリア detune : {0} 基本周波数 : {1}Hz", synth.Operators[0].Detune, freq) + Environment.NewLine;
            str += String.Format("モジュレータ detune : {0} 変調指数 : {1}", synth.Operators[1].Detune, synth.Operators[1].ModulationIndex) + Environment.NewLine;
            synth.Attack(freq);
            for (int i = 0; i < 640; i++)
            {
                var pos = i / 640.0;
                var st = synth.GetState(pos);
                str += st.ToString() + Environment.NewLine;
            }
            File.WriteAllText("fm.txt", str, Encoding.GetEncoding("Shift-JIS"));
        }
    }
}
