using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NLua;
using NLua.Method;


namespace Kb10uy.DxLib
{
    /// <summary>
    /// ゲーム内の各シーンを制御
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// シーンごとに固有のID。かぶると例外が発生する。
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// シーンの描画順。値が大きいほどあとから描画される。
        /// </summary>
        public int Index { get; set; }

        /// <summary>
        /// シーンの処理が終わっているかのフラグ。
        /// なんか微妙な実装方法。
        /// </summary>
        public bool HasFinished { get; set; }

        /// <summary>
        /// あたらしいシーンを生成します。
        /// なお、継承するクラスでは内部で必ずコンストラクタをオーバーライドし、
        /// IDとIndexを設定する必要があります。
        /// </summary>
        public Scene()
        {
            ID = "";
            Index = 0;
        }

        /// <summary>
        /// シーン開始時に呼び出される。
        /// </summary>
        /// <param name="prev">開始させたシーン</param>
        public virtual void Start(Scene prev)
        {
#if DEBUG
            Console.WriteLine("{0} によって {1} が開始", prev != null ? prev.ID : "", ID);
#endif
        }

        /// <summary>
        /// tick処理。1F内のぶんのみ処理するようにするので、
        /// コルーチンとかはこの下にぶら下げる。
        /// </summary>
        public virtual void Tick()
        {

        }

        /// <summary>
        /// 描画処理。
        /// </summary>
        public virtual void Draw()
        {

        }

        /// <summary>
        /// 終了時に呼ばれるメソッド
        /// </summary>
        public virtual void Exit()
        {
#if  DEBUG
            Console.WriteLine("{0} が終了", ID);
#endif
        }
    }


    /// <summary>
    /// Luaによって制御できるシーン。
    /// 呼び出す関数はある程度名前が決まってる。
    /// <para>
    /// コルーチンの扱いについては http://piorimu.blog121.fc2.com/blog-entry-96.html を参照
    /// </para>
    /// </summary>
    public class LuaScene : Scene
    {

        List<DisplayObjectBase> objs;

        /// <summary>
        /// Luaのインスタンス
        /// </summary>
        public Lua LuaScript { get; set; }

        /// <summary>
        /// オブジェクトのリスト。
        /// </summary>
        public IList<DisplayObjectBase> Children
        {
            get
            {
                return objs;
            }
            set
            {
                objs = value.ToList();
            }
        }

        /// <summary>
        /// 新しくLuaシーンを初期化。
        /// </summary>
        /// <param name="sceneName">シーン名</param>
        /// <param name="luafile">制御するLuaのファイル名</param>
        public LuaScene(string sceneName, string luafile)
            : base()
        {
            LuaScript = new Lua();
            LuaScript["Scenes"] = Scenes.Instance;
            LuaScript.DoFile(luafile);
            ID = sceneName;
        }

        /// <summary>
        /// 初期化。
        /// 呼ばれるLuaの関数は<example>Start_&lt;シーン名&gt;(&lt;所属するシーン&gt;)</example>
        /// </summary>
        public override void Start(Scene prev)
        {
            LuaScript.GetFunction("Start_" + ID).Call(this);
            base.Start(prev);
        }

        /// <summary>
        /// 終末処理。
        /// 呼ばれるLuaの関数は<example>Exit_&lt;SceneName&gt;()</example>
        /// </summary>
        public override void Exit()
        {
            LuaScript.GetFunction("Exit_" + ID).Call();
            LuaScript.Dispose();
            base.Exit();
        }

        /// <summary>
        /// 制御を1F進める。
        /// 呼ばれるLuaの関数は<example>Tick_&lt;SceneName&gt;()</example>
        /// </summary>
        public override void Tick()
        {
            LuaScript.GetFunction("Tick_" + ID).Call();
            base.Tick();
        }

        /// <summary>
        /// 描画処理。
        /// 呼ばれるLuaの関数は<example>Tick_&lt;SceneName&gt;()</example>
        /// </summary>
        public override void Draw()
        {
            LuaScript.GetFunction("Draw_" + ID).Call();
            base.Draw();
        }

        private void RegisterFunctions()
        {

        }

    }

    /// <summary>
    /// Luaからこっち側に色々するためのやつ
    /// </summary>
    public static class LuaSceneAssist
    {
        
    }

    /// <summary>
    /// アクティブなシーンを格納する。
    /// <para>
    /// 2013/12/28 14:17   
    /// Luaへのインスタンス引き渡しが必要になったので、
    /// Singletonパターンに変更。
    /// </para>
    /// </summary>
    public sealed class Scenes
    {

        static Scenes instance = new Scenes();

        HashSet<Scene> scenes;

        private Scenes()
        {
            scenes = new HashSet<Scene>();
        }

        /// <summary>
        /// Scenesの唯一のインスタンスを取得する。
        /// 外部にインスタンスしか渡せない時とか以外は
        /// 使わないようにする。
        /// </summary>
        /// <returns>Singletonなインスタンス</returns>
        public static Scenes Instance
        {
            get
            {
                return instance;
            }
        }

        /// <summary>
        /// 一括でtick処理。
        /// また、終了したやつを消しとく。
        /// </summary>
        public static void TickAll()
        {
            foreach (var p in instance.scenes.OrderBy(p => p.Index))
            {
                p.Tick();
            }
            var e = instance.scenes.Where((p) => p.HasFinished);
            foreach (var t in e)
            {
                t.Exit();
            }
            instance.scenes.RemoveWhere((p) => p.HasFinished);
        }

        /// <summary>
        /// 一括で描画処理。
        /// また、終了したやつを消しとく。
        /// </summary>
        public static void DrawAll()
        {
            foreach (var p in instance.scenes.OrderBy(p => p.Index))
            {
                p.Draw();
            }
        }

        /// <summary>
        /// 指定したIDのシーンを取得。ない場合はnull。
        /// </summary>
        /// <param name="id">ID</param>
        /// <returns>合致シーン</returns>
        public static Scene GetScene(string id)
        {
            return instance.scenes.Where(p => p.ID == id).First();
        }

        /// <summary>
        /// シーンリストに新しいシーンを追加します。
        /// </summary>
        /// <param name="item">追加するシーン</param>
        public static void Add(Scene item)
        {
            if (instance.scenes.Any(p => p.ID == item.ID))
            {
                throw new ArgumentException("指定されたIDのシーンがすでに再生中です", "item");
            }
            instance.scenes.Add(item);
            item.Start(null);
        }

        /// <summary>
        /// 呼び出し源を指定してシーンを追加
        /// </summary>
        /// <param name="item">追加するシーン</param>
        /// <param name="calp">呼び出し元</param>
        public static void Add(Scene item, Scene calp)
        {
            if (instance.scenes.Any(p => p.ID == item.ID))
            {
                throw new ArgumentException("指定されたIDのシーンがすでに再生中です", "item");
            }
            instance.scenes.Add(item);
            item.Start(calp);
        }

        /// <summary>
        /// シーンを全て削除します。
        /// </summary>
        public static void Clear()
        {
            foreach (var p in instance.scenes)
            {
                p.Exit();
            }
            instance.scenes.Clear();
        }

        /// <summary>
        /// 指定シーンが存在するか判定します。
        /// 判定基準はIDです。
        /// </summary>
        /// <param name="item">シーン</param>
        /// <returns></returns>
        public static bool Contains(Scene item)
        {
            return instance.scenes.Any(p => p.ID == item.ID);
        }

        /// <summary>
        /// 配列に内容をコピーします。
        /// </summary>
        /// <param name="array"></param>
        /// <param name="arrayIndex"></param>
        public static void CopyTo(Scene[] array, int arrayIndex)
        {
            instance.scenes.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// シーン数を返します。
        /// </summary>
        public static int Count
        {
            get { return instance.scenes.Count; }
        }

        /// <summary>
        /// 絶対にfalse
        /// </summary>
        public static bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// 指定したシーンを削除します。
        /// 同じIDのシーンが削除対象です。
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public static bool Remove(Scene item)
        {
            var e = instance.scenes.Where(p => p.ID == item.ID).First();
            if (e == null) return false;
            e.Exit();
            instance.scenes.Remove(e);
            return true;
        }

        /// <summary>
        /// IEnumerable.GetEnumeratorです。
        /// </summary>
        /// <returns></returns>
        public static IEnumerator<Scene> GetEnumerator()
        {
            return instance.scenes.GetEnumerator();
        }

        /// <summary>
        /// 現在動いている全てのシーンを返します。
        /// </summary>
        /// <returns></returns>
        public static IEnumerable<Scene> GetAllScenes()
        {
            return instance.scenes;
        }

    }

}
