using System.Text;
using System.IO;
using TinyPG.Compiler;
using System.Text.RegularExpressions;

namespace TinyPG.CodeGenerators.VBNet
{
    public class ParseTreeGenerator : BaseGenerator, ICodeGenerator
    {
        internal ParseTreeGenerator()
            : base("ParseTree.vb")
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
                evalsymbols.AppendLine("                Case TokenType." + s.Name + "");
                evalsymbols.AppendLine("                    Value = Eval" + s.Name + "(tree, paramlist)");
                evalsymbols.AppendLine("                    Exit Select");

                evalmethods.AppendLine("        Protected Overridable Function Eval" + s.Name + "(ByVal tree As ParseTree, ByVal ParamArray paramlist As Object()) As Object");
                if (s.CodeBlock != null)
                {
                    // paste user code here
                    evalmethods.AppendLine(FormatCodeBlock(s as NonTerminalSymbol));
                }
                else
                {
                    if (s.Name == "Start") // return a nice warning message from root object.
                        evalmethods.AppendLine("            Return \"Could not interpret input; no semantics implemented.\"");
                    else
                        evalmethods.AppendLine("            Throw New NotImplementedException()");

                    // otherwise simply not implemented!
                }
                evalmethods.AppendLine("        End Function\r\n");
            }

            if (Debug)
            {
                parsetree = parsetree.Replace(@"<%Imports%>", "Imports TinyPG.Debug");
                parsetree = parsetree.Replace(@"<%Namespace%>", "TinyPG.Debug");
                parsetree = parsetree.Replace(@"<%IParseTree%>", "\r\n        Implements IParseTree");
                parsetree = parsetree.Replace(@"<%IParseNode%>", "\r\n        Implements IParseNode\r\n");
                parsetree = parsetree.Replace(@"<%ParseError%>", "\r\n        Implements IParseError\r\n");
                parsetree = parsetree.Replace(@"<%ParseErrors%>", "List(Of IParseError)");

                string itoken = "        Public ReadOnly Property IToken() As IToken Implements IParseNode.IToken\r\n"
                                + "            Get\r\n"
                                + "                Return DirectCast(Token, IToken)\r\n"
                                + "            End Get\r\n"
                                + "        End Property\r\n";

                parsetree = parsetree.Replace(@"<%ITokenGet%>", itoken);


                parsetree = parsetree.Replace(@"<%ImplementsIParseTreePrintTree%>", " Implements IParseTree.PrintTree");
                parsetree = parsetree.Replace(@"<%ImplementsIParseTreeEval%>", " Implements IParseTree.Eval");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorCode%>", " Implements IParseError.Code");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorLine%>", " Implements IParseError.Line");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorColumn%>", " Implements IParseError.Column");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorPosition%>", " Implements IParseError.Position");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorLength%>", " Implements IParseError.Length");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorMessage%>", " Implements IParseError.Message");

                string inodes = "        Public Shared Function Node2INode(ByVal node As ParseNode) As IParseNode\r\n"
                                    + "            Return DirectCast(node, IParseNode)\r\n"
                                    + "        End Function\r\n\r\n"
                                    + "        Public ReadOnly Property INodes() As List(Of IParseNode) Implements IParseNode.INodes\r\n"
                                    + "            Get\r\n"
                                    + "                Return Nodes.ConvertAll(Of IParseNode)(New Converter(Of ParseNode, IParseNode)(AddressOf Node2INode))\r\n"
                                    + "            End Get\r\n"
                                    + "        End Property\r\n";
                parsetree = parsetree.Replace(@"<%INodesGet%>", inodes);
                parsetree = parsetree.Replace(@"<%ImplementsIParseNodeText%>", " Implements IParseNode.Text");

            }
            else
            {
                parsetree = parsetree.Replace(@"<%Imports%>", "");
                parsetree = parsetree.Replace(@"<%Namespace%>", Grammar.Directives["TinyPG"]["Namespace"]);
                parsetree = parsetree.Replace(@"<%ParseError%>", "");
                parsetree = parsetree.Replace(@"<%ParseErrors%>", "List(Of ParseError)");
                parsetree = parsetree.Replace(@"<%IParseTree%>", "");
                parsetree = parsetree.Replace(@"<%IParseNode%>", "");
                parsetree = parsetree.Replace(@"<%ITokenGet%>", "");
                parsetree = parsetree.Replace(@"<%INodesGet%>", "");

                parsetree = parsetree.Replace(@"<%ImplementsIParseTreePrintTree%>", "");
                parsetree = parsetree.Replace(@"<%ImplementsIParseTreeEval%>", "");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorCode%>", "");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorLine%>", "");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorColumn%>", "");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorPosition%>", "");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorLength%>", "");
                parsetree = parsetree.Replace(@"<%ImplementsIParseErrorMessage%>", "");
                parsetree = parsetree.Replace(@"<%ImplementsIParseNodeText%>", "");
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
                    break; // error situation
                string indexer = "0";
                if (match.Groups["index"].Value.Length > 0)
                {
                    indexer = match.Groups["index"].Value;
                }

                string replacement = "Me.GetValue(tree, TokenType." + s.Name + ", " + indexer + ")";

                codeblock = codeblock.Substring(0, match.Captures[0].Index) + replacement + codeblock.Substring(match.Captures[0].Index + match.Captures[0].Length);
                match = var.Match(codeblock);
            }

            codeblock = "            " + codeblock.Replace("\n", "\r\n        ");
            return codeblock;
        }
    }

}
