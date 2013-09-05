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
using System.IO;
using System.Windows.Forms;
using TinyPG.Compiler;
using System.Text;

namespace TinyPG
{
    public class Program
    {
        public enum ExitCode : int
        {
            Success = 0,
            InvalidFilename = 1,
            UnknownError = 10
        }

        [STAThread]
        public static int Main(string[] args)
        {
            if (args.Length > 0)
            {
                string GrammarFilePath = Path.GetFullPath(args[0]);
                StringBuilder output = new StringBuilder(string.Empty);
                if (!File.Exists(GrammarFilePath))
                {
                    output.Append("Specified file " + GrammarFilePath + " does not exists");
                    Console.WriteLine(output.ToString());
                    return (int)ExitCode.InvalidFilename;
                }

                //As stated in documentation current directory is the one of the TPG file.
                Directory.SetCurrentDirectory(Path.GetDirectoryName(GrammarFilePath));

                DateTime starttimer = DateTime.Now;

                Program prog = new Program(ManageParseError, output);
                Grammar grammar = prog.ParseGrammar(System.IO.File.ReadAllText(GrammarFilePath), Path.GetFileName(GrammarFilePath));

                if (grammar != null && prog.BuildCode(grammar, new TinyPG.Compiler.Compiler()))
                {
                    TimeSpan span = DateTime.Now.Subtract(starttimer);
                    output.AppendLine("Compilation successful in " + span.TotalMilliseconds + "ms.");
                }

                Console.WriteLine(output.ToString());
            }
            else
            {
                Application.ThreadException += new System.Threading.ThreadExceptionEventHandler(Application_ThreadException);
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                Application.Run(new MainForm());
            }
            return (int)ExitCode.Success;
        }

        static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
        {
            MessageBox.Show("An unhandled exception occured: " + e.Exception.Message);
        }

        public delegate void OnParseErrorDelegate(ParseTree tree, StringBuilder output);
        private OnParseErrorDelegate parseErrorDelegate;
        private StringBuilder output;
        public StringBuilder Output { get { return this.output; } }

        public Program(OnParseErrorDelegate parseErrorDelegate, StringBuilder output)
        {
            this.parseErrorDelegate = parseErrorDelegate;
            this.output = output;
        }

        public Grammar ParseGrammar(string input, string grammarFile)
        {
            Grammar grammar = null;
            Scanner scanner = new Scanner();
            Parser parser = new Parser(scanner);

            ParseTree tree = parser.Parse(input, grammarFile, new GrammarTree());

            if (tree.Errors.Count > 0)
            {
                this.parseErrorDelegate(tree, this.output);
            }
            else
            {
                grammar = (Grammar)tree.Eval();
                grammar.Preprocess();

                if (tree.Errors.Count == 0)
                {
                    this.output.AppendLine(grammar.PrintGrammar());
                    this.output.AppendLine(grammar.PrintFirsts());

                    this.output.AppendLine("Parse successful!\r\n");
                }
            }
            return grammar;
        }


        public bool BuildCode(Grammar grammar, TinyPG.Compiler.Compiler compiler)
        {

            this.output.AppendLine("Building code...");
            compiler.Compile(grammar);
            if (!compiler.IsCompiled)
            {
                foreach (string err in compiler.Errors)
                    this.output.AppendLine(err);
                this.output.AppendLine("Compilation contains errors, could not compile.");
            }

            new GeneratedFilesWriter(grammar).Generate(false);

            return compiler.IsCompiled;
        }

        protected static void ManageParseError(ParseTree tree, StringBuilder output)
        {
            foreach (ParseError error in tree.Errors)
                output.AppendLine(string.Format("({0},{1}): {2}", error.Line, error.Column, error.Message));

            output.AppendLine("Semantic errors in grammar found.");
        }

    }
}
