using System.Text;
using System.IO;
using TinyPG.Compiler;

namespace TinyPG.CodeGenerators.VBNet
{
    public class ParserGenerator : BaseGenerator, ICodeGenerator
    {
        internal ParserGenerator()
            : base("Parser.vb")
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

            if (Debug)
            {
                parser = parser.Replace(@"<%Imports%>", "Imports TinyPG.Debug");
                parser = parser.Replace(@"<%Namespace%>", "TinyPG.Debug");
                parser = parser.Replace(@"<%IParser%>", "\r\n        Implements IParser\r\n");
                parser = parser.Replace(@"<%IParseTree%>", "IParseTree");
            }
            else
            {
                parser = parser.Replace(@"<%Imports%>", "");
                parser = parser.Replace(@"<%Namespace%>", Grammar.Directives["TinyPG"]["Namespace"]);
                parser = parser.Replace(@"<%IParser%>", "");
                parser = parser.Replace(@"<%IParseTree%>", "ParseTree");
            }

            parser = parser.Replace(@"<%ParseNonTerminals%>", parsers.ToString());
            return parser;
        }

        // generates the method header and body
        private string GenerateParseMethod(NonTerminalSymbol s)
        {

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("        Private Sub Parse" + s.Name + "(ByVal parent As ParseNode)" + Helper.AddComment("'", "NonTerminalSymbol: " + s.Name));
            sb.AppendLine("            Dim tok As Token");
            sb.AppendLine("            Dim n As ParseNode");
            sb.AppendLine("            Dim node As ParseNode = parent.CreateNode(m_scanner.GetToken(TokenType." + s.Name + "), \"" + s.Name + "\")");
            sb.AppendLine("            parent.Nodes.Add(node)");
            sb.AppendLine("");

            foreach (Rule rule in s.Rules)
            {
                sb.AppendLine(GenerateProductionRuleCode(s.Rules[0], 3));
            }

            sb.AppendLine("            parent.Token.UpdateRange(node.Token)");
            sb.AppendLine("        End Sub" + Helper.AddComment("'", "NonTerminalSymbol: " + s.Name));
            sb.AppendLine();
            return sb.ToString();
        }

        // generates the rule logic inside the method body
        private string GenerateProductionRuleCode(Rule r, int indent)
        {
            int i = 0;
            Symbols firsts = null;
            StringBuilder sb = new StringBuilder();
            string Indent = IndentTabs(indent);

            switch (r.Type)
            {
                case RuleType.Terminal:
                    // expecting terminal, so scan it.
                    sb.AppendLine(Indent + "tok = m_scanner.Scan(TokenType." + r.Symbol.Name + ")" + Helper.AddComment("'", "Terminal Rule: " + r.Symbol.Name));
                    sb.AppendLine(Indent + "n = node.CreateNode(tok, tok.ToString() )");
                    sb.AppendLine(Indent + "node.Token.UpdateRange(tok)");
                    sb.AppendLine(Indent + "node.Nodes.Add(n)");
                    sb.AppendLine(Indent + "If tok.Type <> TokenType." + r.Symbol.Name + " Then");
                    sb.AppendLine(Indent + "    m_tree.Errors.Add(New ParseError(\"Unexpected token '\" + tok.Text.Replace(\"\\n\", \"\") + \"' found. Expected \" + TokenType." + r.Symbol.Name + ".ToString(), &H1001, tok))");
                    sb.AppendLine(Indent + "    Return\r\n");
                    sb.AppendLine(Indent + "End If\r\n");
                    break;
                case RuleType.NonTerminal:
                    sb.AppendLine(Indent + "Parse" + r.Symbol.Name + "(node)" + Helper.AddComment("'", "NonTerminal Rule: " + r.Symbol.Name));
                    break;
                case RuleType.Concat:
                    foreach (Rule rule in r.Rules)
                    {
                        sb.AppendLine();
                        sb.AppendLine(Indent + Helper.AddComment("'", "Concat Rule"));
                        sb.Append(GenerateProductionRuleCode(rule, indent));
                    }
                    break;
                case RuleType.ZeroOrMore:
                    firsts = r.GetFirstTerminals();
                    i = 0;
                    sb.Append(Indent + "tok = m_scanner.LookAhead(");
                    foreach (TerminalSymbol s in firsts)
                    {
                        if (i == 0)
                            sb.Append("TokenType." + s.Name);
                        else
                            sb.Append(", TokenType." + s.Name);
                        i++;
                    }
                    sb.AppendLine(")" + Helper.AddComment("'", "ZeroOrMore Rule"));

                    i = 0;
                    foreach (TerminalSymbol s in firsts)
                    {
                        if (i == 0)
                            sb.Append(Indent + "While tok.Type = TokenType." + s.Name);
                        else
                            sb.Append(" Or tok.Type = TokenType." + s.Name);
                        i++;
                    }
                    sb.AppendLine("");


                    foreach (Rule rule in r.Rules)
                    {
                        sb.Append(GenerateProductionRuleCode(rule, indent + 1));
                    }

                    i = 0;
                    sb.Append(Indent + "tok = m_scanner.LookAhead(");
                    foreach (TerminalSymbol s in firsts)
                    {
                        if (i == 0)
                            sb.Append("TokenType." + s.Name);
                        else
                            sb.Append(", TokenType." + s.Name);
                        i++;
                    }
                    sb.AppendLine(")" + Helper.AddComment("'", "ZeroOrMore Rule"));
                    sb.AppendLine(Indent + "End While");
                    break;
                case RuleType.OneOrMore:
                    sb.AppendLine(Indent + "Do" + Helper.AddComment("'", "OneOrMore Rule"));

                    foreach (Rule rule in r.Rules)
                    {
                        sb.Append(GenerateProductionRuleCode(rule, indent + 1));
                    }

                    i = 0;
                    firsts = r.GetFirstTerminals();
                    sb.Append(Indent + "    tok = m_scanner.LookAhead(");
                    foreach (TerminalSymbol s in firsts)
                    {
                        if (i == 0)
                            sb.Append("TokenType." + s.Name);
                        else
                            sb.Append(", TokenType." + s.Name);
                        i++;
                    }
                    sb.AppendLine(")" + Helper.AddComment("'", "OneOrMore Rule"));

                    i = 0;
                    foreach (TerminalSymbol s in r.GetFirstTerminals())
                    {
                        if (i == 0)
                            sb.Append(Indent + "Loop While tok.Type = TokenType." + s.Name);
                        else
                            sb.Append(" Or tok.Type = TokenType." + s.Name);
                        i++;
                    }
                    sb.AppendLine("" + Helper.AddComment("'", "OneOrMore Rule"));
                    break;
                case RuleType.Option:
                    i = 0;
                    firsts = r.GetFirstTerminals();
                    sb.Append(Indent + "tok = m_scanner.LookAhead(");
                    foreach (TerminalSymbol s in firsts)
                    {
                        if (i == 0)
                            sb.Append("TokenType." + s.Name);
                        else
                            sb.Append(", TokenType." + s.Name);
                        i++;
                    }
                    sb.AppendLine(")" + Helper.AddComment("'", "Option Rule"));

                    i = 0;
                    foreach (TerminalSymbol s in r.GetFirstTerminals())
                    {
                        if (i == 0)
                            sb.Append(Indent + "If tok.Type = TokenType." + s.Name);
                        else
                            sb.Append(" Or tok.Type = TokenType." + s.Name);
                        i++;
                    }
                    sb.AppendLine(" Then");

                    foreach (Rule rule in r.Rules)
                    {
                        sb.Append(GenerateProductionRuleCode(rule, indent + 1));
                    }
                    sb.AppendLine(Indent + "End If");
                    break;
                case RuleType.Choice:
                    i = 0;
                    firsts = r.GetFirstTerminals();
                    sb.Append(Indent + "tok = m_scanner.LookAhead(");
                    foreach (TerminalSymbol s in firsts)
                    {
                        if (i == 0)
                            sb.Append("TokenType." + s.Name);
                        else
                            sb.Append(", TokenType." + s.Name);
                        i++;
                    }
                    sb.AppendLine(")" + Helper.AddComment("'", "Choice Rule"));

                    sb.AppendLine(Indent + "Select Case tok.Type");
                    sb.AppendLine(Indent + "" + Helper.AddComment("'", "Choice Rule"));
                    foreach (Rule rule in r.Rules)
                    {
                        foreach (TerminalSymbol s in rule.GetFirstTerminals())
                        {
                            sb.AppendLine(Indent + "    Case TokenType." + s.Name + "");
                            sb.Append(GenerateProductionRuleCode(rule, indent + 2));
                        }
                    }
                    sb.AppendLine(Indent + "    Case Else");
                    sb.AppendLine(Indent + "        m_tree.Errors.Add(new ParseError(\"Unexpected token '\" + tok.Text.Replace(\"\\n\", \"\") + \"' found.\", &H0002, tok))");
                    sb.AppendLine(Indent + "        Exit Select");
                    sb.AppendLine(Indent + "End Select" + Helper.AddComment("'", "Choice Rule"));
                    break;
                default:
                    break;
            }
            return sb.ToString();
        }

        // replaces tabs by spaces, so outlining is more consistent
        public static string IndentTabs(int indent)
        {
            string t = "";
            for (int i = 0; i < indent; i++)
                t += "    ";

            return t;
        }
    }
}
