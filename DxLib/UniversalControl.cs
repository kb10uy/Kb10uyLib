using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using Kb10uy.Input;

namespace Kb10uy.DxLib
{
    /// <summary>
    /// あらゆるデバイスから入力を取るためのクラス。
    /// </summary>
    public class UniversalControl
    {

        internal XInput XInputInstance;
        private UniversalControlType t;
        private byte n;

        static UniversalControl()
        {
            StateGetter = DefaultStateGetter.Getter;
        }

        /// <summary>
        /// 具体的に取得する関数を指定します。
        /// </summary>
        public static UniversalStateGetter StateGetter { get; set; }

        /// <summary>
        /// 現在の取得方法。
        /// </summary>
        public UniversalControlType InputType
        {
            get
            {
                return t;
            }
            set
            {
                t = value;
                if (t == UniversalControlType.XInputGamepad)
                {
                    XInputInstance = new XInput(Number);
                }
            }
        }

        /// <summary>
        /// 取得先が何番目か
        /// </summary>
        public byte Number
        {
            get
            {
                return n;
            }
            set
            {
                n = value;
                XInputInstance = new XInput(n);
            }
        }

        /// <summary>
        /// 新しいコントローラを割り当てて、初期化します。
        /// </summary>
        /// <param name="type"></param>
        /// <param name="num"></param>
        public UniversalControl(UniversalControlType type, byte num)
        {
            InputType = type;
            Number = num;

            RemappedA = DX.PAD_INPUT_1;
            RemappedB = DX.PAD_INPUT_2;
            RemappedX = DX.PAD_INPUT_3;
            RemappedY = DX.PAD_INPUT_4;
            RemappedL = DX.PAD_INPUT_5;
            RemappedR = DX.PAD_INPUT_6;
            RemappedUp = DX.PAD_INPUT_UP;
            RemappedDown = DX.PAD_INPUT_DOWN;
            RemappedLeft = DX.PAD_INPUT_LEFT;
            RemappedRight = DX.PAD_INPUT_RIGHT;
            RemappedStart = DX.PAD_INPUT_7;
            RemappedBack = DX.PAD_INPUT_8;
        }

        /// <summary>
        /// 最新の情報に更新します。
        /// </summary>
        /// <returns></returns>
        public UniversalControlState GetState()
        {
            return StateGetter(this, InputType, Number);
        }

        /// <summary>
        /// 配置変更されているとするA
        /// </summary>
        public int RemappedA { get; set; }

        /// <summary>
        /// 配置変更されているとするB
        /// </summary>
        public int RemappedB { get; set; }

        /// <summary>
        /// 配置変更されているとするX
        /// </summary>
        public int RemappedX { get; set; }

        /// <summary>
        /// 配置変更されているとするY
        /// </summary>
        public int RemappedY { get; set; }

        /// <summary>
        /// 配置変更されているとするL
        /// </summary>
        public int RemappedL { get; set; }

        /// <summary>
        /// 配置変更されているとするR
        /// </summary>
        public int RemappedR { get; set; }

        /// <summary>
        /// 配置変更されているとするUp
        /// </summary>
        public int RemappedUp { get; set; }

        /// <summary>
        /// 配置変更されているとするDown
        /// </summary>
        public int RemappedDown { get; set; }

        /// <summary>
        /// 配置変更されているとするLeft
        /// </summary>
        public int RemappedLeft { get; set; }

        /// <summary>
        /// 配置変更されているとするRight
        /// </summary>
        public int RemappedRight { get; set; }

        /// <summary>
        /// 配置変更されているとするStart
        /// </summary>
        public int RemappedStart { get; set; }

        /// <summary>
        /// 配置変更されているとするBack
        /// </summary>
        public int RemappedBack { get; set; }

    }

    /// <summary>
    /// 汎用コントローラーの状態
    /// </summary>
    public struct UniversalControlState
    {
        /// <summary>
        /// Aボタン
        /// </summary>
        public bool A;

        /// <summary>
        /// Bボタン
        /// </summary>
        public bool B;

        /// <summary>
        /// Xボタン
        /// </summary>
        public bool X;

        /// <summary>
        /// Yボタン
        /// </summary>
        public bool Y;

        /// <summary>
        /// Lボタン
        /// </summary>
        public bool L;

        /// <summary>
        /// Rボタン
        /// </summary>
        public bool R;

        /// <summary>
        /// 上
        /// </summary>
        public bool Up;

        /// <summary>
        /// 下
        /// </summary>
        public bool Down;

        /// <summary>
        /// 左
        /// </summary>
        public bool Left;

        /// <summary>
        /// 右
        /// </summary>
        public bool Right;

        /// <summary>
        /// スタートボタン
        /// </summary>
        public bool Start;

        /// <summary>
        /// バックボタン
        /// </summary>
        public bool Back;
    }

    /// <summary>
    /// 汎用コントローラの状態を取得する関数用のデリゲート
    /// </summary>
    /// <param name="c">汎用コントローラのインスタンス</param>
    /// <param name="type">タイプ</param>
    /// <param name="num">番号</param>
    /// <returns></returns>
    public delegate UniversalControlState UniversalStateGetter(UniversalControl c, UniversalControlType type, byte num);

    /// <summary>
    /// 汎用コントローラの種類を指定する。
    /// </summary>
    public enum UniversalControlType
    {
        /// <summary>
        /// キーボード
        /// </summary>
        Keyboard,

        /// <summary>
        /// DirectInputゲームパッド
        /// </summary>
        DirectInputGamepad,

        /// <summary>
        /// XInputゲームパッド
        /// </summary>
        XInputGamepad,
    }

    internal static class DefaultStateGetter
    {

        /// <summary>
        /// 特になし
        /// </summary>
        /// <param name="c">特になし</param>
        /// <param name="type">特になし</param>
        /// <param name="num">特になし</param>
        /// <returns>特になし</returns>
        public static UniversalControlState Getter(UniversalControl c, UniversalControlType type, byte num)
        {
            switch (type)
            {
                case UniversalControlType.XInputGamepad:
                    return StateXInput(c, num);
                case UniversalControlType.DirectInputGamepad:
                    return StateDirectInput(c, num);
                case UniversalControlType.Keyboard:
                    return StateKeyboard(c, num);
            }
            return new UniversalControlState();
        }

        /// <summary>
        /// 特になし
        /// </summary>
        /// <param name="c">特になし</param>
        /// <param name="num">特になし</param>
        /// <returns>特になし</returns>
        private static UniversalControlState StateXInput(UniversalControl c, byte num)
        {
            var s = c.XInputInstance.GetState().Gamepad;
            return new UniversalControlState
            {
                //最近は便利なメソッドがあるもんですね
                A = s.Buttons.HasFlag(XInputButtonKind.A),
                B = s.Buttons.HasFlag(XInputButtonKind.B),
                X = s.Buttons.HasFlag(XInputButtonKind.X),
                Y = s.Buttons.HasFlag(XInputButtonKind.Y),
                L = s.Buttons.HasFlag(XInputButtonKind.LeftShoulder),
                R = s.Buttons.HasFlag(XInputButtonKind.RightShoulder),
                Left = s.Buttons.HasFlag(XInputButtonKind.DigitalPadLeft),
                Right = s.Buttons.HasFlag(XInputButtonKind.DigitalPadRight),
                Up = s.Buttons.HasFlag(XInputButtonKind.DigitalPadUp),
                Down = s.Buttons.HasFlag(XInputButtonKind.DigitalPadDown),
                Start = s.Buttons.HasFlag(XInputButtonKind.Start),
                Back = s.Buttons.HasFlag(XInputButtonKind.Back),
            };
        }

        /// <summary>
        /// 特になし
        /// </summary>
        /// <param name="c">特になし</param>
        /// <param name="num">特になし</param>
        /// <returns>特になし</returns>
        private static UniversalControlState StateDirectInput(UniversalControl c, byte num)
        {
            var t = DX.GetJoypadInputState(num);
            return new UniversalControlState
            {
                A = (t & c.RemappedA) != 0,
                B = (t & c.RemappedB) != 0,
                X = (t & c.RemappedX) != 0,
                Y = (t & c.RemappedY) != 0,
                L = (t & c.RemappedL) != 0,
                R = (t & c.RemappedR) != 0,
                Left = (t & c.RemappedLeft) != 0,
                Right = (t & c.RemappedRight) != 0,
                Up = (t & c.RemappedUp) != 0,
                Down = (t & c.RemappedDown) != 0,
                Start = (t & c.RemappedStart) != 0,
                Back = (t & c.RemappedBack) != 0,

            };
        }

        /// <summary>
        /// 特になし
        /// </summary>
        /// <param name="c">特になし</param>
        /// <param name="num">特になし</param>
        /// <returns>特になし</returns>
        public static UniversalControlState StateKeyboard(UniversalControl c, byte num)
        {
            return new UniversalControlState();
        }

    }

    /// <summary>
    /// DXライブラリ経由のキーボード入力。
    /// </summary>
    public static class KeyboardInput
    {
        static int state;
        static int pstate;
        static int tstate;

        /// <summary>
        /// 押下を更新します。
        /// </summary>
        public static void Refresh()
        {
            state = DX.GetJoypadInputState(DX.DX_INPUT_KEY_PAD1);
            tstate = (-1-pstate) & state;
            pstate = state;
        }

        /// <summary>
        /// Aボタン(Zキー)
        /// </summary>
        public static bool A
        {
            get { return (state & DX.PAD_INPUT_1) != 0; }
        }

        /// <summary>
        /// Bボタン(Xキー)
        /// </summary>
        public static bool B
        {
            get { return (state & DX.PAD_INPUT_2) != 0; }
        }

        /// <summary>
        /// Cボタン(Cキー)
        /// </summary>
        public static bool C
        {
            get { return (state & DX.PAD_INPUT_3) != 0; }
        }

        /// <summary>
        /// Xボタン(Aキー)
        /// </summary>
        public static bool X
        {
            get { return (state & DX.PAD_INPUT_4) != 0; }
        }

        /// <summary>
        /// Yボタン(Sキー)
        /// </summary>
        public static bool Y
        {
            get { return (state & DX.PAD_INPUT_5) != 0; }
        }

        /// <summary>
        /// Zボタン(Dキー)
        /// </summary>
        public static bool Z
        {
            get { return (state & DX.PAD_INPUT_6) != 0; }
        }

        /// <summary>
        /// 上
        /// </summary>
        public static bool Up
        {
            get { return (state & DX.PAD_INPUT_UP) != 0; }
        }

        /// <summary>
        /// 下
        /// </summary>
        public static bool Down
        {
            get { return (state & DX.PAD_INPUT_DOWN) != 0; }
        }

        /// <summary>
        /// 左
        /// </summary>
        public static bool Left
        {
            get { return (state & DX.PAD_INPUT_LEFT) != 0; }
        }

        /// <summary>
        /// 右
        /// </summary>
        public static bool Right
        {
            get { return (state & DX.PAD_INPUT_RIGHT) != 0; }
        }

        /// <summary>
        /// Aボタン(Zキー)
        /// </summary>
        public static bool TriggerA
        {
            get { return (tstate & DX.PAD_INPUT_1) != 0; }
        }

        /// <summary>
        /// Bボタン(Xキー)
        /// </summary>
        public static bool TriggerB
        {
            get { return (tstate & DX.PAD_INPUT_2) != 0; }
        }

        /// <summary>
        /// Cボタン(Cキー)
        /// </summary>
        public static bool TriggerC
        {
            get { return (tstate & DX.PAD_INPUT_3) != 0; }
        }

        /// <summary>
        /// Xボタン(Aキー)
        /// </summary>
        public static bool TriggerX
        {
            get { return (tstate & DX.PAD_INPUT_4) != 0; }
        }

        /// <summary>
        /// Yボタン(Sキー)
        /// </summary>
        public static bool TriggerY
        {
            get { return (tstate & DX.PAD_INPUT_5) != 0; }
        }

        /// <summary>
        /// Zボタン(Dキー)
        /// </summary>
        public static bool TriggerZ
        {
            get { return (tstate & DX.PAD_INPUT_6) != 0; }
        }

        /// <summary>
        /// 上
        /// </summary>
        public static bool TriggerUp
        {
            get { return (tstate & DX.PAD_INPUT_UP) != 0; }
        }

        /// <summary>
        /// 下
        /// </summary>
        public static bool TriggerDown
        {
            get { return (tstate & DX.PAD_INPUT_DOWN) != 0; }
        }

        /// <summary>
        /// 左
        /// </summary>
        public static bool TriggerLeft
        {
            get { return (tstate & DX.PAD_INPUT_LEFT) != 0; }
        }

        /// <summary>
        /// 右
        /// </summary>
        public static bool TriggerRight
        {
            get { return (tstate & DX.PAD_INPUT_RIGHT) != 0; }
        }
    }
}
