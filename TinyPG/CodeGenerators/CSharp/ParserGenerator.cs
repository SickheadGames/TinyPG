using System.Collections.Generic;
using System.Text;
using System.IO;
using TinyPG.Compiler;
using System;

namespace TinyPG.CodeGenerators.CSharp
{
	public class ParserGenerator : BaseGenerator, ICodeGenerator
	{
		internal ParserGenerator()
			: base("Parser.cs")
		{
		}

		public string Generate(Grammar Grammar, bool Debug)
		{
			if (string.IsNullOrEmpty(Grammar.GetTemplatePath()))
				return null;

			// generate the parser file
			StringBuilder parsers = new StringBuilder();
			string parser = File.ReadAllText(Grammar.GetTemplatePath() + templateName);

			// build non terminal tokens
			foreach (NonTerminalSymbol s in Grammar.GetNonTerminals())
			{
				string method = GenerateParseMethod(s);
				parsers.Append(method);
			}

			parser = parser.Replace(@"<%SourceFilename%>", Grammar.SourceFilename);
			if (Debug)
			{
				parser = parser.Replace(@"<%Namespace%>", "TinyPG.Debug");
				parser = parser.Replace(@"<%IParser%>", " : TinyPG.Debug.IParser");
				parser = parser.Replace(@"<%IParseTree%>", "TinyPG.Debug.IParseTree");
				parser = parser.Replace(@"<%ParserCustomCode%>", Grammar.Directives["Parser"]["CustomCode"]);
			}
			else
			{
				parser = parser.Replace(@"<%Namespace%>", Grammar.Directives["TinyPG"]["Namespace"]);
				parser = parser.Replace(@"<%IParser%>", "");
				parser = parser.Replace(@"<%IParseTree%>", "ParseTree");
				parser = parser.Replace(@"<%ParserCustomCode%>", Grammar.Directives["Parser"]["CustomCode"]);
			}

			parser = parser.Replace(@"<%ParseNonTerminals%>", parsers.ToString());
			return parser;
		}

		// generates the method header and body
		private string GenerateParseMethod(NonTerminalSymbol s)
		{
			string Indent2 = IndentTabs(2);
			string Indent3 = IndentTabs(3);
			StringBuilder sb = new StringBuilder();
			sb.AppendLine(Indent2 + "private void Parse" + s.Name + "(ParseNode parent)" + Helper.AddComment("NonTerminalSymbol: " + s.Name));
			sb.AppendLine(Indent2 + "{");
			sb.AppendLine(Indent3 + "Token tok;");
			sb.AppendLine(Indent3 + "ParseNode n;");
			sb.AppendLine(Indent3 + "bool found;");
			sb.AppendLine(Indent3 + "ParseNode node = parent.CreateNode(scanner.GetToken(TokenType." + s.Name + "), \"" + s.Name + "\");");
			sb.AppendLine(Indent3 + "parent.Nodes.Add(node);");
			sb.AppendLine("");

			if (s.Rules.Count == 1)
				sb.AppendLine(GenerateProductionRuleCode(s.Rules, 0, 3));
			else
				throw new Exception("Internal error");

			sb.AppendLine(Indent3 + "parent.Token.UpdateRange(node.Token);");
			sb.AppendLine(Indent2 + "}" + Helper.AddComment("NonTerminalSymbol: " + s.Name));
			sb.AppendLine();
			return sb.ToString();
		}

		// generates the rule logic inside the method body
		private string GenerateProductionRuleCode(Rules rules, int index, int indent)
		{
			Rule r = rules[index];
			Symbols firsts = null;
			Symbols firstsExtended = null;
			StringBuilder sb = new StringBuilder();
			string Indent = IndentTabs(indent);
			switch (r.Type)
			{
				case RuleType.Terminal:
					// expecting terminal, so scan it.
					sb.AppendLine(Indent + "tok = scanner.Scan(TokenType." + r.Symbol.Name + ");" + Helper.AddComment("Terminal Rule: " + r.Symbol.Name));
					sb.AppendLine(Indent + "n = node.CreateNode(tok, tok.ToString() );");
					sb.AppendLine(Indent + "node.Token.UpdateRange(tok);");
					sb.AppendLine(Indent + "node.Nodes.Add(n);");
					sb.AppendLine(Indent + "if (tok.Type != TokenType." + r.Symbol.Name + ") {");
					sb.AppendLine(Indent + IndentString + "tree.Errors.Add(new ParseError(\"Unexpected token '\" + tok.Text.Replace(\"\\n\", \"\") + \"' found. Expected \" + TokenType." + r.Symbol.Name + ".ToString(), 0x1001, tok));");
					sb.AppendLine(Indent + IndentString + "return;");
					sb.AppendLine(Indent + "}");
					break;
				case RuleType.NonTerminal:
					sb.AppendLine(Indent + "Parse" + r.Symbol.Name + "(node);" + Helper.AddComment("NonTerminal Rule: " + r.Symbol.Name));
					break;
				case RuleType.Concat:

					for (int i = 0; i < r.Rules.Count; i++)
					{
						sb.AppendLine();
						sb.AppendLine(Indent + Helper.AddComment("Concat Rule"));
						sb.Append(GenerateProductionRuleCode(r.Rules, i, indent));
					}
					break;
				case RuleType.ZeroOrMore:
					firsts = r.GetFirstTerminals();
					firstsExtended = CollectExpectedTokens(rules, index + 1);
					firstsExtended.AddRange(firsts);
					sb.Append(Indent + "tok = scanner.LookAhead(");
					AppendTokenList(firsts, sb);
					sb.AppendLine(");" + Helper.AddComment("ZeroOrMore Rule"));

					sb.Append(Indent + "while (");
					AppendTokenCondition(firsts, sb, Indent);
					sb.AppendLine(")");
					sb.AppendLine(Indent + "{");

					for (int i = 0; i < r.Rules.Count; i++)
					{
						sb.Append(GenerateProductionRuleCode(r.Rules, i, indent + 1));
					}

					sb.Append(Indent + "tok = scanner.LookAhead(");
					AppendTokenList(firstsExtended, sb);
					sb.AppendLine(");" + Helper.AddComment("ZeroOrMore Rule"));
					sb.AppendLine(Indent + "}");
					break;
				case RuleType.OneOrMore:
					sb.AppendLine(Indent + "found = false;");
					sb.AppendLine(Indent + "do {" + Helper.AddComment("OneOrMore Rule"));

					for (int i = 0; i < r.Rules.Count; i++)
					{
						sb.Append(GenerateProductionRuleCode(r.Rules, i, indent + 1));
					}

					firsts = r.GetFirstTerminals();
					firstsExtended = CollectExpectedTokens(rules, index + 1);
					firstsExtended.AddRange(firsts);
					sb.AppendLine(Indent + IndentString + "if(!found) {");
					sb.Append(Indent + IndentString + IndentString + "tok = scanner.LookAhead(");
					AppendTokenList(firsts, sb);
					sb.AppendLine(");" + Helper.AddComment("OneOrMore Rule"));
					sb.AppendLine(Indent + IndentString + "found = true;");
					sb.AppendLine(Indent + IndentString + "} else {");
					sb.Append(Indent + IndentString + IndentString + "tok = scanner.LookAhead(");
					AppendTokenList(firstsExtended, sb);
					sb.AppendLine(");" + Helper.AddComment("OneOrMore Rule"));
					sb.AppendLine(Indent + IndentString + "}");
					sb.Append(Indent + "} while (");
					AppendTokenCondition(firsts, sb, Indent);
					sb.AppendLine(");" + Helper.AddComment("OneOrMore Rule"));
					break;
				case RuleType.Option:
					firsts = r.GetFirstTerminals();
					sb.Append(Indent + "tok = scanner.LookAhead(");
					AppendTokenList(firsts, sb);
					sb.AppendLine(");" + Helper.AddComment("Option Rule"));

					sb.Append(Indent + "if (");
					AppendTokenCondition(firsts, sb, Indent);
					sb.AppendLine(")");
					sb.AppendLine(Indent + "{");

					for (int i = 0; i < r.Rules.Count; i++)
					{
						sb.Append(GenerateProductionRuleCode(r.Rules, i, indent + 1));
					}
					sb.AppendLine(Indent + "}");
					break;
				case RuleType.Choice:
					firsts = r.GetFirstTerminals();
					sb.Append(Indent + "tok = scanner.LookAhead(");
					var tokens = new List<string>();
					AppendTokenList(firsts, sb, tokens);
					string expectedTokens;
					if (tokens.Count == 1)
						expectedTokens = tokens[0];
					else if (tokens.Count == 2)
						expectedTokens = tokens[0] + " or " + tokens[1];
					else
					{
						expectedTokens = string.Join(", ", tokens.GetRange(0, tokens.Count - 1).ToArray());
						expectedTokens += ", or " + tokens[tokens.Count - 1];
					}
					sb.AppendLine(");" + Helper.AddComment("Choice Rule"));

					sb.AppendLine(Indent + "switch (tok.Type)");
					sb.AppendLine(Indent + "{" + Helper.AddComment("Choice Rule"));
					for (int i = 0; i < r.Rules.Count; i++)
					{
						foreach (TerminalSymbol s in r.Rules[i].GetFirstTerminals())
						{
							sb.AppendLine(Indent + IndentString + "case TokenType." + s.Name + ":");
						}
						sb.Append(GenerateProductionRuleCode(r.Rules, i, indent + 2));
						sb.AppendLine(Indent + IndentString + IndentString + "break;");
					}
					sb.AppendLine(Indent + IndentString + "default:");
					sb.AppendLine(Indent + IndentString + IndentString + "tree.Errors.Add(new ParseError(\"Unexpected token '\" + tok.Text.Replace(\"\\n\", \"\") + \"' found. Expected " + expectedTokens + ".\", 0x0002, tok));");
					sb.AppendLine(Indent + IndentString + IndentString + "break;");
					sb.AppendLine(Indent + "}" + Helper.AddComment("Choice Rule"));
					break;
				default:
					break;
			}
			return sb.ToString();
		}

		private Symbols CollectExpectedTokens(Rules rules, int index)
		{
			var symbols = new Symbols();
			for (int i = index; i < rules.Count; i++)
			{
				rules[i].DetermineFirstTerminals(symbols);
				if (rules[i].Type != RuleType.ZeroOrMore &&
					rules[i].Type != RuleType.Option)
					break;
			}
			return symbols;
		}

		private void AppendTokenList(Symbols symbols, StringBuilder sb, List<string> tokenNames = null)
		{
			int i = 0;
			foreach (TerminalSymbol s in symbols)
			{
				if (i == 0)
					sb.Append("TokenType." + s.Name);
				else
					sb.Append(", TokenType." + s.Name);
				i++;
				if (tokenNames != null)
					tokenNames.Add(s.Name);
			}
		}

		private void AppendTokenCondition(Symbols symbols, StringBuilder sb, string indent)
		{
			for (int i = 0; i < symbols.Count; i++)
			{
				TerminalSymbol s = (TerminalSymbol)symbols[i];
				if (i == 0)
					sb.Append("tok.Type == TokenType." + s.Name);
				else
					sb.Append(Environment.NewLine + indent + "    || tok.Type == TokenType." + s.Name);
			}

		}

		// replaces tabs by spaces, so outlining is more consistent
		public static string IndentTabs(int indent)
		{
			string t = "";
			for (int i = 0; i < indent; i++)
				t += IndentString;

			return t;
		}

		public static string IndentString = "    ";
	}
}
