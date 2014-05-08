using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Diagnostics;
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
                new FMOperatorInfomation{Envelope=Envelope.Default,Oscillator=FMOscillators.Triangle},
                new FMOperatorInfomation{Envelope=Envelope.Default,Oscillator=FMOscillators.Sine},
            }, FMAlgorithms.PairModulationAlgorithm);
            var str = "";
            var freq = 1.0;
            synth.Attack(freq);
            for (int i = 0; i < 640; i++)
            {
                var pos = i / 640.0;
                var st = synth.GetState(pos);
                str += st.ToString() + Environment.NewLine;
            }
            File.WriteAllText("fm.txt", str, Encoding.GetEncoding("Shift-JIS"));
            Console.WriteLine("fm.txt wrote");
            Console.ReadLine();
        }
    }
}
