using System;
using System.Collections.Generic;
using System.Text;
using System.Globalization;
using System.CodeDom.Compiler;

namespace TinyPG.CodeGenerators
{
    public enum SupportedLanguage
    {
        CSharp = 0, // default
        VBNet = 1,
    }

    public static class CodeGeneratorFactory
    {
        public static SupportedLanguage GetSupportedLanguage(string language)
        {
            switch (language.ToLower(CultureInfo.InvariantCulture))
            {
                // set the default templates directory
                case "visualbasic":
                case "vbnet":
                case "vb.net":
                case "vb":
                    return SupportedLanguage.VBNet;
                default: // c# is default language
                    return SupportedLanguage.CSharp;
            }
        }

        public static ICodeGenerator CreateGenerator(string generator, string language)
        {
            switch (GetSupportedLanguage(language))
            {
                // set the default templates directory
                case SupportedLanguage.VBNet:
                    switch (generator)
                    {
                        case "Parser":
                            return new VBNet.ParserGenerator();
                        case "Scanner":
                            return new VBNet.ScannerGenerator();
                        case "ParseTree":
                            return new VBNet.ParseTreeGenerator();
                        case "TextHighlighter":
                            return new VBNet.TextHighlighterGenerator();
                    }
                    break;
                default: // c# is default language
                    switch (generator)
                    {
                        case "Parser":
                            return new CSharp.ParserGenerator();
                        case "Scanner":
                            return new CSharp.ScannerGenerator();
                        case "ParseTree":
                            return new CSharp.ParseTreeGenerator();
                        case "TextHighlighter":
                            return new CSharp.TextHighlighterGenerator();
                    }
                    break;
            }
            return null; // codegenerator was not found
        }

        public static CodeDomProvider CreateCodeDomProvider(string language)
        {
            switch (language.ToLower(CultureInfo.InvariantCulture))
            {
                // set the default templates directory
                case "visualbasic":
                case "vbnet":
                case "vb.net":
                case "vb":
                    return new Microsoft.VisualBasic.VBCodeProvider();
                default:
                    return new Microsoft.CSharp.CSharpCodeProvider(new Dictionary<string, string> { { "CompilerVersion", "v3.5" } });
            }
        }
    }
}
