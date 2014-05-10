using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Kb10uy.Extension;
using Kb10uy.Audio.Synthesis;
using Kb10uy.Audio.Synthesis.FM;
using NAudio;
using NAudio.Wave;
using NAudio.Wave.WaveFormats;
using NAudio.Wave.Asio;
using NAudio.Midi;
using NAudio.Mixer;
using NAudio.CoreAudioApi;
using SysTimer = System.Timers.Timer;

namespace FMMidi
{
    public partial class Form1 : Form
    {
        WaveFormat format;
        BufferedWaveProvider provider;
        /*IWavePlayer*/
        DirectSoundOut player;
        MidiIn min;
        FMSynthesiser[] synth;
        NoteEvent[] atk;
        SysTimer TimerStreaming;
        bool connected = false;

        int mch = 0, mnote = 0;
        int sri = 0;
        int chs = 32;
        double[] time, vel;

        readonly double freqmul = Math.Pow(2, 1.0 / 12.0);

        public Form1()
        {
            InitializeComponent();
            TimerStreaming = new SysTimer(5);
            TimerStreaming.Elapsed += TimerStreaming_Tick;
            format = new WaveFormat(44100, 1);

            Envelope menv = new Envelope(0, 0.5, 0, 0);
            Envelope m2env = new Envelope(0, 0.5, 0, 0);
            FMOperator car = new FMOperator(new FMOperatorInfomation { Oscillator = FMOscillators.Sine, Envelope = menv });
            FMOperator mod = new FMOperator(new FMOperatorInfomation { Oscillator = FMOscillators.Sine, Envelope = menv, ModulationIndex = 5 });

            atk = new NoteEvent[chs];
            synth = new FMSynthesiser[chs];
            time = new double[chs];
            vel = new double[chs];
            for (int i = 0; i < chs; i++)
            {
                synth[i] = new FMSynthesiser(new[] { car.GetInfomation(), mod.GetInfomation() }, FMAlgorithms.SerialModulationAlgorithm);
                //synth[i].Operators.Add(car);
                //synth[i].Operators.Add(mod);
                time[i] = 0.0;
            }
        }

        private void SetPlayer()
        {
            /*
            switch (ComboBoxAPISelection.SelectedIndex)
            {
                case 0:
                    var wo = new WaveOut();
                    wo.DeviceNumber = ComboBoxOutputSelection.SelectedIndex;
                    wo.DesiredLatency = 100;
                    player = wo;
                    break;
                case 1:
                    var dso = new DirectSoundOut();
                    player = dso;
                    break;
                case 2:
                    var waso = new WasapiOut(AudioClientShareMode.Exclusive, 200);
                    player = waso;
                    break;
                case 3:
                    var ao = new AsioOut();
                    player = ao;
                    break;
                default:
                    break;
            }
             * */
            //ストリーミング品質的にDirectSoundで
            player = new DirectSoundOut();
            min = new MidiIn(ComboBoxMidiInSelection.SelectedIndex);
            min.MessageReceived += min_MessageReceived;
            min.Start();
            TimerStreaming.Start();
            provider = new BufferedWaveProvider(format);
            player.Init(provider);
            player.Play();
            connected = true;
        }

        void min_MessageReceived(object sender, MidiInMessageEventArgs e)
        {
            var mes = e.MidiEvent;
            if (mes.CommandCode == MidiCommandCode.NoteOn)
            {
                var non = mes as NoteEvent;
                if (non.Velocity == 0 && non.Channel == mch && non.NoteNumber == mnote)
                {
                    ReleaseFM(non);
                    return;
                }

                AttackFM(non);
            }
            else if (mes.CommandCode == MidiCommandCode.NoteOff)
            {
                ReleaseFM(mes as NoteEvent);
            }
        }

        void AttackFM(NoteEvent non)
        {
            Task.Factory.StartNew(() =>
            {
                if (non.Velocity == 0)
                {
                    ReleaseFM(non);
                    return;
                }
                var reln = non.NoteNumber - 69;
                var fmul = Math.Pow(freqmul, reln);
                var s = 0;
                for (int i = 0; i < chs; i++)
                {
                    if (atk[i] == null)
                    {
                        s = i;
                        break;
                    }
                }
                atk[s] = non;
                synth[s].Attack(440.0 * fmul);
                vel[s] = (double)non.Velocity / 128;
                time[s] = 0;
            });

        }

        void ReleaseFM(NoteEvent non)
        {
            Parallel.For(0, chs, (i) =>
            {
                if (atk[i] == null) return;
                if (non.Channel == atk[i].Channel && non.NoteNumber == atk[i].NoteNumber)
                {
                    atk[i] = null;
                    time[i] = 0;
                    vel[i] = 0;
                    synth[i].Release();
                    return;
                }
            });
        }

        void ReleasePlayer()
        {
            player.Stop();
            player.Dispose();
            min.Dispose();
            TimerStreaming.Stop();
            connected = false;
            sri = 0;
        }

        private void TimerStreaming_Tick(object sender, EventArgs e)
        {
            int smp = 512;
            var pos = player.GetPosition() / (smp * 2);
            while (pos > sri)
            {
                sri++;
                var buf = new byte[smp * 2];
                Parallel.For(0, smp, (i) =>
                {
                    var st = 0.0;
                    Parallel.For(0, chs, (j) =>
                    {
                        if (atk[j] == null) return;
                        lock (buf)
                        {
                            st += synth[j].GetState(time[j] + (i / (double)smp) * (smp / 44100.0)) * vel[j];
                        }
                    });
                    st /= chs;

                    var s = BitConverter.GetBytes((short)(st * 32768));
                    buf[i * 2] = s[0];
                    buf[i * 2 + 1] = s[1];
                });
                provider.AddSamples(buf, 0, smp * 2);
                for (int i = 0; i < chs; i++)
                {
                    if (atk[i] == null) continue;
                    time[i] += smp / 44100.0;
                }
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {

            for (int i = 0; i < WaveOut.DeviceCount; i++)
            {
                var cap = WaveOut.GetCapabilities(i);
                ComboBoxOutputSelection.Items.Add(cap.ProductName);
            }
            ComboBoxOutputSelection.SelectedIndex = 0;

            ComboBoxAPISelection.Enabled = false;

            for (int i = 0; i < MidiIn.NumberOfDevices; i++)
            {
                var info = MidiIn.DeviceInfo(i);
                ComboBoxMidiInSelection.Items.Add(info.ProductName);
            }
            ComboBoxMidiInSelection.SelectedIndex = 0;
        }

        private void ComboBoxAPISelection_SelectedIndexChanged(object sender, EventArgs e)
        {
            ComboBoxOutputSelection.Enabled = ComboBoxAPISelection.SelectedIndex != 0 ? false : true;
        }

        private void ButtonConnect_Click(object sender, EventArgs e)
        {
            if (connected)
            {
                ReleasePlayer();
                ButtonConnect.Text = "接続";
            }
            else
            {
                SetPlayer();
                ButtonConnect.Text = "解放";
            }
        }
    }
}
