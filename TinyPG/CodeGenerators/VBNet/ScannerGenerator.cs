using System;
using System.Text;
using System.IO;
using TinyPG.Compiler;

namespace TinyPG.CodeGenerators.VBNet
{
    public class ScannerGenerator : BaseGenerator, ICodeGenerator
    {
        internal ScannerGenerator()
            : base("Scanner.vb")
        {
        }

        public string Generate(Grammar Grammar, bool Debug)
        {
            if (string.IsNullOrEmpty(Grammar.GetTemplatePath()))
                return null;

            string scanner = File.ReadAllText(Grammar.GetTemplatePath() + templateName);

            int counter = 2;
            StringBuilder tokentype = new StringBuilder();
            StringBuilder regexps = new StringBuilder();
            StringBuilder skiplist = new StringBuilder();

            foreach (TerminalSymbol s in Grammar.SkipSymbols)
            {
                skiplist.AppendLine("            SkipList.Add(TokenType." + s.Name + ")");
            }

            // build system tokens
            tokentype.AppendLine("\r\n        'Non terminal tokens:");
            tokentype.AppendLine(Helper.Outline("_NONE_", 2, "= 0", 5));
            tokentype.AppendLine(Helper.Outline("_UNDETERMINED_", 2, "= 1", 5));

            // build non terminal tokens
            tokentype.AppendLine("\r\n        'Non terminal tokens:");
            foreach (Symbol s in Grammar.GetNonTerminals())
            {
                tokentype.AppendLine(Helper.Outline(s.Name, 2, "= " + String.Format("{0:d}", counter), 5));
                counter++;
            }

            // build terminal tokens
            tokentype.AppendLine("\r\n        'Terminal tokens:");
            bool first = true;
            foreach (TerminalSymbol s in Grammar.GetTerminals())
            {
                string vbexpr = s.Expression.ToString();
                if (vbexpr.StartsWith("@"))
                    vbexpr = vbexpr.Substring(1);
                regexps.Append("            regex = new Regex(" + vbexpr + ", RegexOptions.Compiled)\r\n");
                regexps.Append("            Patterns.Add(TokenType." + s.Name + ", regex)\r\n");
                regexps.Append("            Tokens.Add(TokenType." + s.Name + ")\r\n\r\n");

                if (first) first = false;
                else tokentype.AppendLine("");

                tokentype.Append(Helper.Outline(s.Name, 2, "= " + String.Format("{0:d}", counter), 5));
                counter++;
            }

            scanner = scanner.Replace(@"<%SkipList%>", skiplist.ToString());
            scanner = scanner.Replace(@"<%RegExps%>", regexps.ToString());
            scanner = scanner.Replace(@"<%TokenType%>", tokentype.ToString());

            if (Debug)
            {
                scanner = scanner.Replace(@"<%Imports%>", "Imports TinyPG.Debug");
                scanner = scanner.Replace(@"<%Namespace%>", "TinyPG.Debug");
                scanner = scanner.Replace(@"<%IToken%>", "\r\n        Implements IToken");
                scanner = scanner.Replace(@"<%ImplementsITokenStartPos%>", " Implements IToken.StartPos");
                scanner = scanner.Replace(@"<%ImplementsITokenEndPos%>", " Implements IToken.EndPos");
                scanner = scanner.Replace(@"<%ImplementsITokenLength%>", " Implements IToken.Length");
                scanner = scanner.Replace(@"<%ImplementsITokenText%>", " Implements IToken.Text");
                scanner = scanner.Replace(@"<%ImplementsITokenToString%>", " Implements IToken.ToString");

            }
            else
            {
                scanner = scanner.Replace(@"<%Imports%>", "");
                scanner = scanner.Replace(@"<%Namespace%>", Grammar.Directives["TinyPG"]["Namespace"]);
                scanner = scanner.Replace(@"<%IToken%>", "");
                scanner = scanner.Replace(@"<%ImplementsITokenStartPos%>", "");
                scanner = scanner.Replace(@"<%ImplementsITokenEndPos%>", "");
                scanner = scanner.Replace(@"<%ImplementsITokenLength%>", "");
                scanner = scanner.Replace(@"<%ImplementsITokenText%>", "");
                scanner = scanner.Replace(@"<%ImplementsITokenToString%>", "");
            }

            return scanner;
        }
    }
}
