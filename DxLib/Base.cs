using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DxLibDLL;
using Kb10uy.Scripting;
using System.IO;

namespace Kb10uy.DxLib
{
    /// <summary>
    /// 自分の意思で消滅するためのインターフェース
    /// </summary>
    public interface ISelfDisappearable
    {
        /// <summary>
        /// 消滅した時に呼ばれる
        /// </summary>
        /// <returns>☆未定義☆</returns>
        bool HasDisappeared();
    }

    /// <summary>
    /// 画面上に表示される全てのオブジェクトが継承する。
    /// </summary>
    public class DisplayObjectBase
    {
        /// <summary>
        /// X位置
        /// </summary>
        public double X { get; set; }

        /// <summary>
        /// Y位置
        /// </summary>
        public double Y { get; set; }

        /// <summary>
        /// 新しいインスタンスを生成できるけどしないでね
        /// </summary>
        public DisplayObjectBase()
        {
            X = 0;
            Y = 0;
        }

        /// <summary>
        /// 描画。しない。
        /// </summary>
        public virtual void Draw()
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 255);
        }
    }

    /// <summary>
    /// 画面に表示されるものの全ての基底クラス
    /// </summary>
    public class GraphicObject : DisplayObjectBase, ICoroutine
    {
        /// <summary>
        /// グラフィックハンドル
        /// </summary>
        public int GraphicHandle { get; set; }

        /// <summary>
        /// コルーチンを確保する。
        /// </summary>
        IEnumerator<bool> cr;

        /// <summary>
        /// 新しいGraphicObjectのインスタンスは生成できるけど
        /// ほとんど死にコンストラクタ
        /// </summary>
        /// <param name="file">ファイル名</param>
        public GraphicObject(string file)
            : this()
        {
            GraphicHandle = DX.LoadGraph(file);
        }

        /// <summary>
        /// 新しいGraphicObjectのインスタンスは生成できるけど
        /// ほとんど死にコンストラクタ
        /// </summary>
        public GraphicObject()
            : base()
        {
            cr = RunCoroutine();
        }

        /// <summary>
        /// 描画する。ただし最低限。
        /// </summary>
        public override void Draw()
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 255);
            DX.DrawGraph((int)X, (int)Y, GraphicHandle, DX.FALSE);
        }

        /// <summary>
        /// コルーチンを生成。
        /// </summary>
        /// <returns>コルーチンの結果</returns>
        public virtual IEnumerator<bool> RunCoroutine()
        {
            yield return true;
        }

        /// <summary>
        /// コルーチンを取得
        /// </summary>
        public IEnumerator<bool> Coroutine
        {
            get { return cr; }
            protected set { cr = value; }
        }
    }


    /// <summary>
    /// 文字列を表示するオブジェクト
    /// </summary>
    public class StringObject : DisplayObjectBase
    {
        /// <summary>
        /// 表示される文字列
        /// </summary>
        public string Text { get; set; }

        /// <summary>
        /// 色
        /// </summary>
        public int Color { get; set; }

        /// <summary>
        /// 透明度。1.0で完璧に見える。
        /// </summary>
        public double Transparency { get; set; }

        /// <summary>
        /// 特に何もなく白で初期化
        /// </summary>
        public StringObject()
            : base()
        {
            Text = "";
            Color = DX.GetColor(255, 255, 255);
            Transparency = 1.0;
        }

        /// <summary>
        /// 文字列を指定して初期化
        /// </summary>
        /// <param name="str">文字列</param>
        public StringObject(string str)
            : this()
        {
            Text = str;
        }

        /// <summary>
        /// 文字列と位置を指定して初期化
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="x">X位置</param>
        /// <param name="y">Y位置</param>
        public StringObject(string str, double x, double y)
            : this(str)
        {
            X = x;
            Y = y;
        }

        /// <summary>
        /// 要素をほぼすべて指定して初期化
        /// </summary>
        /// <param name="str">文字列</param>
        /// <param name="x">X</param>
        /// <param name="y">Y</param>
        /// <param name="col">色</param>
        public StringObject(string str, double x, double y, int col)
            : this(str, x, y)
        {
            Color = col;
        }

        /// <summary>
        /// 描画する。なお、サイズなどは固定で、透明度のみ。
        /// </summary>
        public override void Draw()
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, (int)(Transparency * 255));
            DX.DrawString((int)X, (int)Y, Text, Color);
        }
    }

    /// <summary>
    /// 16*16に整列された文字画像を使った文字列描画。
    /// </summary>
    public class GraphicString : StringObject
    {
        /// <summary>
        /// 文字ごとのハンドル
        /// </summary>
        public IList<int> CharacterHandles { get; set; }

        /// <summary>
        /// 文字幅
        /// </summary>
        public double CharacterWidth { get; set; }

        /// <summary>
        /// 文字高さ
        /// </summary>
        public double CharacterHeight { get; set; }

        /// <summary>
        /// 合計幅
        /// </summary>
        public double Width
        {
            get
            {
                return Text.Length * CharacterWidth;
            }
        }

        /// <summary>
        /// 高さ
        /// </summary>
        public double Height
        {
            get
            {
                return CharacterHeight;
            }
        }

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="h">元画像ファイル名</param>
        /// <param name="x">1文字X</param>
        /// <param name="y">1文字Y</param>
        public GraphicString(string h, int x, int y)
        {
            CharacterWidth = x;
            CharacterHeight = y;
            var t = new int[256];
            DX.LoadDivGraph(h, 256, 16, 16, x, y, out t[0]);
            CharacterHandles = t;
            Transparency = 1.0;
        }

        /// <summary>
        /// gfファイル用
        /// </summary>
        /// <param name="h">gfパス</param>
        public GraphicString(string h)
        {
            Config cf = new Config();
            cf.LoadFile(h);
            CharacterWidth = (double)cf["Font#CharacterSize"][0];
            CharacterHeight = (double)cf["Font#CharacterSize"][1];
            var t = new int[256];
            var fp = Path.GetDirectoryName(Path.GetFullPath(h));
            DX.LoadDivGraph(Path.Combine(fp, cf["Font#File"].StringValue), 256, 16, 16, (int)CharacterWidth, (int)CharacterHeight, out t[0]);
            CharacterHandles = t;
            Transparency = 1.0;
        }

        /// <summary>
        /// 描画します。
        /// </summary>
        public override void Draw()
        {
            DX.SetDrawBright((Color >> 16) & 255, (Color >> 8) & 255, Color & 255);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, (int)(Transparency * 255));
            for (int i = 0; i < Text.Length; i++)
            {
                DX.DrawGraph((int)(X + CharacterWidth * i), (int)Y, CharacterHandles[(int)Text[i]], DX.TRUE);
            }
            DX.SetDrawBright(255, 255, 255);
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, 255);
        }

    }



    /// <summary>
    /// 任意の領域を定義する。
    /// </summary>
    public struct Region
    {
        /// <summary>
        /// 左上X
        /// </summary>
        public double X;

        /// <summary>
        /// 左上Y
        /// </summary>
        public double Y;

        /// <summary>
        /// 幅
        /// </summary>
        public double Width;

        /// <summary>
        /// 高さ
        /// </summary>
        public double Height;

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="x">左上X</param>
        /// <param name="y">左上Y</param>
        /// <param name="w">幅</param>
        /// <param name="h">高さ</param>
        public Region(double x, double y, double w, double h)
        {
            X = x;
            Y = y;
            Width = w;
            Height = h;
        }
    }

    /// <summary>
    /// コルーチンを表すインターフェース。
    /// 実際にはGraphicObjectで一応実装されている。
    /// 継承する時は、とりあえずRunCoeoutineだけオバラすればいい。
    /// Spriteにおいては、TickとCoroutineは一応どうにかする。
    /// </summary>
    public interface ICoroutine
    {
        /// <summary>
        /// コルーチンの実体の関数。
        /// 続く間は、yield return trueする。
        /// </summary>
        /// <returns></returns>
        IEnumerator<bool> RunCoroutine();

        /// <summary>
        /// 一応格納しておくやつ
        /// </summary>
        IEnumerator<bool> Coroutine { get; }
    }
}
