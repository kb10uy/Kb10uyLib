using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony;
using Irony.Parsing;
using System.Text.RegularExpressions;

namespace Kb10uy.Scripting.Text
{
    /// <summary>
    /// Kastepsの実行環境を提供します。
    /// </summary>
    public class KastepsRuntime
    {
        /// <summary>
        /// スクリプトの解析データ
        /// </summary>
        public Kasteps ScriptData { get; set; }

        /// <summary>
        /// 登録されているルール
        /// </summary>
        public IList<KastepsRule> Rules { get; set; }

        /// <summary>
        /// 登録されている関数
        /// </summary>
        public IList<KastepsFunction> Functions { get; set; }

        /// <summary>
        /// 登録されている変数
        /// </summary>
        public IDictionary<string, KastepsVariable> Variables { get; set; }

        /// <summary>
        /// 新しいVMを初期化します。
        /// </summary>
        public KastepsRuntime()
        {
            ScriptData = new Kasteps();
            Rules = new List<KastepsRule>();
            Functions = new List<KastepsFunction>();
            Variables = new Dictionary<string, KastepsVariable>();
        }

        /// <summary>
        /// ファイルからKastepsスクリプトを読み込みます。
        /// </summary>
        /// <param name="file">ファイル名</param>
        public void LoadScriptFromFile(string file)
        {
            ScriptData.LoadScriptFile(file);
            Parse();
        }

        /// <summary>
        /// スクリプトを記述した文字列を読み込みます。
        /// </summary>
        /// <param name="srcstr">文字列</param>
        public void LoadScriptFromString(string srcstr)
        {
            ScriptData.LoadScriptString(srcstr);
            Parse();
        }

        #region パース用

        void Parse()
        {
            var topl = ScriptData.RawTree.Root;
            foreach (var i in topl.ChildNodes)
            {
                switch (i.Term.Name)
                {
                    case "FunctionDefinitionStatement":
                        ParseFunction(i);
                        break;
                    case "RuleDefinitionStatement":
                        ParseRule(i);
                        break;
                    default:
                        break;
                }
            }
        }

        void ParseFunction(ParseTreeNode node)
        {
            Console.WriteLine(node);
        }

        void ParseRule(ParseTreeNode node)
        {
            var t = node.ChildNodes[0];
            KastepsRule r=new KastepsRule();
            switch (t.Term.Name)
            {
                case "Number":
                    r.ConditionType = KastepsRuleConditionType.LineNumber;
                    r.LineNumber = (int)t.Token.Value;
                    break;
                case "String":
                    r.ConditionType = KastepsRuleConditionType.RegularExpression;
                    r.Regex = new Regex(t.Token.Value as string);
                    break;
                case "begin":
                    r.ConditionType = KastepsRuleConditionType.Begin;
                    break;
                case "every":
                    r.ConditionType = KastepsRuleConditionType.Every;
                    break;
                case "end":
                    r.ConditionType = KastepsRuleConditionType.End;
                    break;
                default:
                    break;
            }
        }

        #endregion
    }

    /// <summary>
    /// Kastepsの変数を定義します。
    /// </summary>
    public class KastepsVariable
    {
        /// <summary>
        /// 変数の範囲。trueの場合グローバル変数
        /// </summary>
        public bool IsGlobal { get; set; }

        /// <summary>
        /// 実際の値。
        /// </summary>
        public KastepsValue Value { get; set; }
    }

    /// <summary>
    /// Kastepsの関数を定義します。
    /// </summary>
    public class KastepsFunction
    {
        /// <summary>
        /// 関数の種類
        /// </summary>
        public KastepsFunctionType Type { get; set; }

        /// <summary>
        /// 仮引数名のリスト
        /// </summary>
        public IList<string> ArgumentNames { get; set; }

        /// <summary>
        /// ScriptFunctionのコード
        /// </summary>
        internal IList<KastepsProgramCode> Codes { get; set; }

        /// <summary>
        /// UserFunctionの関数
        /// </summary>
        public KastepsUserFunction UserFunction { get; set; }

        /// <summary>
        /// ユーザー定義関数として初期化します。
        /// </summary>
        /// <param name="ufnc">関数</param>
        public KastepsFunction(KastepsUserFunction ufnc)
        {
            Type = KastepsFunctionType.UserFunction;
            UserFunction = ufnc;
        }

        /// <summary>
        /// Kasteps VMコード関数として初期化します。
        /// </summary>
        public KastepsFunction()
        {
            Type = KastepsFunctionType.ScriptFunction;
            Codes = new List<KastepsProgramCode>();
        }
    }

    /// <summary>
    /// Kastepsの関数の種類を定義します。
    /// </summary>
    public enum KastepsFunctionType
    {
        /// <summary>
        /// ユーザー定義関数
        /// </summary>
        UserFunction,
        /// <summary>
        /// スクリプト内関数
        /// </summary>
        ScriptFunction,
    }

    /// <summary>
    /// kastepsで自由に追加できる関数のデリゲート。
    /// </summary>
    /// <param name="args">渡された引数</param>
    /// <returns>返り値</returns>
    public delegate KastepsValue KastepsUserFunction(params KastepsValue[] args);

    /// <summary>
    /// Kastepsの編集ルールを定義します。
    /// </summary>
    public class KastepsRule
    {
        /// <summary>
        /// 条件の種類
        /// </summary>
        public KastepsRuleConditionType ConditionType { get; set; }

        /// <summary>
        /// 対応する行番号
        /// </summary>
        public int? LineNumber { get; set; }

        /// <summary>
        /// 対応する正規表現
        /// </summary>
        public Regex Regex { get; set; }


        internal IList<KastepsProgramCode> Codes { get; set; }

        /// <summary>
        /// インスタンスを初期化します。
        /// </summary>
        public KastepsRule()
        {
            Codes = new List<KastepsProgramCode>();
        }
    }

    /// <summary>
    /// Kastepsの値を定義します。
    /// </summary>
    public class KastepsValue
    {
        /// <summary>
        /// 値の型
        /// </summary>
        public KastepsValueType Type { get; set; }

        /// <summary>
        /// Integer型の値
        /// </summary>
        public int? IntegerValue { get; set; }

        /// <summary>
        /// Float型の値
        /// </summary>
        public double? FloatValue { get; set; }

        /// <summary>
        /// String型の値
        /// </summary>
        public string StringValue { get; set; }

        /// <summary>
        /// Boolean型の値
        /// </summary>
        public bool BooleanValue { get; set; }

        /// <summary>
        /// Nil型として作成
        /// </summary>
        public KastepsValue()
        {
            Type = KastepsValueType.Nil;
        }

        /// <summary>
        /// Integer型として作成
        /// </summary>
        /// <param name="v">値</param>
        public KastepsValue(int v)
        {
            Type = KastepsValueType.Integer;
            IntegerValue = v;
        }

        /// <summary>
        /// Float型として作成
        /// </summary>
        /// <param name="v">値</param>
        public KastepsValue(double v)
        {
            Type = KastepsValueType.Float;
            FloatValue = v;
        }

        /// <summary>
        /// String型として作成
        /// </summary>
        /// <param name="v">値</param>
        public KastepsValue(string v)
        {
            Type = KastepsValueType.String;
            StringValue = v;
        }

        /// <summary>
        /// Boolean型として作成
        /// </summary>
        /// <param name="v">値</param>
        public KastepsValue(bool v)
        {
            Type = KastepsValueType.Boolean;
            BooleanValue = v;
        }

    }

    /// <summary>
    /// Kastepsの値の種類を定義します。
    /// </summary>
    public enum KastepsValueType
    {
        /// <summary>
        /// nil(無)
        /// </summary>
        Nil,
        /// <summary>
        /// 32bit符号付き整数
        /// </summary>
        Integer,
        /// <summary>
        /// 倍精度浮動小数点数
        /// </summary>
        Float,
        /// <summary>
        /// 文字列
        /// </summary>
        String,
        /// <summary>
        /// 真偽値
        /// </summary>
        Boolean,
    }

    /// <summary>
    /// Kastepsのルールの条件の種類を定義します。
    /// </summary>
    public enum KastepsRuleConditionType
    {
        /// <summary>
        /// スクリプト開始時
        /// </summary>
        Begin,
        /// <summary>
        /// スクリプト終了時
        /// </summary>
        End,
        /// <summary>
        /// すべての行
        /// </summary>
        Every,
        /// <summary>
        /// 正規表現
        /// </summary>
        RegularExpression,
        /// <summary>
        /// 行番号
        /// </summary>
        LineNumber,
    }

    internal class KastepsCodeData
    {
        public KastepsVariable Variable { get; set; }
        public KastepsValue Value { get; set; }
    }

    internal class KastepsProgramCode
    {
        public KastepsVmCode Code { get; set; }
        public KastepsCodeData Data { get; set; }
    }

    /// <summary>
    /// Kastepsの内部コードを定義します。
    /// </summary>
    internal enum KastepsVmCode
    {
        /// <summary>
        /// No-Operation
        /// </summary>
        Nop,
        /// <summary>
        /// Set Label
        /// </summary>
        Slb,
        /// <summary>
        /// Push
        /// </summary>
        Psh,
        /// <summary>
        /// Push Argument
        /// </summary>
        Psa,
        /// <summary>
        /// Pop
        /// </summary>
        Pop,

        /// <summary>
        /// Let
        /// </summary>
        Let,

        /// <summary>
        /// Add
        /// </summary>
        Add,

        /// <summary>
        /// Subtract
        /// </summary>
        Sub,
        /// <summary>
        /// Multiple
        /// </summary>
        Mul,
        /// <summary>
        /// Divide
        /// </summary>
        Div,
        /// <summary>
        /// Divide Ex
        /// </summary>
        Dex,
        /// <summary>
        /// And
        /// </summary>
        And,
        /// <summary>
        /// Or
        /// </summary>
        Or,
        /// <summary>
        /// Exclusive Or
        /// </summary>
        Xor,
        /// <summary>
        /// Lefter Shift
        /// </summary>
        Lsh,
        /// <summary>
        /// Righter Shift
        /// </summary>
        Rsh,

        /// <summary>
        /// Equal
        /// </summary>
        Eql,
        /// <summary>
        /// Not Equal
        /// </summary>
        Neq,
        /// <summary>
        /// Greater Equal
        /// </summary>
        Geq,
        /// <summary>
        /// Lesser Equal
        /// </summary>
        Leq,
        /// <summary>
        /// Greater
        /// </summary>
        Grt,
        /// <summary>
        /// Lesser
        /// </summary>
        Les,
        /// <summary>
        /// Short-circuit And
        /// </summary>
        Ssa,
        /// <summary>
        /// Short-circuit Or
        /// </summary>
        Sso,
        /// <summary>
        /// Reverse
        /// </summary>
        Rev,

        /// <summary>
        /// Jump
        /// </summary>
        Jmp,
        /// <summary>
        /// True Jump
        /// </summary>
        Jmt,
        /// <summary>
        /// Return
        /// </summary>
        Ret,

        /// <summary>
        /// Call Function
        /// </summary>
        Cal,

    }
}
