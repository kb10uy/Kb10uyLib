using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony.Parsing;
using Irony;
using System.IO;
using Kb10uy.Extension;

namespace Kb10uy.Scripting
{

    internal interface IKb10uyConfigHasGroups
    {
        IDictionary<String, ConfigGroup> Groups { get; }
    }

    /// <summary>
    /// 色々できるKb10uyConfigを読み込むクラス。
    /// </summary>
    public class Config : IKb10uyConfigHasGroups
    {
        #region 非推奨な
        static Dictionary<string, ConfigValueType> typeset = new Dictionary<string, ConfigValueType>
        {
            { "Number" , ConfigValueType.Number},
            { "String" , ConfigValueType.String},
            { "Array" ,  ConfigValueType.Array },
        };
        #endregion

        Dictionary<string, ConfigGroup> groups = new Dictionary<string, ConfigGroup>();

        /// <summary>
        /// ファイルの文字列そのまま
        /// </summary>
        public string RawString { get; protected set; }

        /// <summary>
        /// 所属する全てのグループ
        /// </summary>
        public IDictionary<string, ConfigGroup> Groups
        {
            get { return groups; }
        }

        /// <summary>
        /// 解析器
        /// </summary>
        public static Kb10uyConfigGrammar Grammar { get; protected set; }

        /// <summary>
        /// パーサ
        /// </summary>
        public static Parser Parser { get; protected set; }

        /// <summary>
        /// 生ツリー
        /// </summary>
        public ParseTree RawTree { get; protected set; }

        static Config()
        {
            Grammar = new Kb10uyConfigGrammar();
            Parser = new Parser(Grammar);
        }

        /// <summary>
        /// ファイルを読み込む
        /// </summary>
        /// <param name="file">ファイル名</param>
        public void LoadFile(string file)
        {
            RawString = File.ReadAllText(file);
            Parse();
        }

        /// <summary>
        /// 文字列を読み込む
        /// </summary>
        /// <param name="str">Kb10uyConfigの文字列</param>
        public void LoadString(string str)
        {
            RawString = str;
            Parse();
        }

        private void Parse()
        {
            RawTree = Parser.Parse(RawString);
            if (RawTree.Root == null) throw new InvalidDataException("無効なKb10uyConfigです");
            var gs = RawTree.Root.ChildNodes;
            var dg = new ConfigGroup();   //形式的にダミーを作ってそれから取得
            foreach (var g in gs)
            {
                ParseGroup(ref dg, g);
            }
            groups = dg.gs;
        }

        /// <summary>
        /// ファイルに保存します。
        /// </summary>
        /// <param name="file">ファイル名</param>
        public void SaveFile(string file)
        {
            var ss = new List<string>();
            foreach (var g in Groups)
            {
                ss.AddRange(SerializeGroup(g.Value, g.Key, 0));
            }
            File.WriteAllLines(file, ss);
        }



        private IList<string> SerializeGroup(ConfigGroup gr, string name, int inl)
        {
            var ret = new List<string>();
            ret.Add("\t".Repeat(inl)
                + String.Format("\"{0}\" : ", name)
                + "{");
            foreach (var g in gr.Groups)
            {
                ret.AddRange(SerializeGroup(g.Value, g.Key, inl + 1));
            }
            foreach (var p in gr.Properties)
            {
                ret.Add(SerializeProperty(p.Value, p.Key, inl + 1));
            }
            ret.Add("\t".Repeat(inl) + "}");
            return ret;
        }

        private string SerializeProperty(ConfigValue pr, string name, int inl)
        {
            return String.Format("{3}{0} : \"{1}\" = {2};", pr.Type, name, SerializeValue(pr), "\t".Repeat(inl));
        }

        private string SerializeValue(ConfigValue val)
        {
            var ret = "";

            switch (val.Type)
            {
                case ConfigValueType.Array:
                    var ar = new List<string>();
                    foreach (var v in val.ArrayValue)
                    {
                        ar.Add(SerializeValue(v));
                    }
                    var t = "[ " + ar[0];
                    foreach (var v in ar.Skip(1))
                    {
                        t += ", " + v;
                    }
                    t += " ]";
                    return t;
                case ConfigValueType.Number:
                    return ((double)val).ToString();
                case ConfigValueType.String:
                    return "\"" + (string)val + "\"";
            }

            return ret;
        }


        /// <summary>
        /// 任意のプロパティを直接取得。
        /// Group.Group.Propertyのように指定。
        /// したがって、"."を含む名前のプロパティは
        /// 正常に取得できないおそれがある。
        /// </summary>
        /// <param name="path">プロパティフルパス</param>
        /// <returns></returns>
        public ConfigValue this[string path]
        {
            get
            {
                ConfigGroup gr;
                var pl = path.Split('.');
                if (pl.Length <= 1) throw new ArgumentException("プロパティはグループの中に入れてください");
                var pn = pl.Last();

                var fgr = pl.First();
                var sgrp = pl.Take(pl.Length - 1).Skip(1);

                gr = Groups.GetOrCreateValue(fgr);

                foreach (var i in sgrp)
                {
                    gr = gr.Groups.GetOrCreateValue(i);
                }
                return gr.Properties.GetOrCreateValue(pn);

            }

            set
            {
                ConfigGroup gr;
                var pl = path.Split('.');
                if (pl.Length <= 1) throw new ArgumentException("プロパティはグループの中に入れてください");
                var pn = pl.Last();

                var fgr = pl.First();
                var sgrp = pl.Take(pl.Length - 1).Skip(1);

                gr = Groups.GetOrCreateValue(fgr);

                foreach (var i in sgrp)
                {
                    gr = gr.Groups.GetOrCreateValue(i);
                }
                gr.Properties[pn] = value;
            }
        }

        #region パース用内部関数

        private void ParseGroup(ref ConfigGroup par, ParseTreeNode groupnode)
        {
            if (groupnode.Term.Name != "Group") throw new InvalidDataException("Groupの形式ではありません");
            var gname = groupnode.ChildNodes[0].Token.ValueString;
            var cnodes = groupnode.ChildNodes[1].ChildNodes;
            var pdg = new ConfigGroup();
            foreach (var i in cnodes)
            {
                switch (i.ChildNodes[0].Term.Name)
                {
                    case "Property":
                        ParseProperty(ref pdg, i.ChildNodes[0]);
                        break;

                    case "Group":
                        ParseGroup(ref pdg, i.ChildNodes[0]);
                        break;

                    default:
                        throw new InvalidDataException("何かがおかしい");

                }
            }
            par.Groups[gname] = pdg;
        }

        private void ParseProperty(ref ConfigGroup par, ParseTreeNode propnode)
        {
            var types = propnode.ChildNodes[0].ChildNodes[0].Token.ValueString;
            var cty = typeset[types];
            var name = propnode.ChildNodes[1].Token.ValueString;
            //値のパース
            if (propnode.ChildNodes[2].ChildNodes[0].Term.Name != types)
                throw new InvalidDataException(String.Format("型の不一致が発生しています : 宣言 {0} に対して {1} の値", types, propnode.ChildNodes[2].ChildNodes[0].Term.Name));
            ConfigValue val = new ConfigValue(); ;
            ParseValue(ref val, propnode.ChildNodes[2].ChildNodes[0]);
            par.Properties[name] = val;
        }

        private void ParseValue(ref ConfigValue par, ParseTreeNode valnode)
        {
            switch (valnode.Term.Name)
            {
                case "Number":
                    par.Type = ConfigValueType.Number;
                    par.NumberValue = Convert.ToDouble(valnode.Token.ValueString);
                    break;

                case "String":
                    par.Type = ConfigValueType.String;
                    par.StringValue = valnode.Token.ValueString;
                    break;

                case "Array":
                    par.Type = ConfigValueType.Array;
                    ParseArrayValue(ref par._av, valnode.ChildNodes[0]);
                    break;

                default:
                    throw new InvalidDataException("何故だッ!");
            }
        }

        private void ParseArrayValue(ref List<ConfigValue> par, ParseTreeNode valnode)
        {
            foreach (var i in valnode.ChildNodes)
            {
                ConfigValue v = new ConfigValue();
                ParseValue(ref v, i.ChildNodes[0]);
                par.Add(v);
            }
        }

        #endregion

    }

    /// <summary>
    /// Kb10uyConfigでのグループを表す。
    /// </summary>
    public class ConfigGroup : IKb10uyConfigHasGroups
    {
        internal Dictionary<string, ConfigGroup> gs = new Dictionary<string, ConfigGroup>();
        internal Dictionary<string, ConfigValue> ps = new Dictionary<string, ConfigValue>();

        /// <summary>
        /// 所属する全てのグループ
        /// </summary>
        public IDictionary<string, ConfigGroup> Groups
        {
            get { return gs; }
        }

        /// <summary>
        /// 所属する全てのプロパティ
        /// </summary>
        public IDictionary<string, ConfigValue> Properties
        {
            get { return ps; }
        }
    }

    /// <summary>
    /// Kb10uyConfigでのプロパティを表す。
    /// </summary>
    public class ConfigProperty
    {
        internal ConfigValue _value = new ConfigValue();

        /// <summary>
        /// プロパティの値
        /// </summary>
        public ConfigValue Value
        {
            get { return _value; }
            set { _value = value; }
        }
    }

    /// <summary>
    /// Kb10uyConfigでの値を表す。
    /// 数値・文字列に関してはexplicitな型変換演算子が、
    /// 配列型に関してはIEnumerable&lt;Kb10uyConfigValue&gt;が
    /// 実装されている。
    /// </summary>
    public class ConfigValue : IEnumerable<ConfigValue>
    {

        internal List<ConfigValue> _av = new List<ConfigValue>();

        /// <summary>
        /// 値の型
        /// </summary>
        public ConfigValueType Type { get; set; }

        /// <summary>
        /// 数値
        /// </summary>
        public double NumberValue { get; set; }

        /// <summary>
        /// 文字列
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// 配列
        /// </summary>
        public IList<ConfigValue> ArrayValue { get { return _av; } }

        /// <summary>
        /// 新しく(ry
        /// </summary>
        public ConfigValue()
        {
            NumberValue = 0;
            StringValue = "";
        }

        /// <summary>
        /// 型変換
        /// </summary>
        /// <param name="t">--</param>
        /// <returns>--</returns>
        public static explicit operator double(ConfigValue t)
        {
            return t.NumberValue;
        }

        /// <summary>
        /// 型変換
        /// </summary>
        /// <param name="t">--</param>
        /// <returns>--</returns>
        public static explicit operator string(ConfigValue t)
        {
            return t.StringValue;
        }

        /// <summary>
        /// 型変換
        /// </summary>
        /// <param name="t">--</param>
        /// <returns>--</returns>
        public static implicit operator ConfigValue(double t)
        {
            return new ConfigValue { NumberValue = t, Type = ConfigValueType.Number };
        }

        /// <summary>
        /// 型変換
        /// </summary>
        /// <param name="t">--</param>
        /// <returns>--</returns>
        public static implicit operator ConfigValue(string t)
        {
            return new ConfigValue { StringValue = t, Type = ConfigValueType.String };
        }

        /// <summary>
        /// 型変換
        /// </summary>
        /// <param name="t">--</param>
        /// <returns>--</returns>
        public static implicit operator ConfigValue(ConfigValue[] t)
        {
            return new ConfigValue { _av = t.ToList(), Type = ConfigValueType.Array };
        }

        /// <summary>
        /// 型変換的に使ってね
        /// </summary>
        /// <param name="t">--</param>
        /// <returns>--</returns>

        public static ConfigValue GenerateArray(IEnumerable<ConfigValue> t)
        {
            return new ConfigValue { _av = t.ToList(), Type = ConfigValueType.Array };
        }

        /// <summary>
        /// ArrayValueからKb10uyConfigValueを取得
        /// </summary>
        /// <param name="id">インデックス</param>
        /// <returns>階下のKb10uyConfigValue</returns>
        public ConfigValue this[int id]
        {
            get { return ArrayValue[id]; }
        }

        #region IEnumerable<Kb10uyConfigValue>の実装

        /// <summary>
        /// コレクションを反復処理する列挙子を返します。
        /// </summary>
        /// <returns>コレクションを反復処理する列挙子</returns>
        public IEnumerator<ConfigValue> GetEnumerator()
        {
            return ArrayValue.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return ArrayValue.GetEnumerator();
        }

        #endregion
    }

    /// <summary>
    /// Kb10uyConfigValueの型を指定します
    /// </summary>
    public enum ConfigValueType
    {
        /// <summary>
        /// 数値
        /// </summary>
        Number,

        /// <summary>
        /// 文字列
        /// </summary>
        String,

        /// <summary>
        /// 配列
        /// </summary>
        Array,
    }

}
