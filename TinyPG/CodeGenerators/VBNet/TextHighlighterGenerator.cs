using System;
using System.Text;
using System.IO;
using TinyPG.Compiler;

namespace TinyPG.CodeGenerators.VBNet
{
    public class TextHighlighterGenerator : BaseGenerator, ICodeGenerator
    {
        internal TextHighlighterGenerator()
            : base("TextHighlighter.vb")
        {
        }

        public string Generate(Grammar Grammar, bool Debug)
        {
            if (string.IsNullOrEmpty(Grammar.GetTemplatePath()))
                return null;

            string generatedtext = File.ReadAllText(Grammar.GetTemplatePath() + templateName);
            StringBuilder tokens = new StringBuilder();
            StringBuilder colors = new StringBuilder();

            int colorindex = 1;
            foreach (TerminalSymbol t in Grammar.GetTerminals())
            {
                if (!t.Attributes.ContainsKey("Color"))
                    continue;

                tokens.AppendLine(Helper.Indent(5) + "Case TokenType." + t.Name + ":");
                tokens.AppendLine(Helper.Indent(6) + @"sb.Append(""{{\cf" + colorindex + @" "")");
                tokens.AppendLine(Helper.Indent(6) + "Exit Select");

                int red = 0;
                int green = 0;
                int blue = 0;
                int len = t.Attributes["Color"].Length;
                if (len == 1)
                {
                    if (t.Attributes["Color"][0] is long)
                    {
                        int v = Convert.ToInt32(t.Attributes["Color"][0]);
                        red = (v >> 16) & 255;
                        green = (v >> 8) & 255;
                        blue = v & 255;
                    }
                }
                else if (len == 3)
                {
                    if (t.Attributes["Color"][0] is int || t.Attributes["Color"][0] is long)
                        red = Convert.ToInt32(t.Attributes["Color"][0]) & 255;
                    if (t.Attributes["Color"][1] is int || t.Attributes["Color"][1] is long)
                        green = Convert.ToInt32(t.Attributes["Color"][1]) & 255;
                    if (t.Attributes["Color"][2] is int || t.Attributes["Color"][2] is long)
                        blue = Convert.ToInt32(t.Attributes["Color"][2]) & 255;
                }

                colors.Append(String.Format(@"\red{0}\green{1}\blue{2};", red, green, blue));
                colorindex++;
            }

            generatedtext = generatedtext.Replace(@"<%HightlightTokens%>", tokens.ToString());
            generatedtext = generatedtext.Replace(@"<%RtfColorPalette%>", colors.ToString());

            if (Debug)
            {
                generatedtext = generatedtext.Replace(@"<%Namespace%>", "TinyPG.Debug");
            }
            else
            {
                generatedtext = generatedtext.Replace(@"<%Namespace%>", Grammar.Directives["TinyPG"]["Namespace"]);
            }

            return generatedtext;
        }

    }
}
