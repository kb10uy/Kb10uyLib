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
    /// 既存のゲームライブラリのスプライトのような機能を提供します。
    /// </summary>
    public class Sprite : GraphicObject
    {

        /// <summary>
        /// 終了したかどうか
        /// </summary>
        public bool HasFinished { get; set; }

        /// <summary>
        /// X中心座標
        /// </summary>
        public double HomeX { get; set; }

        /// <summary>
        /// Y中心座標
        /// </summary>
        public double HomeY { get; set; }

        /// <summary>
        /// X拡大率
        /// </summary>
        public double ScaleX { get; set; }

        /// <summary>
        /// Y拡大率
        /// </summary>
        public double ScaleY { get; set; }

        /// <summary>
        /// 透明度(1.0で完全描画)
        /// </summary>
        public double Transparency { get; set; }

        /// <summary>
        /// 角度
        /// </summary>
        public double Angle { get; set; }

        /// <summary>
        /// Spriteの新しいインスタンスを生成。
        /// </summary>
        public Sprite()
            : base()
        {
            HomeX = 0;
            HomeY = 0;
            ScaleX = 1.0;
            ScaleY = 1.0;
            Transparency = 1.0;
            Angle = 0.0;
            HasFinished = false;

        }

        /// <summary>
        /// 1フレームごとの処理
        /// </summary>
        /// <returns>
        /// 成功フラグ。falseを返すと消滅する(様に実装する！)。コルーチンを使いたければ、
        /// return Coroutine.MoveNext() &amp;&amp; Coroutine.Current;するだけでいいと思う。
        /// 2014/02/01 foreachに怒られるのでHasFinishedで返すようにする。
        /// </returns>
        public virtual bool Tick()
        {
            return true;
        }

        /// <summary>
        /// 描画する。回転・透明度・拡大込みの標準的な描画。
        /// </summary>
        public override void Draw()
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, (int)(Transparency * 255));
            DX.DrawRotaGraph3((int)X, (int)Y, (int)HomeX, (int)HomeY, ScaleX, ScaleY, Angle, GraphicHandle, DX.TRUE);
        }

    }

    /// <summary>
    /// 任意のオブジェクトをタグとして持てるSpriteです。
    /// </summary>
    /// <typeparam name="TTag">タグの型</typeparam>
    public class Sprite<TTag> : Sprite
    {
        /// <summary>
        /// 任意のオブジェクト
        /// </summary>
        public TTag Tag { get; set; }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public Sprite()
            : base()
        {
        }


        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="tag">タグ</param>
        public Sprite(TTag tag)
            : this()
        {
            Tag = tag;
        }

    }

    /// <summary>
    /// アニメーションするスプライトを定義する。
    /// </summary>
    public class AnimationSprite : Sprite
    {
        /// <summary>
        /// アニメーションに使うハンドルを入れる。
        /// </summary>
        public IList<int> AnimationHandles { get; set; }

        /// <summary>
        /// アニメーションのフレーム数
        /// </summary>
        public int Frames { get; set; }

        /// <summary>
        /// 現在再生しているフレーム。
        /// </summary>
        public int PlayingFrame { get; set; }

        /// <summary>
        /// 幅
        /// </summary>
        protected int Width { get; private set; }

        /// <summary>
        /// 高さ
        /// </summary>
        protected int Height { get; private set; }

        /// <summary>
        /// X枚数
        /// </summary>
        protected int XCount { get; private set; }

        /// <summary>
        /// Y枚数
        /// </summary>
        protected int YCount { get; private set; }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="f">ファイル名</param>
        /// <param name="fr">フレーム数</param>
        /// <param name="xc">X枚数</param>
        /// <param name="yc">Y枚数</param>
        /// <param name="w">幅</param>
        /// <param name="h">高さ</param>
        public AnimationSprite(string f, int fr, int xc, int yc, int w, int h)
            : base()
        {
            Width = w;
            Height = h;
            XCount = xc;
            YCount = yc;
            Frames = fr;
            GraphicHandle = DX.LoadGraph(f);
            int[] gb = new int[fr];
            DX.LoadDivGraph(f, fr, xc, yc, w, h, out gb[0]);
            AnimationHandles = gb;
        }

        /// <summary>
        /// 空っぽで新しいインスタンスを初期化します。
        /// </summary>
        public AnimationSprite()
        {

        }

        /// <summary>
        /// 処理をします。
        /// </summary>
        /// <returns>(Deprecated)生存フラグ</returns>
        public override bool Tick()
        {
            PlayingFrame = (++PlayingFrame) % Frames;
            return base.Tick();
        }

        /// <summary>
        /// 描画します。
        /// </summary>
        public override void Draw()
        {
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, (int)(Transparency * 255));
            DX.DrawRotaGraph3((int)X, (int)Y, (int)HomeX, (int)HomeY, ScaleX, ScaleY, Angle, AnimationHandles[PlayingFrame], DX.TRUE);
        }

    }

    /// <summary>
    /// 任意のオブジェクトをタグとして持てるAnimationSpriteです。
    /// </summary>
    /// <typeparam name="TTag">タグの型</typeparam>
    public class AnimationSprite<TTag> : AnimationSprite
    {
        /// <summary>
        /// 任意のオブジェクト
        /// </summary>
        public TTag Tag { get; set; }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public AnimationSprite()
            : base()
        {
        }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="tag">タグ</param>
        public AnimationSprite(TTag tag)
            : this()
        {
            Tag = tag;
        }
    }

    /// <summary>
    /// 1度のみ再生されるAnimationSpriteです。
    /// </summary>
    public class EffectSprite : AnimationSprite
    {

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="f">ファイル名</param>
        /// <param name="fr">フレーム数</param>
        /// <param name="xc">X枚数</param>
        /// <param name="yc">Y枚数</param>
        /// <param name="w">幅</param>
        /// <param name="h">高さ</param>
        public EffectSprite(string f, int fr, int xc, int yc, int w, int h)
            : base(f, fr, xc, yc, w, h)
        {

        }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public EffectSprite()
        {

        }

        /// <summary>
        /// 処理します。
        /// </summary>
        /// <returns>(Deprecated)生存フラグ</returns>
        public override bool Tick()
        {
            PlayingFrame++;
            if (PlayingFrame >= Frames)
            {
                PlayingFrame = Frames - 1;
                HasFinished = true;
            }
            return true;
        }

        /// <summary>
        /// クローンを作成します。
        /// </summary>
        /// <returns>クローン</returns>
        public EffectSprite Clone()
        {
            EffectSprite ret = new EffectSprite();
            ret.Angle = Angle;
            ret.AnimationHandles = AnimationHandles;
            ret.Frames = Frames;
            ret.GraphicHandle = GraphicHandle;
            ret.HomeX = HomeX;
            ret.HomeY = HomeY;
            ret.ScaleX = ScaleX;
            ret.ScaleY = ScaleY;
            ret.Transparency = Transparency;
            ret.X = X;
            ret.Y = Y;

            return ret;
        }

    }

    /// <summary>
    /// 任意のオブジェクトをタグとして持てるEffectSpriteです。
    /// </summary>
    /// <typeparam name="TTag">タグの型</typeparam>
    public class EffectSprite<TTag> : EffectSprite
    {
        /// <summary>
        /// 任意のオブジェクト
        /// </summary>
        public TTag Tag { get; set; }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public EffectSprite()
            : base()
        {
        }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="tag">タグ</param>
        public EffectSprite(TTag tag)
            : this()
        {
            Tag = tag;
        }
    }


    /// <summary>
    /// デフォTickがコルーチン用なだけのSprite
    /// </summary>
    public class CoroutineSprite : Sprite
    {

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        public CoroutineSprite()
        {

        }

        /// <summary>
        /// 新しくインスタンスを生成。
        /// </summary>
        /// <param name="coroutine">
        /// コルーチン関数。ただしラムダ式は使用できません。
        /// </param>
        public CoroutineSprite(Func<CoroutineSprite, IEnumerator<bool>> coroutine)
            : base()
        {
            Coroutine = coroutine(this);
        }

        /// <summary>
        /// 1F処理を進める。
        /// </summary>
        /// <returns>生存フラグ</returns>
        public override bool Tick()
        {
            return Coroutine.MoveNext() && Coroutine.Current;
        }
    }

    /// <summary>
    /// 任意のオブジェクトをタグとして持てるCoroutineSpriteです。
    /// </summary>
    /// <typeparam name="TTag">タグの型</typeparam>
    public class CoroutineSprite<TTag> : CoroutineSprite
    {
        /// <summary>
        /// 任意のオブジェクト
        /// </summary>
        public TTag Tag { get; set; }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="coroutine">
        /// コルーチン関数。ただしラムダ式は使用できません。
        /// </param>
        public CoroutineSprite(Func<CoroutineSprite<TTag>, IEnumerator<bool>> coroutine)
        {
            Coroutine = coroutine(this);
        }

        /// <summary>
        /// 新しいインスタンスを初期化します。
        /// </summary>
        /// <param name="coroutine">
        /// コルーチン関数。ただしラムダ式は使用できません。
        /// </param>
        /// <param name="tag">タグ</param>
        public CoroutineSprite(Func<CoroutineSprite<TTag>, IEnumerator<bool>> coroutine,TTag tag)
            : this(coroutine)
        {
            Tag = tag;
        }
    }

    /// <summary>
    /// 9スライス的なオブジェクト。
    /// </summary>
    public class NineSlice : GraphicObject
    {
        /// <summary>
        /// 0~1の不透明度。
        /// </summary>
        public double Transparency { get; set; }

        /// <summary>
        /// 全体の幅。
        /// </summary>
        public double Width { get; set; }

        /// <summary>
        /// 全体の高さ
        /// </summary>
        public double Height { get; set; }

        /// <summary>
        /// 9スライス処理後のハンドル。
        /// </summary>
        public IList<int> SliceHandles { get; set; }

        /// <summary>
        /// 9スライスのそれぞれの範囲。
        /// </summary>
        public IList<Region> SliceRegions { get; set; }

        /// <summary>
        /// 指定のファイル名の情報を元に、
        /// 9sliceオブジェクトを作成します。
        /// </summary>
        /// <param name="path">9sliceの定義ファイル</param>
        public NineSlice(string path)
        {
            Transparency = 1.0;
            SliceHandles = new int[9];
            SliceRegions = new Region[9];

            Config cf = new Config();
            cf.LoadFile(path);
            var fp = Path.GetDirectoryName(Path.GetFullPath(path));
            GraphicHandle = DX.LoadGraph(Path.Combine(fp, cf["9Slice#File"].StringValue));
            SliceRegions[0] = new Region((double)cf["9Slice#TLSlice"][0], (double)cf["9Slice#TLSlice"][1], (double)cf["9Slice#TLSlice"][2], (double)cf["9Slice#TLSlice"][3]);
            SliceRegions[1] = new Region((double)cf["9Slice#TSlice"][0], (double)cf["9Slice#TSlice"][1], (double)cf["9Slice#TSlice"][2], (double)cf["9Slice#TSlice"][3]);
            SliceRegions[2] = new Region((double)cf["9Slice#TRSlice"][0], (double)cf["9Slice#TRSlice"][1], (double)cf["9Slice#TRSlice"][2], (double)cf["9Slice#TRSlice"][3]);
            SliceRegions[3] = new Region((double)cf["9Slice#MLSlice"][0], (double)cf["9Slice#MLSlice"][1], (double)cf["9Slice#MLSlice"][2], (double)cf["9Slice#MLSlice"][3]);
            SliceRegions[4] = new Region((double)cf["9Slice#MSlice"][0], (double)cf["9Slice#MSlice"][1], (double)cf["9Slice#MSlice"][2], (double)cf["9Slice#MSlice"][3]);
            SliceRegions[5] = new Region((double)cf["9Slice#MRSlice"][0], (double)cf["9Slice#MRSlice"][1], (double)cf["9Slice#MRSlice"][2], (double)cf["9Slice#MRSlice"][3]);
            SliceRegions[6] = new Region((double)cf["9Slice#BLSlice"][0], (double)cf["9Slice#BLSlice"][1], (double)cf["9Slice#BLSlice"][2], (double)cf["9Slice#BLSlice"][3]);
            SliceRegions[7] = new Region((double)cf["9Slice#BSlice"][0], (double)cf["9Slice#BSlice"][1], (double)cf["9Slice#BSlice"][2], (double)cf["9Slice#BSlice"][3]);
            SliceRegions[8] = new Region((double)cf["9Slice#BRSlice"][0], (double)cf["9Slice#BRSlice"][1], (double)cf["9Slice#BRSlice"][2], (double)cf["9Slice#BRSlice"][3]);
            SliceHandles[0] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[0]);
            SliceHandles[1] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[1]);
            SliceHandles[2] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[2]);
            SliceHandles[3] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[3]);
            SliceHandles[4] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[4]);
            SliceHandles[5] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[5]);
            SliceHandles[6] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[6]);
            SliceHandles[7] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[7]);
            SliceHandles[8] = ExtraDxLib.DerivationGraphFromRegion(GraphicHandle, SliceRegions[8]);
        }

        /// <summary>
        /// 描画します。
        /// </summary>
        public override void Draw()
        {
            var by = Y;
            DX.SetDrawBlendMode(DX.DX_BLENDMODE_ALPHA, (int)(Transparency * 255));

            DX.DrawGraph((int)X, (int)by, SliceHandles[0], DX.TRUE);
            var tw = Width - (SliceRegions[0].Width + SliceRegions[2].Width);
            DX.DrawExtendGraph((int)(X + SliceRegions[0].Width), (int)by, (int)(X + SliceRegions[0].Width + tw), (int)(by + SliceRegions[1].Height), SliceHandles[1], DX.TRUE);
            DX.DrawGraph((int)(X + SliceRegions[0].Width + tw), (int)by, SliceHandles[2], DX.TRUE);
            by += SliceRegions[0].Height;
            var th = Height - (SliceRegions[0].Height + SliceRegions[6].Height);
            DX.DrawExtendGraph((int)X, (int)by, (int)(X + SliceRegions[3].Width), (int)(by + th), SliceHandles[3], DX.TRUE);
            by += (SliceRegions[0].Height - SliceRegions[2].Height);
            th = Height - (SliceRegions[2].Height + SliceRegions[8].Height);
            DX.DrawExtendGraph((int)(X + Width - SliceRegions[5].Width), (int)by, (int)(X + Width), (int)(by + th), SliceHandles[5], DX.TRUE);
            by = Y + Height - SliceRegions[6].Height;
            DX.DrawGraph((int)X, (int)by, SliceHandles[6], DX.TRUE);
            tw = Width - (SliceRegions[6].Width + SliceRegions[8].Width);
            DX.DrawExtendGraph((int)(X + SliceRegions[6].Width), (int)(Y + Height - SliceRegions[7].Height), (int)(X + SliceRegions[6].Width + tw), (int)(Y + Height), SliceHandles[7], DX.TRUE);
            DX.DrawGraph((int)(X + Width - SliceRegions[8].Width), (int)(Y + Height - SliceRegions[8].Height), SliceHandles[8], DX.TRUE);
            DX.DrawExtendGraph((int)(X + SliceRegions[0].Width), (int)(Y + SliceRegions[0].Height), (int)(X + Width - SliceRegions[8].Width), (int)(Y + Height - SliceRegions[8].Height), SliceHandles[4], DX.TRUE);
        }

    }

}
