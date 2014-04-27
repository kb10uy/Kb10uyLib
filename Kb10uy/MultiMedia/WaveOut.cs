using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Kb10uy.Extension;
using System.Runtime.InteropServices;

namespace Kb10uy.MultiMedia
{
    /// <summary>
    /// WindowsのwaveOut系APIを用いた音声出力をサポートします。
    /// </summary>
    public class WaveOut
    {

        #region DllImport
        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutBreakLoop(IntPtr hwo);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutClose(IntPtr hwo);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetDevCaps(uint did, [MarshalAs(UnmanagedType.LPStruct, SizeParamIndex = 2)] ref WaveOutCaps pwoc, uint cbwoc);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetErrorText(uint err, [MarshalAs(UnmanagedType.LPStr, SizeParamIndex = 2)] StringBuilder pszt, uint ccht);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetID(IntPtr hwo, ref uint pudi);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetNumDevs();

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetPitch(IntPtr hwo, ref uint pdwp);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetPlaybackRate(IntPtr hwo, ref uint pdwr);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetPosition(IntPtr hwo, [MarshalAs(UnmanagedType.LPStruct, SizeParamIndex = 2)]ref MMTime pmmt, uint cbmmt);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutGetVolume(IntPtr hwo, ref uint pdwv);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutMessage(IntPtr hwo, uint uMsg, uint dwParam1, uint dwParam2);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutOpen(ref IntPtr phwo, uint udi, [MarshalAs(UnmanagedType.LPStruct)]ref WaveFormatEx pwfx, [MarshalAs(UnmanagedType.FunctionPtr)]WaveOutProc dwCallBack, uint dwCallbackInstance, [MarshalAs(UnmanagedType.U4)] WaveOutCallbackKind fdwOpen);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutPause(IntPtr hwo);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutPrepareHeader(IntPtr hwo, [MarshalAs(UnmanagedType.LPStruct, SizeParamIndex = 2)]ref WaveHDR pwh, uint cbwh);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutReset(IntPtr hwo);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutRestart(IntPtr hwo);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutSetPitch(IntPtr hwo, uint dwPitch);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutSetPlaybackRate(IntPtr hwo, uint dwRate);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutSetVolume(IntPtr hwo, uint dwVolume);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutUnprepareHeader(IntPtr hwo, [MarshalAs(UnmanagedType.LPStruct, SizeParamIndex = 2)]ref WaveHDR pwh, uint cbwh);

        [DllImport("winmm.dll", SetLastError = true)]
        static extern uint waveOutWrite(IntPtr hwo, [MarshalAs(UnmanagedType.LPStruct, SizeParamIndex = 2)]ref WaveHDR pwh, uint cbwh);
        #endregion

        #region readonlyプロパティ
        /// <summary>
        /// 利用できるデバイスの数を取得します。
        /// </summary>
        public static int DeviceCount
        {
            get
            {
                return (int)waveOutGetNumDevs();
            }
        }
        #endregion

        /// <summary>
        /// WaveOutのデバイスハンドルを取得します。
        /// </summary>
        public IntPtr Handle { get; private set; }

        /// <summary>
        /// 病患あたりのサンプル数を取得します。
        /// </summary>
        public uint SamplesPerSecond { get; private set; }

        /// <summary>
        /// チャンネル数を取得します。
        /// </summary>
        public ushort Channels { get; private set; }

        /// <summary>
        /// サンプルあたりのビット数を取得します。
        /// </summary>
        public ushort Bits { get; private set; }

        /// <summary>
        /// コールバック発生時に呼び出される。
        /// </summary>
        public event Action<WaveOut, uint, uint, uint, uint> OnCallback;

        /// <summary>
        /// 44.1kHz Stereo 16bitで
        /// Waveマッパーに出力する設定で、デバイスを初期化します。
        /// </summary>
        public WaveOut()
        {
            IntPtr hwo = IntPtr.Zero;
            WaveFormatEx wfx = new WaveFormatEx();
            SamplesPerSecond = 44100;
            Channels = 2;
            Bits = 16;

            wfx.wFormatTag = WaveFormatKind.PCM;
            wfx.nChannels = Channels;
            wfx.nSamplesPerSec = SamplesPerSecond;
            wfx.wBitsPerSample = Bits;
            wfx.nBlockAlign = (ushort)(Channels * Bits / 8);
            wfx.nAvgBytesPerSec = SamplesPerSecond * wfx.nBlockAlign;
            waveOutOpen(ref hwo, (uint)WaveOutDevice.WaveMapper, ref wfx, WaveOutProc, 0, WaveOutCallbackKind.Function | WaveOutCallbackKind.WaveMapped);

        }

        public void Write(WaveHDR data)
        {
            unsafe
            {
                waveOutPrepareHeader(Handle, ref data, (uint)sizeof(WaveHDR));
                waveOutWrite(Handle, ref data, (uint)sizeof(WaveHDR));
            }
        }

        private void WaveOutProc(IntPtr hwo, uint uMsg, uint dwInstance, uint dwParam1, uint dwParam2)
        {
            OnCallback(this, uMsg, dwInstance, dwParam1, dwParam2);
        }
    }

    /// <summary>
    /// WaveOutで使用するコールバック関数を定義します。
    /// </summary>
    /// <param name="hwo">WaveOut デバイス</param>
    /// <param name="uMsg">メッセージ</param>
    /// <param name="dwInstance">ユーザーデータ</param>
    /// <param name="dwParam1">パラメータ</param>
    /// <param name="dwParam2">パラメータ</param>
    public delegate void WaveOutProc(IntPtr hwo, uint uMsg, uint dwInstance, uint dwParam1, uint dwParam2);

    /// <summary>
    /// Win32 API上のWAVEOUTCAPS構造体を定義します。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WaveOutCaps
    {
        /// <summary>
        /// wMid
        /// </summary>
        public ushort wMid;

        /// <summary>
        /// wPid
        /// </summary>
        public ushort wPid;

        /// <summary>
        /// vDriverVersion
        /// </summary>
        public uint vDriverVersion;

        /// <summary>
        /// szPname
        /// </summary>
        [MarshalAs(UnmanagedType.LPStr, SizeConst = 32)]
        public char[] szPname;

        /// <summary>
        /// dwFormats
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public WaveOutDeviceFormat dwFormats;

        /// <summary>
        /// wChannels
        /// </summary>
        public ushort wCahnnels;

        /// <summary>
        /// wReserved1
        /// </summary>
        public ushort wReserved1;

        /// <summary>
        /// dwSupport
        /// </summary>
        [MarshalAs(UnmanagedType.SysUInt)]
        public WaveOutDeviceSupport dwSupport;
    }

    /// <summary>
    /// waveOutが可能なデバイスがサポートするフォーマットを定義します。
    /// </summary>
    [Flags]
    public enum WaveOutDeviceFormat : uint
    {
        /// <summary>
        /// 11.025kHz モノラル 8bit
        /// </summary>
        Format1M08 = 0x00000001,
        /// <summary>
        /// 11.025kHz ステレオ 8bit
        /// </summary>
        Format1S08 = 0x00000002,
        /// <summary>
        /// 11.025kHz モノラル 16bit
        /// </summary>
        Format1M16 = 0x00000004,
        /// <summary>
        /// 11.025kHz モノラル 16bit
        /// </summary>
        Format1S16 = 0x00000008,

        /// <summary>
        /// 22.05kHz モノラル 8bit
        /// </summary>
        Format2M08 = 0x00000010,
        /// <summary>
        /// 22.05kHz ステレオ 8bit
        /// </summary>
        Format2S08 = 0x00000020,
        /// <summary>
        /// 22.05kHz モノラル 16bit
        /// </summary>
        Format2M16 = 0x00000040,
        /// <summary>
        /// 22.05kHz ステレオ 16bit
        /// </summary>
        Format2S16 = 0x00000080,

        /// <summary>
        /// 44.1kHz モノラル 8bit
        /// </summary>
        Format4M08 = 0x00000100,
        /// <summary>
        /// 44.1kHz ステレオ 8bit
        /// </summary>
        Format4S08 = 0x00000200,
        /// <summary>
        /// 44.1kHz モノラル 16bit
        /// </summary>
        Format4M16 = 0x00000400,
        /// <summary>
        /// 44.1kHz ステレオ 16bit
        /// </summary>
        Format4S16 = 0x00000800,

        /// <summary>
        /// 44.1kHz モノラル 8bit
        /// </summary>
        Format44M08 = 0x00000100,
        /// <summary>
        /// 44.1kHz ステレオ 8bit
        /// </summary>
        Format44S08 = 0x00000200,
        /// <summary>
        /// 44.1kHz モノラル 16bit
        /// </summary>
        Format44M16 = 0x00000400,
        /// <summary>
        /// 44.1kHz ステレオ 16bit
        /// </summary>
        Format44S16 = 0x00000800,

        /// <summary>
        /// 48kHz モノラル 8bit
        /// </summary>
        Format48M08 = 0x00001000,
        /// <summary>
        /// 48kHz ステレオ 8bit
        /// </summary>
        Format48S08 = 0x00002000,
        /// <summary>
        /// 48kHz モノラル 16bit
        /// </summary>
        Format48M16 = 0x00004000,
        /// <summary>
        /// 48kHz ステレオ 16bit
        /// </summary>
        Format48S16 = 0x00008000,

        /// <summary>
        /// 96kHz モノラル 8bit
        /// </summary>
        Format96M08 = 0x00010000,
        /// <summary>
        /// 96kHz ステレオ 8bit
        /// </summary>
        Format96S08 = 0x00020000,
        /// <summary>
        /// 96kHz モノラル 16bit
        /// </summary>
        Format96M16 = 0x00040000,
        /// <summary>
        /// 96kHz ステレオ 16bit
        /// </summary>
        Format96S16 = 0x00080000,
    }

    /// <summary>
    /// waveOutが可能なデバイスがサポートする機能を定義します。
    /// </summary>
    [Flags]
    public enum WaveOutDeviceSupport : uint
    {
        /// <summary>
        /// ピッチ変更
        /// </summary>
        Pitch = 0x0001,
        /// <summary>
        /// 再生レート変更
        /// </summary>
        PlaybackRate = 0x0002,
        /// <summary>
        /// 音量変更
        /// </summary>
        Volume = 0x0004,
        /// <summary>
        /// 左右独立した音量変更
        /// </summary>
        LRVolume = 0x0008,
        /// <summary>
        /// 不明
        /// </summary>
        Sync = 0x0010,
        /// <summary>
        /// 不明
        /// </summary>
        SampleAccurate = 0x0020,
    }

    /// <summary>
    /// Windows マルチメディアAPI上の時刻を定義します。
    /// </summary>
    [StructLayout(LayoutKind.Explicit)]
    public struct MMTime
    {
        /// <summary>
        /// wType
        /// </summary>
        [FieldOffset(0)]
        public uint wType;
        /// <summary>
        /// ms
        /// </summary>
        [FieldOffset(4)]
        public uint ms;
        /// <summary>
        /// sample
        /// </summary>
        [FieldOffset(4)]
        public uint sample;
        /// <summary>
        /// cb
        /// </summary>
        [FieldOffset(4)]
        public uint cb;
        /// <summary>
        /// ticks
        /// </summary>
        [FieldOffset(4)]
        public uint ticks;
        /// <summary>
        /// hour
        /// </summary>
        [FieldOffset(4)]
        public byte smpteHour;
        /// <summary>
        /// min
        /// </summary>
        [FieldOffset(5)]
        public byte smpteMin;
        /// <summary>
        /// sec
        /// </summary>
        [FieldOffset(6)]
        public byte smpteSec;
        /// <summary>
        /// frame
        /// </summary>
        [FieldOffset(7)]
        public byte smpteFrame;
        /// <summary>
        /// frame
        /// </summary>
        [FieldOffset(8)]
        public byte smpteFps;
        /// <summary>
        /// dummy
        /// </summary>
        [FieldOffset(9)]
        public byte smpteDummy;
        /// <summary>
        /// dummy
        /// </summary>
        [FieldOffset(10)]
        public byte smptePad0;
        /// <summary>
        /// dummy
        /// </summary>
        [FieldOffset(11)]
        public byte smptePad1;
        /// <summary>
        /// songpos
        /// </summary>
        [FieldOffset(4)]
        public uint midiSongPtrPos;
    }

    /// <summary>
    /// WAVEFORMATEX構造体を定義します。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WaveFormatEx
    {
        /// <summary>
        /// wFormatTag
        /// </summary>
        public WaveFormatKind wFormatTag;
        /// <summary>
        /// nChannels
        /// </summary>
        public ushort nChannels;
        /// <summary>
        /// nSamplesPerSec
        /// </summary>
        public uint nSamplesPerSec;
        /// <summary>
        /// nAvgBytesPerSec
        /// </summary>
        public uint nAvgBytesPerSec;
        /// <summary>
        /// nBlockAlign
        /// </summary>
        public ushort nBlockAlign;
        /// <summary>
        /// wBitsPerSample
        /// </summary>
        public ushort wBitsPerSample;
        /// <summary>
        /// cbSize
        /// </summary>
        public ushort cbSize;
    }

    /// <summary>
    /// WAVE_FORMAT_PCMなどのWaveデータの種類を定義します。
    /// </summary>
    public enum WaveFormatKind : ushort
    {
        /// <summary>
        /// 不明
        /// </summary>
        Unknown = 0x0000,
        /// <summary>
        /// PCM
        /// </summary>
        PCM = 0x0001,
        /// <summary>
        /// ADPCM
        /// </summary>
        ADPCM = 0x0002,
        /// <summary>
        /// 32bit小数
        /// </summary>
        IEEEFloat = 0x0003,
        /// <summary>
        /// MPEG Layer-3
        /// </summary>
        MpegLayer3 = 0x0055,
        /// <summary>
        /// DOLBY Audio Codec 3 S/PDIF
        /// </summary>
        DolbyAC3SPDIF = 0x0092,
        /// <summary>
        /// Windows Media Audio
        /// </summary>
        WMAudio2 = 0x0161,
        /// <summary>
        /// Windows Media Audio Pro
        /// </summary>
        WMAudio3 = 0x0162,
        /// <summary>
        /// Windows Media Audio S/PDIF
        /// </summary>
        WMAudioSPDIF = 0x164,
        /// <summary>
        /// WAVEFORMATEXTENSIBLE
        /// </summary>
        Extensible = 0xFFFE
    }

    /// <summary>
    /// WaveOutのコールバックの種類を定義します。
    /// </summary>
    [Flags]
    public enum WaveOutCallbackKind : uint
    {
        /// <summary>
        /// イベント
        /// </summary>
        Event = 0x00050000,
        /// <summary>
        /// 関数(Kb10uy.Audioではこれがデフォルトです)
        /// </summary>
        Function = 0x00030000,
        /// <summary>
        /// なし(本来の既定値)
        /// </summary>
        Null = 0x00000000,
        /// <summary>
        /// スレッド
        /// </summary>
        Thread = 0x00020000,
        /// <summary>
        /// ウィンドウハンドル
        /// </summary>
        Window = 0x00010000,
        /// <summary>
        /// 不明
        /// </summary>
        AllowSync = 0x0002,
        /// <summary>
        /// 不明
        /// </summary>
        Direct = 0x0008,
        /// <summary>
        /// 実際にデバイスをオープンできるかのみ判定
        /// </summary>
        Query = 0x0001,
        /// <summary>
        /// Waveマッパー使用
        /// </summary>
        WaveMapped = 0x0004
    }

    /// <summary>
    /// WaveOut系関数で使用する特殊なデバイスIDを定義します。
    /// </summary>
    public enum WaveOutDevice : uint
    {
        /// <summary>
        /// デフォルトのWaveマッパー
        /// </summary>
        WaveMapper = 0x0000
    }

    /// <summary>
    /// WAVEHDR構造体を定義します。
    /// </summary>
    [StructLayout(LayoutKind.Sequential)]
    public struct WaveHDR
    {
        /// <summary>
        /// lpData
        /// </summary>
        //[MarshalAs(UnmanagedType.LPStr)]
        public IntPtr lpData;
        /// <summary>
        /// dwBufferLength
        /// </summary>
        public uint dwBufferLength;
        /// <summary>
        /// dwBytesRecoded
        /// </summary>
        public uint dwBytesRecoded;
        /// <summary>
        /// dwUser
        /// </summary>
        public IntPtr dwUser;
        /// <summary>
        /// dwFlags
        /// </summary>
        public uint dwFlags;
        /// <summary>
        /// dwLoops
        /// </summary>
        public uint dwLoops;
        /// <summary>
        /// lpNext
        /// </summary>
        public IntPtr lpNext;
        /// <summary>
        /// reserved
        /// </summary>
        public IntPtr reserved;

        /// <summary>
        /// 16bitのデータを書き込みます。
        /// </summary>
        /// <param name="data"></param>
        public void Write16(short[] data)
        {
            unsafe
            {
                int size = sizeof(short) * data.Length;
                var ahg = Marshal.AllocHGlobal(size);
                Marshal.Copy(data, 0, ahg, data.Length);
                dwBufferLength = (uint)size;
                lpData=ahg;
            }
        }
    }

}
