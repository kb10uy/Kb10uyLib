using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony;
using Irony.Parsing;
using System.IO;

namespace Kb10uy.Scripting.Text
{
    /// <summary>
    /// awkやsedのようなテキスト処理言語「Kasteps」の機能の底辺層を提供します。
    /// </summary>
    public class Kasteps
    {
        /// <summary>
        /// スクリプトの文字列。
        /// </summary>
        public string RawString { get; set; }

        /// <summary>
        /// スクリプトの解析結果のツリー。
        /// </summary>
        public ParseTree RawTree { get; set; }

        /// <summary>
        /// Kastepsの解析器。
        /// </summary>
        public static KastepsGrammar Grammar { get; protected set; }

        /// <summary>
        /// Kastepsのパーサ。
        /// </summary>
        public static Parser Parser { get; protected set; }

        static Kasteps()
        {
            Grammar = new KastepsGrammar();
            Parser = new Parser(Grammar);
        }

        /// <summary>
        /// Kastepsインスタンスを初期化します。
        /// </summary>
        public Kasteps()
        {

        }

        /// <summary>
        /// 処理内容を書いたスクリプトをファイルから読み込みます。
        /// </summary>
        /// <param name="filename">ファイル名</param>
        public void LoadScriptFile(string filename)
        {
            RawString = File.ReadAllText(filename);
            Parse();
        }

        /// <summary>
        /// 処理内容を書いたスクリプトを文字列から読み込みます。
        /// </summary>
        /// <param name="sstr">文字列</param>
        public void LoadScriptString(string sstr)
        {
            RawString = sstr;
            Parse();
        }

        /// <summary>
        /// スクリプトを解析し、処理ができるようにします。
        /// 通常、この関数はスクリプトを読み込んだ時点で自動的に実行されます。
        /// </summary>
        public void Parse()
        {
            RawTree = Parser.Parse(RawString);
            if (RawTree.HasErrors())
            {
                var exm = "Kastepsスクリプトにエラーがありました。" + Environment.NewLine;
                foreach (var m in RawTree.ParserMessages)
                {
                    exm += String.Format("{0} @[{1},{2}]: {3}", m.Level, m.Location.Line, m.Location.Column, m.Message);
                }
                throw new InvalidDataException(exm);
            }
        }

    }
}
