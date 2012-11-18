using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TinyPG;
using TinyPG.Compiler;

namespace TinyPG.CodeGenerators.CSharp
{
    public class ScannerGenerator : ICodeGenerator
    {
        internal ScannerGenerator()
        {
        }

        public string FileName
        {
            get { return "Scanner.cs"; }
        }

        public string Generate(Grammar Grammar, bool Debug)
        {
            if (string.IsNullOrEmpty(Grammar.GetTemplatePath()))
                return null;

            string scanner = File.ReadAllText(Grammar.GetTemplatePath() + FileName);

            int counter = 2;
            StringBuilder tokentype = new StringBuilder();
            StringBuilder regexps = new StringBuilder();
            StringBuilder skiplist = new StringBuilder();

            foreach (TerminalSymbol s in Grammar.SkipSymbols)
            {
                skiplist.AppendLine("            SkipList.Add(TokenType." + s.Name + ");");
            }

            // build system tokens
            tokentype.AppendLine("\r\n            //Non terminal tokens:");
            tokentype.AppendLine(Helper.Outline("_NONE_", 3, "= 0,", 5));
            tokentype.AppendLine(Helper.Outline("_UNDETERMINED_", 3, "= 1,", 5));

            // build non terminal tokens
            tokentype.AppendLine("\r\n            //Non terminal tokens:");
            foreach (Symbol s in Grammar.GetNonTerminals())
            {
                tokentype.AppendLine(Helper.Outline(s.Name, 3, "= " + String.Format("{0:d},", counter), 5));
                counter++;
            }

            // build terminal tokens
            tokentype.AppendLine("\r\n            //Terminal tokens:");
            bool first = true;
            foreach (TerminalSymbol s in Grammar.GetTerminals())
            {
                regexps.Append("            regex = new Regex(" + s.Expression.ToString() + ", RegexOptions.Compiled");

                if (s.Attributes.ContainsKey("IgnoreCase"))
                    regexps.Append(" | RegexOptions.IgnoreCase");
                
                regexps.Append(");\r\n");                    

                regexps.Append("            Patterns.Add(TokenType." + s.Name + ", regex);\r\n");
                regexps.Append("            Tokens.Add(TokenType." + s.Name + ");\r\n\r\n");

                if (first) first = false;
                else tokentype.AppendLine(",");

                tokentype.Append(Helper.Outline(s.Name, 3, "= " + String.Format("{0:d}", counter), 5));
                counter++;
            }

            scanner = scanner.Replace(@"<%SkipList%>", skiplist.ToString());
            scanner = scanner.Replace(@"<%RegExps%>", regexps.ToString());
            scanner = scanner.Replace(@"<%TokenType%>", tokentype.ToString());

            if (Debug)
            {
                scanner = scanner.Replace(@"<%Namespace%>", "TinyPG.Debug");
                scanner = scanner.Replace(@"<%IToken%>", " : TinyPG.Debug.IToken");
            }
            else
            {
                scanner = scanner.Replace(@"<%Namespace%>", Grammar.Directives["TinyPG"]["Namespace"]);
                scanner = scanner.Replace(@"<%IToken%>", "");
            }

            return scanner;
        }
    }
}
