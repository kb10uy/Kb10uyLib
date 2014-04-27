using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Irony;
using Irony.Parsing;

namespace Kb10uy.Scripting
{
    /// <summary>
    /// Kb10uyConfigの解析器
    /// </summary>
    [Language("Kb10uyConfig", "1.0", "多分便利なコンフィグ")]
    public class Kb10uyConfigGrammar : Grammar
    {
        /// <summary>
        /// 新しいインスタンスを以下略
        /// </summary>
        public Kb10uyConfigGrammar()
            : base()
        {
            var Number = new NumberLiteral("Number");
            var String = new StringLiteral("String", "\"", StringOptions.AllowsAllEscapes);
            var CommentLine = new CommentTerminal("Comment", "#", "\n", "\r");
            var CommentBlock = new CommentTerminal("Comment", "#<", ">");
            //数値設定
            Number.DefaultIntTypes = new[] { TypeCode.Int32 };
            Number.DefaultFloatType = TypeCode.Double;
            String.EscapeChar = '\\';
            NonGrammarTerminals.Add(CommentBlock);
            NonGrammarTerminals.Add(CommentLine);

            var Value = new NonTerminal("Value");
            var Values = new NonTerminal("Value\'s\'");
            var ValueSet = new NonTerminal("Array");
            var Property = new NonTerminal("Property");
            var Prefix = new NonTerminal("Prefix");
            var Node = new NonTerminal("Node");
            var Nodes = new NonTerminal("Nodes");
            //var PropertySet = new NonTerminal("PropertySet");
            var Group = new NonTerminal("Group");
            var ConfigRoot = new NonTerminal("Root");

            Value.Rule = Number | String | ValueSet;
            Values.Rule = MakeStarRule(Values, ToTerm(","), Value);
            ValueSet.Rule = ToTerm("[") + Values + "]";
            Prefix.Rule = ToTerm("Number") | "String" | "Array" ;
            Property.Rule = Prefix + ":" + String + "=" + Value + ";";
            Node.Rule = Property | Group;
            Nodes.Rule = MakeStarRule(Nodes, Node);
            Group.Rule = String + ":" + "{" +Nodes+ "}";
            ConfigRoot.Rule = MakeStarRule(ConfigRoot, Group);
            Root = ConfigRoot;

            MarkPunctuation("[", "]", ",", "{", "}",":",";","=");

        }
    }
}
