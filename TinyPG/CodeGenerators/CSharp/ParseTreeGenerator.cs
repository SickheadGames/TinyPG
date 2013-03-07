using System.Text;
using System.IO;
using TinyPG.Compiler;
using System.Text.RegularExpressions;

namespace TinyPG.CodeGenerators.CSharp
{
    public class ParseTreeGenerator : BaseGenerator, ICodeGenerator
    {
        internal ParseTreeGenerator()
            : base("ParseTree.cs")
        {
        }

        public string Generate(Grammar Grammar, bool Debug)
        {
            if (string.IsNullOrEmpty(Grammar.GetTemplatePath()))
                return null;

            // copy the parse tree file (optionally)
            string parsetree = File.ReadAllText(Grammar.GetTemplatePath() + templateName);

            StringBuilder evalsymbols = new StringBuilder();
            StringBuilder evalmethods = new StringBuilder();

            // build non terminal tokens
            foreach (Symbol s in Grammar.GetNonTerminals())
            {
                evalsymbols.AppendLine("                case TokenType." + s.Name + ":");
                evalsymbols.AppendLine("                    Value = Eval" + s.Name + "(tree, paramlist);");
                //evalsymbols.AppendLine("                Value = Token.Text;");
                evalsymbols.AppendLine("                    break;");

                evalmethods.AppendLine("        protected virtual object Eval" + s.Name + "(ParseTree tree, params object[] paramlist)");
                evalmethods.AppendLine("        {");
                if (s.CodeBlock != null)
                {
                    // paste user code here
                    evalmethods.AppendLine(FormatCodeBlock(s as NonTerminalSymbol));
                }
                else
                {
                    if (s.Name == "Start") // return a nice warning message from root object.
                        evalmethods.AppendLine("            return \"Could not interpret input; no semantics implemented.\";");
                    else
                        evalmethods.AppendLine("            foreach (var node in Nodes)\r\n" +
                                               "                node.Eval(tree, paramlist);\r\n" +
                                               "            return null;");

                    // otherwise simply not implemented!
                }
                evalmethods.AppendLine("        }\r\n");
            }

            if (Debug)
            {
                parsetree = parsetree.Replace(@"<%Namespace%>", "TinyPG.Debug");
                parsetree = parsetree.Replace(@"<%ParseError%>", " : TinyPG.Debug.IParseError");
                parsetree = parsetree.Replace(@"<%ParseErrors%>", "List<TinyPG.Debug.IParseError>");
                parsetree = parsetree.Replace(@"<%IParseTree%>", ", TinyPG.Debug.IParseTree");
                parsetree = parsetree.Replace(@"<%IParseNode%>", " : TinyPG.Debug.IParseNode");
                parsetree = parsetree.Replace(@"<%ITokenGet%>", "public IToken IToken { get {return (IToken)Token;} }");

                string inodes = "public List<IParseNode> INodes {get { return nodes.ConvertAll<IParseNode>( new Converter<ParseNode, IParseNode>( delegate(ParseNode n) { return (IParseNode)n; })); }}\r\n\r\n";
                parsetree = parsetree.Replace(@"<%INodesGet%>", inodes);
            }
            else
            {
                parsetree = parsetree.Replace(@"<%Namespace%>", Grammar.Directives["TinyPG"]["Namespace"]);
                parsetree = parsetree.Replace(@"<%ParseError%>", "");
                parsetree = parsetree.Replace(@"<%ParseErrors%>", "List<ParseError>");
                parsetree = parsetree.Replace(@"<%IParseTree%>", "");
                parsetree = parsetree.Replace(@"<%IParseNode%>", "");
                parsetree = parsetree.Replace(@"<%ITokenGet%>", "");
                parsetree = parsetree.Replace(@"<%INodesGet%>", "");
            }

            parsetree = parsetree.Replace(@"<%EvalSymbols%>", evalsymbols.ToString());
            parsetree = parsetree.Replace(@"<%VirtualEvalMethods%>", evalmethods.ToString());

            return parsetree;
        }

        /// <summary>
        /// replaces $ variables with a c# statement
        /// the routine also implements some checks to see if $variables are matching with production symbols
        /// errors are added to the Error object.
        /// </summary>
        /// <param name="nts">non terminal and its production rule</param>
        /// <returns>a formated codeblock</returns>
        private string FormatCodeBlock(NonTerminalSymbol nts)
        {
            string codeblock = nts.CodeBlock;
            if (nts == null) return "";

            Regex var = new Regex(@"\$(?<var>[a-zA-Z_0-9]+)(\[(?<index>[^]]+)\])?", RegexOptions.Compiled);

            Symbols symbols = nts.DetermineProductionSymbols();


            Match match = var.Match(codeblock);
            while (match.Success)
            {
                Symbol s = symbols.Find(match.Groups["var"].Value);
                if (s == null)
                {
                    //TOD: handle error situation
                    //Errors.Add("Variable $" + match.Groups["var"].Value + " cannot be matched.");
                    break; // error situation
                }
                string indexer = "0";
                if (match.Groups["index"].Value.Length > 0)
                {
                    indexer = match.Groups["index"].Value;
                }

                string replacement = "this.GetValue(tree, TokenType." + s.Name + ", " + indexer + ")";

                codeblock = codeblock.Substring(0, match.Captures[0].Index) + replacement + codeblock.Substring(match.Captures[0].Index + match.Captures[0].Length);
                match = var.Match(codeblock);
            }

            codeblock = "            " + codeblock.Replace("\n", "\r\n        ");
            return codeblock;
        }
    }

}
