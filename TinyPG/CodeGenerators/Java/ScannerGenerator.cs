using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using TinyPG;
using TinyPG.Compiler;

namespace TinyPG.CodeGenerators.Java
{
    public class ScannerGenerator : BaseGenerator, ICodeGenerator
    {
        internal ScannerGenerator() : base("Scanner.java")
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
                skiplist.AppendLine("            SkipList.add(TokenType." + s.Name + ");");
            }

			if (Grammar.FileAndLine != null)
				skiplist.AppendLine("            FileAndLine = TokenType." + Grammar.FileAndLine.Name + ";");

			// build system tokens
			tokentype.AppendLine("\r\n            //Non terminal tokens:");
            tokentype.AppendLine(Helper.Outline("_NONE_", 3, ",", 5));
            tokentype.AppendLine(Helper.Outline("_UNDETERMINED_", 3, ",", 5));

            // build non terminal tokens
            tokentype.AppendLine("\r\n            //Non terminal tokens:");
            foreach (Symbol s in Grammar.GetNonTerminals())
            {
                tokentype.AppendLine(Helper.Outline(s.Name, 3, ",", 5));
                counter++;
            }

            // build terminal tokens
            tokentype.AppendLine("\r\n            //Terminal tokens:");
            bool first = true;
            foreach (TerminalSymbol s in Grammar.GetTerminals())
            {
                regexps.Append("            regex = Pattern.compile(" + Unverbatim(s.Expression.ToString()));
				if (s.Attributes.ContainsKey("IgnoreCase"))
					regexps.Append(", Pattern.CASE_INSENSITIVE");
				regexps.Append(");\r\n");
				regexps.Append("            Patterns.put(TokenType." + s.Name + ", regex);\r\n");
                regexps.Append("            Tokens.add(TokenType." + s.Name + ");\r\n\r\n");

                if (first) first = false;
                else tokentype.AppendLine(",");

                tokentype.Append(Helper.Outline(s.Name, 3, "", 5));
                counter++;
            }

			scanner = scanner.Replace(@"<%SourceFilename%>", Grammar.SourceFilename);
			scanner = scanner.Replace(@"<%SkipList%>", skiplist.ToString());
            scanner = scanner.Replace(@"<%RegExps%>", regexps.ToString());
            scanner = scanner.Replace(@"<%TokenType%>", tokentype.ToString());

            if (Debug)
            {
                scanner = scanner.Replace(@"<%Namespace%>", "TinyPG.Debug");
				scanner = scanner.Replace(@"<%IToken%>", " : TinyPG.Debug.IToken");
				scanner = scanner.Replace(@"<%ScannerCustomCode%>", Grammar.Directives["Scanner"]["CustomCode"]);
			}
            else
            {
                scanner = scanner.Replace(@"<%Namespace%>", Grammar.Directives["TinyPG"]["Namespace"]);
                scanner = scanner.Replace(@"<%IToken%>", "");
				scanner = scanner.Replace(@"<%ScannerCustomCode%>", Grammar.Directives["Scanner"]["CustomCode"]);
			}

            return scanner;
        }

		private string Unverbatim(string v)
		{
			if(v[0] == '@')
			{
				v = v.Substring(1);
				v = v.Replace(@"\", @"\\");
				v = v.Replace(@"""", "\"");
			}
			return v;
		}
	}
}
