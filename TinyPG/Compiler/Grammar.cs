// Copyright 2008 - 2010 Herre Kuijpers - <herre.kuijpers@gmail.com>
//
// This source file(s) may be redistributed, altered and customized
// by any means PROVIDING the authors name and all copyright
// notices remain intact.
// THIS SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED. USE IT AT YOUR OWN RISK. THE AUTHOR ACCEPTS NO
// LIABILITY FOR ANY DATA DAMAGE/LOSS THAT THIS PRODUCT MAY CAUSE.
//-----------------------------------------------------------------------
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;
using System.Globalization;

namespace TinyPG.Compiler
{
    public class Directives : List<Directive>
    {
        public bool Exists(Directive directive)
        {
            return this.Exists(new Predicate<Directive>(delegate(Directive d) { return d.Name == directive.Name; }));
        }

        public Directive Find(string name)
        {
            return this.Find(delegate(Directive d) { return d.Name == name; });
        }

        public Directive this[string name]
        {
            get { return Find(name); }
        }
    }

    public class Directive : Dictionary<string, string>
    {
        public Directive(string name)
        {
            Name = name;
        }

        public string Name { get; set; }
    }

    public class Grammar
    {
        /// <summary>
        /// represents all terminal and nonterminal symbols in the grammar
        /// </summary>
        public Symbols Symbols { get; set; }

        /// <summary>
        /// corresponds to the symbols that will be skipped during parsing
        /// e.g. commenting codeblocks
        /// </summary>
        public Symbols SkipSymbols { get; set; }

        /// <summary>
        /// The special symbol used to alter the internal file and line
        /// number tracking for correct error reporting.
        /// </summary>
        public Symbol FileAndLine { get; set; }

        /// <summary>
        /// these are specific directives that should be applied to the grammar
        /// this can be meta data, or information on how code should be generated, e.g.
        /// <%@ Grammar Namespace="TinyPG" %> will generate code with namespace TinyPG.
        /// </summary>
        public Directives Directives { get; set; }

        public Grammar()
        {
            Symbols = new Symbols();
            SkipSymbols = new Symbols();
            Directives = new Directives();
        }

        public Symbols GetTerminals()
        {
            Symbols symbols = new Symbols();
            foreach (Symbol s in Symbols)
            {
                if (s is TerminalSymbol)
                    symbols.Add(s);
            }
            return symbols;
        }

        public Symbols GetNonTerminals()
        {
            Symbols symbols = new Symbols();
            foreach (Symbol s in Symbols)
            {
                if (s is NonTerminalSymbol)
                    symbols.Add(s);
            }
            return symbols;
        }

        /// <summary>
        /// Once the grammar terminals and nonterminal production rules have been defined
        /// use the Compile method to analyse and check the grammar semantics.
        /// </summary>
        public void Preprocess()
        {
            SetupDirectives();

            DetermineFirsts();

            //LookAheadTree LATree = DetermineLookAheadTree();
            //Symbols nts = GetNonTerminals();
            //NonTerminalSymbol n = (NonTerminalSymbol)nts[0];
            //TerminalSymbol t = (TerminalSymbol) n.FirstTerminals[0];

            //Symbols Follow = new Symbols();
            //t.Rule.DetermineFirstTerminals(Follow, 1);
        }

        /*
        private LookAheadTree DetermineLookAheadTree()
        {
            LookAheadTree tree = new LookAheadTree();
            foreach (NonTerminalSymbol nts in GetNonTerminals())
            {
                tree.NonTerminal = nts;
                nts.DetermineLookAheadTree(tree);
                //nts.DetermineFirstTerminals();
                tree.PrintTree();
            }
            return tree;
        }
        */

        private void DetermineFirsts()
        {
            foreach (NonTerminalSymbol nts in GetNonTerminals())
            {
                nts.DetermineFirstTerminals();
            }
        }

        private void SetupDirectives()
        {

            Directive d = Directives.Find("TinyPG");
            if (d == null)
            {
                d = new Directive("TinyPG");
                Directives.Insert(0, d);
            }
            if (!d.ContainsKey("Namespace"))
                d["Namespace"] = "TinyPG"; // set default namespace
            if (!d.ContainsKey("OutputPath"))
                d["OutputPath"] = "./"; // write files to current path
            if (!d.ContainsKey("Language"))
                d["Language"] = "C#"; // set default language
            if (!d.ContainsKey("TemplatePath"))
            {
                switch (d["Language"].ToLower(CultureInfo.InvariantCulture))
                {
                    // set the default templates directory
                    case "visualbasic":
                    case "vbnet":
                    case "vb.net":
                    case "vb":
                        d["TemplatePath"] = AppDomain.CurrentDomain.BaseDirectory + @"Templates\VB\";
                        break;
                    default:
                        d["TemplatePath"] = AppDomain.CurrentDomain.BaseDirectory + @"Templates\C#\";
                        break;
                }
            }

            d = Directives.Find("Parser");
            if (d == null)
            {
                d = new Directive("Parser");
                Directives.Insert(1, d);
            }
            if (!d.ContainsKey("Generate"))
                d["Generate"] = "True"; // generate parser by default

            d = Directives.Find("Scanner");
            if (d == null)
            {
                d = new Directive("Scanner");
                Directives.Insert(1, d);
            }
            if (!d.ContainsKey("Generate"))
                d["Generate"] = "True"; // generate scanner by default

            d = Directives.Find("ParseTree");
            if (d == null)
            {
                d = new Directive("ParseTree");
                Directives.Add(d);
            }
            if (!d.ContainsKey("Generate"))
                d["Generate"] = "True"; // generate parsetree by default

            d = Directives.Find("TextHighlighter");
            if (d == null)
            {
                d = new Directive("TextHighlighter");
                Directives.Add(d);
            }
            if (!d.ContainsKey("Generate"))
                d["Generate"] = "False"; // do NOT generate a text highlighter by default
        }

        public string GetTemplatePath()
        {
            string folder = AppDomain.CurrentDomain.BaseDirectory;
            string pathout = Directives["TinyPG"]["TemplatePath"];
            if (Path.IsPathRooted(pathout))
                folder = Path.GetFullPath(pathout);
            else
                folder = Path.GetFullPath(Path.Combine(folder, pathout));


            DirectoryInfo dir = new DirectoryInfo(folder + @"\");
            if (dir.Exists)
                return folder;
            else
                return null;
        }

        public string GetOutputPath()
        {
            string folder = Directory.GetCurrentDirectory() + @"\";
            string pathout = Directives["TinyPG"]["OutputPath"];
            if (Path.IsPathRooted(pathout))
                folder = Path.GetFullPath(pathout);
            else
                folder = Path.GetFullPath(folder + pathout);


            DirectoryInfo dir = new DirectoryInfo(folder + @"\");
            if (dir.Exists)
                return folder;
            else
                return null;
        }

        public string PrintGrammar()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("//Terminals:");
            foreach (Symbol s in GetTerminals())
            {
                Symbol skip = SkipSymbols.Find(s.Name);
                if (skip != null)
                    sb.Append("[Skip] ");
                sb.AppendLine(s.PrintProduction());
            }

            sb.AppendLine("\r\n//Production lines:");
            foreach (Symbol s in GetNonTerminals())
            {
                sb.AppendLine(s.PrintProduction());
            }
            return sb.ToString();
        }
    
        public string PrintFirsts()
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine("\r\n/*\r\nFirst symbols:");
            foreach (NonTerminalSymbol s in GetNonTerminals())
            {
                string firsts = s.Name + ": ";
                foreach (TerminalSymbol t in s.FirstTerminals)
                    firsts += t.Name + ' ';
                sb.AppendLine(firsts);
            }

            sb.AppendLine("\r\nSkip symbols: ");
            string skips = "";
            foreach (TerminalSymbol s in SkipSymbols)
            {
                skips += s.Name + " ";
            }
            sb.AppendLine(skips);
            sb.AppendLine("*/");
            return sb.ToString();

        }

    }
}