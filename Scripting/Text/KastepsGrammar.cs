using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Irony;
using Irony.Ast;
using Irony.Parsing;

namespace Kb10uy.Scripting.Text
{
    /// <summary>
    /// Kastepsの解析器。
    /// </summary>
    [Language("Kasteps", "1.0.0", "Kb10uy's Awk&Sed-like TExt Processing Script")]
    public class KastepsGrammar : Grammar
    {
        /// <summary>
        /// コンストラクタ
        /// </summary>
        public KastepsGrammar()
        {
            #region 終端・コメント
            var Number = new NumberLiteral("Number");
            Number.DefaultIntTypes = new[] { TypeCode.Int32 };
            Number.DefaultFloatType = TypeCode.Double;

            var String = new StringLiteral("String", "\"", StringOptions.AllowsAllEscapes);
            String.EscapeChar = '\\';

            var Identifer = new IdentifierTerminal("Identifer");

            var CommentLine = new CommentTerminal("CommentLine", "//", "\r", "\n");
            NonGrammarTerminals.Add(CommentLine);
            var CommentBlock = new CommentTerminal("CommentBlock", "/*", "*/");
            NonGrammarTerminals.Add(CommentBlock);
            #endregion

            #region キーワード
            var Begin = ToTerm("begin");
            var End = ToTerm("end");
            var Every = ToTerm("every");
            var Function = ToTerm("function");
            var Return = ToTerm("return");
            var If = ToTerm("if");
            var Else = ToTerm("else");
            var For = ToTerm("for");
            var While = ToTerm("while");
            var Continue = ToTerm("continue");
            var Break = ToTerm("break");
            var Local = ToTerm("local");
            var True = ToTerm("true");
            var False = ToTerm("false");
            var Nil = ToTerm("nil");
            #endregion

            #region 演算子
            var BinaryOperator = new NonTerminal("BinaryOperator");
            var UnaryOperator = new NonTerminal("UnaryOperator");
            var PostFixOperator = new NonTerminal("PostFixOperator");
            var AssignmentOperator = new NonTerminal("AssignmentOperator");
            #endregion

            #region 式・項
            var Expression = new NonTerminal("Expression");
            var BinaryExpression = new NonTerminal("BinaryExpression");
            var ParenExpression = new NonTerminal("ParenExpression");
            var UnaryExpression = new NonTerminal("UnaryExpression");
            var PostFixExpression = new NonTerminal("PostFixExpression");

            var Term = new NonTerminal("Term");
            #endregion

            #region 補助非終端
            var FunctionCallingArgumentList = new NonTerminal("FunctionCallingArgumentList");
            var LocalDefiningVariableList = new NonTerminal("LocalDefiningVariableList");
            var LocalDefininable = new NonTerminal("LocalDefininable");
            var RuleDefinitionCondition = new NonTerminal("RuleDefinitionCondition");
            var FunctionDefinitionArgumentList = new NonTerminal("FunctionDefinitionargumentList");
            #endregion

            #region 構文
            var FunctionCallingStatement = new NonTerminal("FunctionCallingStatement");
            var LocalDefiningStatement = new NonTerminal("LocalDefineingStatement");
            var AssignmentStatement = new NonTerminal("AssignmentStatement");

            var FunctionDefinitionStatement = new NonTerminal("FunctionDefinitionStatement");
            var ReturnStatement = new NonTerminal("ReturnStatement");
            var RuleDefinitionStatement = new NonTerminal("RuleDefinitionStatement");

            var IfStatement = new NonTerminal("IfStatement");
            var ElseStatement = new NonTerminal("ElseStatement");
            var ForStatement = new NonTerminal("ForStatement");
            var WhileStatement = new NonTerminal("WhileStatement");

            var MonoStatements = new NonTerminal("MonoStatements");
            #endregion

            #region メタ
            var ProgramLine = new NonTerminal("ProgramLine");
            var ProgramLineSet = new NonTerminal("ProgramLineSet");
            var ProgramBlock = new NonTerminal("ProgramBlock");
            var RunnableBlock = new NonTerminal("RunnableBlock");
            var Program = new NonTerminal("Program");
            #endregion

            #region Rule設定
            Expression.Rule = Term | UnaryExpression | BinaryExpression | PostFixExpression;
            Term.Rule = Number | String | True | False | Nil | Identifer | FunctionCallingStatement | ParenExpression;
            ParenExpression.Rule = "(" + Expression + ")";
            UnaryExpression.Rule = UnaryOperator + Term;
            UnaryOperator.Rule = ToTerm("+") | "-" | "!" | "++" | "--";
            BinaryExpression.Rule = Expression + BinaryOperator + Expression;
            BinaryOperator.Rule = ToTerm("+") | "-" | "*" | "/" | "%" | "^" | "<<" | ">>" | "&" | "|" |
                                  "==" | "!=" | "<" | ">" | "<=" | ">=" | "&&" | "||";
            PostFixExpression.Rule = Term + PostFixOperator;
            PostFixOperator.Rule = ToTerm("++") | "--";
            AssignmentStatement.Rule = Identifer + AssignmentOperator + Expression;
            AssignmentOperator.Rule = ToTerm("=") | "+=" | "-=" | "*=" | "/=" | "%=" | "^=" | "<<=" | ">>=" | "&=" | "|=";
            LocalDefininable.Rule = Identifer | AssignmentStatement;
            LocalDefiningVariableList.Rule = MakeStarRule(LocalDefiningVariableList, ToTerm(","), LocalDefininable);
            LocalDefiningStatement.Rule = Local + LocalDefiningVariableList;
            FunctionCallingStatement.Rule = Identifer + "(" + FunctionCallingArgumentList + ")";
            FunctionCallingArgumentList.Rule = MakeStarRule(FunctionCallingArgumentList, ToTerm(","), Expression);

            //第一級
            FunctionDefinitionArgumentList.Rule = MakeStarRule(FunctionDefinitionArgumentList, ToTerm(","), Identifer);
            FunctionDefinitionStatement.Rule = Function + Identifer + "(" + FunctionDefinitionArgumentList + ")" + "{" + ProgramLineSet + "}";
            ReturnStatement.Rule = Return | Return + Expression;
            RuleDefinitionStatement.Rule = RuleDefinitionCondition + "{" + ProgramLineSet + "}";

            //ブロック構文
            RunnableBlock.Rule = "{" + ProgramLineSet + "}" | ProgramLine;
            IfStatement.Rule = If + "(" + Expression + ")" + RunnableBlock;
            ElseStatement.Rule = Else + RunnableBlock;

            //ForInitializable.Rule = MonoStatements | IfStatement | ForStatement | WhileStatement;
            //ForInitializeList.Rule = MakeStarRule(ForInitializeList, ToTerm(","), ForInitializable);
            //ForCountingList.Rule = MakeStarRule(ForCountingList, ToTerm(","), ForInitializable);
            ForStatement.Rule = For + "(" + MonoStatements + ";" + Expression + ";" + MonoStatements + ")" + RunnableBlock;

            WhileStatement.Rule = While + "(" + Expression + ")" + RunnableBlock;

            //メタ
            MonoStatements.Rule = FunctionCallingStatement | LocalDefiningStatement | AssignmentStatement |
                                  UnaryExpression | PostFixExpression |
                                  ReturnStatement | Break | Continue |
                                  Empty;
            ProgramLine.Rule = MonoStatements + ";" | IfStatement | ForStatement | WhileStatement | ElseStatement;
            ProgramLineSet.Rule = MakeStarRule(ProgramLineSet, ProgramLine);
            RuleDefinitionCondition.Rule = String | Number | Begin | End | Every;
            ProgramBlock.Rule = RuleDefinitionStatement | FunctionDefinitionStatement;
            Program.Rule = MakeStarRule(Program, ProgramBlock);
            #endregion

            RegisterOperators(1, "||");
            RegisterOperators(2, "&&");
            RegisterOperators(3, "&");
            RegisterOperators(4, "^");
            RegisterOperators(5, "|");
            RegisterOperators(6, "==", "!=");
            RegisterOperators(7, "<", ">", "<=", ">=");
            RegisterOperators(8, "<<", ">>");
            RegisterOperators(9, "+", "-");
            RegisterOperators(10, "*", "/", "%");

            MarkPunctuation("(", ")", "{", "}", ";");
            RegisterBracePair("(", ")");
            RegisterBracePair("{", "}");

            MarkTransient(Term, Expression, MonoStatements, BinaryOperator,
                          UnaryOperator, PostFixOperator, AssignmentOperator,
                          ParenExpression, FunctionCallingArgumentList, LocalDefiningVariableList,
                          ProgramLine, LocalDefininable, ProgramBlock, RuleDefinitionCondition,
                          RunnableBlock);

            Root = Program;
        }
    }
}
