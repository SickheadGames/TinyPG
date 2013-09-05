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
using System.Windows.Forms;
using System.Text;
using System.IO;
using System.Xml;
using TinyPG.Compiler;
using TinyPG.Debug;
using TinyPG.Controls;
using System.Globalization;
using TinyPG.CodeGenerators;

namespace TinyPG
{
    public partial class MainForm : Form
    {
        #region member declarations
        // the compiler used to evaluate the input
        private TinyPG.Compiler.Compiler compiler;
        Grammar grammar;


        // indicates if text/grammar has changed
        private bool IsDirty;

        // the current file the user is editing
        private string GrammarFile;

        // manages docking and floating of panels
        private DockExtender DockExtender;
        // used to make the input pane floating/draggable
        IFloaty inputFloaty;
        // used to make the output pane floating/draggable
        IFloaty outputFloaty;

        // marks erronious text with little waves
        // this is used in combination with the checker
        private TextMarker marker;
        // checks the syntax/semantics while editing on a seperate thread
        private SyntaxChecker checker;

        // timer that will fire if the changed text requires evaluating
        private Timer TextChangedTimer;

        // responsible for text highlighting
        private Highlighter.TextHighlighter textHighlighter;

        // scanner to be used by the highlighter, declare here
        // so we can modify the scanner properies at runtime if needed
        private Highlighter.Scanner highlighterScanner;

        // autocomplete popup form
        private AutoComplete codecomplete;
        private AutoComplete directivecomplete;

        // keep this event handler reference in a seperate object, so it can be
        // unregistered on closing. this is required because the checker runs on a seperate thread
        EventHandler syntaxUpdateChecker;

        #endregion

        #region Initialization
        public MainForm()
        {

            InitializeComponent();
            IsDirty = false;
            compiler = null;
            GrammarFile = null;

            this.Disposed += new EventHandler(MainForm_Disposed);
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            headerEvaluator.Activate(textInput);
            headerEvaluator.CloseClick += new EventHandler(headerEvaluator_CloseClick);
            headerOutput.Activate(tabOutput);
            headerOutput.CloseClick += new EventHandler(headerOutput_CloseClick);

            DockExtender = new DockExtender(this);
            inputFloaty = DockExtender.Attach(panelInput, headerEvaluator, splitterBottom);
            outputFloaty = DockExtender.Attach(panelOutput, headerOutput, splitterRight);

            inputFloaty.Docking += new EventHandler(inputFloaty_Docking);
            outputFloaty.Docking += new EventHandler(inputFloaty_Docking);
            inputFloaty.Hide();
            outputFloaty.Hide();

            textOutput.Text = AssemblyInfo.ProductName + " v" + Application.ProductVersion + "\r\n";
            textOutput.Text += AssemblyInfo.CopyRightsDetail + "\r\n\r\n";


            marker = new TextMarker(textEditor);
            checker = new SyntaxChecker(marker); // run the syntax checker on seperate thread

            // create the syntax update checker event handler and remember its reference
            syntaxUpdateChecker = new EventHandler(checker_UpdateSyntax);
            checker.UpdateSyntax += syntaxUpdateChecker; // listen for events
            System.Threading.Thread thread = new System.Threading.Thread(checker.Start);
            thread.Start();

            TextChangedTimer = new Timer();
            TextChangedTimer.Tick += new EventHandler(TextChangedTimer_Tick);

            // assign the auto completion function to this editor
            // autocomplete form will take care of the rest
            codecomplete = new AutoComplete(textEditor);
            codecomplete.Enabled = false;
            directivecomplete = new AutoComplete(textEditor);
            directivecomplete.Enabled = false;
            directivecomplete.WordList.Items.Add("@ParseTree");
            directivecomplete.WordList.Items.Add("@Parser");
            directivecomplete.WordList.Items.Add("@Scanner");
            directivecomplete.WordList.Items.Add("@TextHighlighter");
            directivecomplete.WordList.Items.Add("@TinyPG");
            directivecomplete.WordList.Items.Add("Generate");
            directivecomplete.WordList.Items.Add("Language");
            directivecomplete.WordList.Items.Add("Namespace");
            directivecomplete.WordList.Items.Add("OutputPath");
            directivecomplete.WordList.Items.Add("TemplatePath");

            // setup the text highlighter (= text coloring)
            highlighterScanner = new TinyPG.Highlighter.Scanner();
            textHighlighter = new TinyPG.Highlighter.TextHighlighter(textEditor, highlighterScanner, new TinyPG.Highlighter.Parser(highlighterScanner));
            textHighlighter.SwitchContext += new TinyPG.Highlighter.ContextSwitchEventHandler(TextHighlighter_SwitchContext);

            LoadConfig();

            if (GrammarFile == null)
                NewGrammar();

        }
        #endregion Initialization

        #region Control events
        /// <summary>
        /// a context switch is raised when the caret of the editor moves from one section to another.
        /// the sections are defined by the highlighter parser. E.g. if the caret moves from the COMMENTBLOCK to
        /// a CODEBLOCK token, the contextswitch is raised.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void TextHighlighter_SwitchContext(object sender, TinyPG.Highlighter.ContextSwitchEventArgs e)
        {

            if (e.NewContext.Token.Type == TinyPG.Highlighter.TokenType.DOTNET_COMMENTBLOCK
                || e.NewContext.Token.Type == TinyPG.Highlighter.TokenType.DOTNET_COMMENTLINE
                || e.NewContext.Token.Type == TinyPG.Highlighter.TokenType.DOTNET_STRING
                || e.NewContext.Token.Type == TinyPG.Highlighter.TokenType.GRAMMARSTRING
                || e.NewContext.Token.Type == TinyPG.Highlighter.TokenType.DIRECTIVESTRING
                || e.NewContext.Token.Type == TinyPG.Highlighter.TokenType.GRAMMARCOMMENTBLOCK
                || e.NewContext.Token.Type == TinyPG.Highlighter.TokenType.GRAMMARCOMMENTLINE)
            {
                codecomplete.Enabled = false; // disable autocompletion if user is editing in any of these blocks
                directivecomplete.Enabled = false;
            }
            else if (e.NewContext.Parent.Token.Type == TinyPG.Highlighter.TokenType.GrammarBlock)
            {
                directivecomplete.Enabled = false;
                codecomplete.Enabled = true; //allow autocompletion
            }
            else if (e.NewContext.Parent.Token.Type == TinyPG.Highlighter.TokenType.DirectiveBlock)
            {
                codecomplete.Enabled = false;
                directivecomplete.Enabled = true; //allow directives autocompletion
            }
            else
            {
                codecomplete.Enabled = false;
                directivecomplete.Enabled = false;
            }

        }

        void checker_UpdateSyntax(object sender, EventArgs e)
        {
            if (this.InvokeRequired && !IsDisposed)
            {
                this.Invoke(new EventHandler(checker_UpdateSyntax), new object[] { sender, e });
                return;
            }
            marker.MarkWords();

            if (checker.Grammar == null) return;

            if (codecomplete.Visible)
                return;

            lock (checker.Grammar)
            {
                bool startAdded = false;
                codecomplete.WordList.Items.Clear();
                foreach (Symbol s in checker.Grammar.Symbols)
                {
                    codecomplete.WordList.Items.Add(s.Name);
                    if (s.Name == "Start")
                        startAdded = true;
                }
                if (!startAdded)
                    codecomplete.WordList.Items.Add("Start");
            }

        }

        void inputFloaty_Docking(object sender, EventArgs e)
        {
            textEditor.BringToFront();
        }
        #endregion Control events

        #region Form events
        void MainForm_Disposed(object sender, EventArgs e)
        {
            // unregister event handler.
            checker.UpdateSyntax -= syntaxUpdateChecker; // listen for events

            checker.Dispose();
            marker.Dispose();
        }

        void headerOutput_CloseClick(object sender, EventArgs e)
        {
            outputFloaty.Hide();
        }

        void headerEvaluator_CloseClick(object sender, EventArgs e)
        {
            inputFloaty.Hide();
        }

        private void menuToolsGenerate_Click(object sender, EventArgs e)
        {

            outputFloaty.Show();
            tabOutput.SelectedIndex = 0;

            CompileGrammar();

            if (compiler != null && compiler.Errors.Count == 0)
            {
                // save the grammar when compilation was successful
                SaveGrammar(GrammarFile);
            }

        }

        private void parseToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputFloaty.Show();
            outputFloaty.Show();
            if (tabOutput.SelectedIndex != 0 && tabOutput.SelectedIndex != 1)
                tabOutput.SelectedIndex = 0;

            EvaluateExpression();
        }


        private void textEditor_TextChanged(object sender, EventArgs e)
        {
            if (textHighlighter.IsHighlighting)
                return;

            marker.Clear();
            TextChangedTimer.Stop();
            TextChangedTimer.Interval = 3000;
            TextChangedTimer.Start();

            if (!IsDirty)
            {
                IsDirty = true;
                SetFormCaption();
            }

        }

        void TextChangedTimer_Tick(object sender, EventArgs e)
        {
            TextChangedTimer.Stop();

            textEditor.Invalidate();
            checker.Check(textEditor.Text);
        }

        private void newToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsDirty)
                SaveGrammarAs();

            NewGrammar();
        }


        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string newgrammarfile = OpenGrammar();
            if (newgrammarfile == null) return;

            if (IsDirty && GrammarFile != null)
            {
                DialogResult r = MessageBox.Show(this, "You will lose current changes, continue?", "Lose changes", MessageBoxButtons.OKCancel);
                if (r == DialogResult.Cancel) return;
            }

            GrammarFile = newgrammarfile;
            LoadGrammarFile();
            SaveConfig();
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(GrammarFile))
            {
                SaveGrammarAs();
            }
            else
            {
                SaveGrammar(GrammarFile);
            }
            SaveConfig();
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveGrammarAs();
            SaveConfig();
        }


        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
            Application.Exit();
        }

        private void tvParsetree_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (e.Node == null)
                return;

            IParseNode ipn = e.Node.Tag as IParseNode;
            if (ipn == null) return;

            textInput.Select(ipn.IToken.StartPos, ipn.IToken.EndPos - ipn.IToken.StartPos);
            textInput.ScrollToCaret();
        }

        private void expressionEvaluatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            inputFloaty.Show();
            textInput.Focus();
        }

        private void outputToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputFloaty.Show();
            tabOutput.SelectedIndex = 0;
        }

        private void parsetreeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputFloaty.Show();
            tabOutput.SelectedIndex = 1;
        }

        private void regexToolToolStripMenuItem_Click(object sender, EventArgs e)
        {
            outputFloaty.Show();
            tabOutput.SelectedIndex = 2;
        }

        private void tabOutput_Selected(object sender, TabControlEventArgs e)
        {
            headerOutput.Text = e.TabPage.Text;
        }

        private void textEditor_SelectionChanged(object sender, EventArgs e)
        {
            SetStatusbar();
        }

        private void textInput_SelectionChanged(object sender, EventArgs e)
        {
            SetStatusbar();
        }

        private void textInput_Enter(object sender, EventArgs e)
        {
            SetStatusbar();
        }

        private void textInput_Leave(object sender, EventArgs e)
        {
            SetStatusbar();
        }

        private void textEditor_Enter(object sender, EventArgs e)
        {
            SetStatusbar();
        }

        private void textEditor_Leave(object sender, EventArgs e)
        {
            SetStatusbar();
        }

        private void aboutTinyParserGeneratorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutTinyPG();
        }

        private void viewParserToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewFile("Parser");
        }

        private void viewScannerToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewFile("Scanner");
        }

        private void viewParseTreeCodeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            ViewFile("ParseTree");
        }

        private void expressionEvaluatorToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            NotepadViewFile(AppDomain.CurrentDomain.BaseDirectory + @"Examples\simple expression1.tpg");
        }

        private void codeblocksToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotepadViewFile(AppDomain.CurrentDomain.BaseDirectory + @"Examples\simple expression2.tpg");
        }

        private void theTinyPGGrammarToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotepadViewFile(AppDomain.CurrentDomain.BaseDirectory + @"Examples\BNFGrammar v1.1.tpg");
        }

        private void theTinyPGGrammarV10ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotepadViewFile(AppDomain.CurrentDomain.BaseDirectory + @"Examples\BNFGrammar v1.0.tpg");
        }

        private void theTinyPGGrammarHighlighterV12ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            NotepadViewFile(AppDomain.CurrentDomain.BaseDirectory + @"Examples\GrammarHighlighter.tpg");
        }

        private void textOutput_LinkClicked(object sender, LinkClickedEventArgs e)
        {
            try
            {
                if (e.LinkText == "www.codeproject.com")
                {
                    System.Diagnostics.Process.Start("http://www.codeproject.com/script/Articles/MemberArticles.aspx?amid=2192187");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        #endregion Form events

        #region Processing functions

        private static void NotepadViewFile(string filename)
        {
            try
            {
                System.Diagnostics.Process.Start("Notepad.exe", filename);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private void ViewFile(string filetype)
        {
            try
            {
                if (IsDirty || compiler == null || !compiler.IsCompiled)
                    CompileGrammar();

                if (grammar == null)
                    return;

                ICodeGenerator generator = CodeGeneratorFactory.CreateGenerator(filetype, grammar.Directives["TinyPG"]["Language"]);
                string folder = grammar.GetOutputPath() + generator.FileName;
                System.Diagnostics.Process.Start(folder);
            }
            catch (Exception)
            {
            }
        }

        private void EvaluateExpression()
        {
            textOutput.Text = "Parsing expression...\r\n";
            try
            {

                if (IsDirty || compiler == null || !compiler.IsCompiled)
                    CompileGrammar();

                if (string.IsNullOrEmpty(GrammarFile))
                    return;

                // save the grammar when compilation was successful
                if (compiler != null && compiler.Errors.Count == 0)
                    SaveGrammar(GrammarFile);

                CompilerResult result = new CompilerResult();
                if (compiler.IsCompiled)
                {
                    result = compiler.Run(textInput.Text, textInput);

                    //textOutput.Text = result.ParseTree.PrintTree();
                    textOutput.Text += result.Output;
                    ParseTreeViewer.Populate(tvParsetree, result.ParseTree);
                }
            }
            catch (Exception exc)
            {
                textOutput.Text += "An exception occured compiling the assembly: \r\n" + exc.Message + "\r\n" + exc.StackTrace;
            }

        }

        /// <summary>
        /// this is where some of the magic happens
        /// to highlight specific C# code or VB code, the language specific keywords are swapped
        /// that is, the DOTNET regexps are overwritten by either the c# or VB regexps
        /// </summary>
        /// <param name="language"></param>
        private void SetHighlighterLanguage(string language)
        {
            lock (Highlighter.TextHighlighter.treelock)
            {
                switch (CodeGeneratorFactory.GetSupportedLanguage(language))
                {
                    case SupportedLanguage.VBNet:
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_STRING] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.VB_STRING];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_SYMBOL] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.VB_SYMBOL];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_COMMENTBLOCK] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.VB_COMMENTBLOCK];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_COMMENTLINE] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.VB_COMMENTLINE];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_KEYWORD] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.VB_KEYWORD];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_NONKEYWORD] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.VB_NONKEYWORD];
                        break;
                    default:
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_STRING] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.CS_STRING];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_SYMBOL] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.CS_SYMBOL];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_COMMENTBLOCK] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.CS_COMMENTBLOCK];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_COMMENTLINE] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.CS_COMMENTLINE];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_KEYWORD] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.CS_KEYWORD];
                        highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.DOTNET_NONKEYWORD] = highlighterScanner.Patterns[TinyPG.Highlighter.TokenType.CS_NONKEYWORD];
                        break;
                }
                textHighlighter.HighlightText();
            }

        }

        private void ManageParseError(ParseTree tree, StringBuilder output)
        {
            foreach (ParseError error in tree.Errors)
                output.AppendLine(string.Format("({0},{1}): {2}", error.Line, error.Column, error.Message));

            output.AppendLine("Semantic errors in grammar found.");
            textEditor.Select(tree.Errors[0].Position, tree.Errors[0].Length > 0 ? tree.Errors[0].Length : 1);
        }

        private void CompileGrammar()
        {

            if (string.IsNullOrEmpty(GrammarFile))
                SaveGrammarAs();

            if (string.IsNullOrEmpty(GrammarFile))
                return;

            compiler = new TinyPG.Compiler.Compiler();
            StringBuilder output = new StringBuilder();

            // clear tree
            tvParsetree.Nodes.Clear();

            Program prog = new Program(ManageParseError, output);
            DateTime starttimer = DateTime.Now;
            grammar = prog.ParseGrammar(textEditor.Text, GrammarFile);

            if (grammar != null)
            {
                SetHighlighterLanguage(grammar.Directives["TinyPG"]["Language"]);

                if (prog.BuildCode(grammar, compiler))
                {
                    TimeSpan span = DateTime.Now.Subtract(starttimer);
                    output.AppendLine("Compilation successful in " + span.TotalMilliseconds + "ms.");
                }
            }

            textOutput.Text = output.ToString();
            textOutput.Select(textOutput.Text.Length, 0);
            textOutput.ScrollToCaret();

        }

        private void AboutTinyPG()
        {
            StringBuilder about = new StringBuilder();

            //http://www.codeproject.com/script/Articles/MemberArticles.aspx?amid=2192187

            about.AppendLine(AssemblyInfo.ProductName + " v" + Application.ProductVersion);
            about.AppendLine(AssemblyInfo.CopyRightsDetail);
            about.AppendLine();
            about.AppendLine("For more information about the author");
            about.AppendLine("or TinyPG visit www.codeproject.com");

            outputFloaty.Show();
            tabOutput.SelectedIndex = 0;
            textOutput.Text = about.ToString();

        }

        private void SetFormCaption()
        {
            this.Text = "@TinyPG - a Tiny Parser Generator .Net";
            if ((GrammarFile == null) || (!File.Exists(GrammarFile)))
            {
                if (IsDirty) this.Text += " *";
                return;
            }

            string name = new FileInfo(GrammarFile).Name;
            this.Text += " [" + name + "]";
            if (IsDirty) this.Text += " *";
        }

        private void NewGrammar()
        {
            GrammarFile = null;
            IsDirty = false;

            string text = "//" + AssemblyInfo.ProductName + " v" + Application.ProductVersion + "\r\n";
            text += "//" + AssemblyInfo.CopyRightsDetail + "\r\n\r\n";
            textEditor.Text = text;
            textEditor.ClearUndo();

            textOutput.Text = AssemblyInfo.ProductName + " v" + Application.ProductVersion + "\r\n";
            textOutput.Text += AssemblyInfo.CopyRightsDetail + "\r\n\r\n";

            SetFormCaption();
            SaveConfig();

            textEditor.Select(textEditor.Text.Length, 0);

            IsDirty = false;
            textHighlighter.ClearUndo();
            SetFormCaption();
            SetStatusbar();

        }
        private void LoadGrammarFile()
        {
            if (GrammarFile == null) return;
            if (!File.Exists(GrammarFile))
            {
                GrammarFile = null; // file does not exist anymore
                return;
            }

            string folder = new FileInfo(GrammarFile).DirectoryName;
            Directory.SetCurrentDirectory(folder);

            textEditor.Text = File.ReadAllText(GrammarFile);
            textEditor.ClearUndo();
            CompileGrammar();
            textOutput.Text = "";
            textEditor.Focus();
            SetStatusbar();
            textHighlighter.ClearUndo();
            IsDirty = false;
            SetFormCaption();
            textEditor.Select(0, 0);
            checker.Check(textEditor.Text);


        }

        private void SaveGrammarAs()
        {
            DialogResult r = saveFileDialog.ShowDialog(this);
            if (r == DialogResult.OK)
            {
                SaveGrammar(saveFileDialog.FileName);
            }

        }

        private string OpenGrammar()
        {
            DialogResult r = openFileDialog.ShowDialog(this);
            if (r == DialogResult.OK)
                return openFileDialog.FileName;
            else
                return null;
        }

        private void SaveGrammar(string filename)
        {
            if (String.IsNullOrEmpty(filename)) return;

            GrammarFile = filename;

            string folder = new FileInfo(GrammarFile).DirectoryName;
            Directory.SetCurrentDirectory(folder);

            string text = textEditor.Text.Replace("\n", "\r\n");
            File.WriteAllText(filename, text);
            IsDirty = false;
            SetFormCaption();
        }

        private void LoadConfig()
        {
            try
            {
                string configfile = AppDomain.CurrentDomain.BaseDirectory + "TinyPG.config";

                if (!File.Exists(configfile))
                    return;

                XmlDocument doc = new XmlDocument();
                doc.Load(configfile);
                openFileDialog.InitialDirectory = doc["AppSettings"]["OpenFilePath"].Attributes[0].Value;
                saveFileDialog.InitialDirectory = doc["AppSettings"]["SaveFilePath"].Attributes[0].Value;
                GrammarFile = doc["AppSettings"]["GrammarFile"].Attributes[0].Value;

                if (string.IsNullOrEmpty(GrammarFile))
                    NewGrammar();
                else
                    LoadGrammarFile();
            }
            catch (Exception)
            {
            }
        }

        private void SaveConfig()
        {
            string configfile = AppDomain.CurrentDomain.BaseDirectory + "TinyPG.config";
            XmlAttribute attr;
            XmlDocument doc = new XmlDocument();
            XmlNode settings = doc.CreateElement("AppSettings", "TinyPG");
            doc.AppendChild(settings);

            XmlNode node = doc.CreateElement("OpenFilePath", "TinyPG");
            settings.AppendChild(node);
            node = doc.CreateElement("SaveFilePath", "TinyPG");
            settings.AppendChild(node);
            node = doc.CreateElement("GrammarFile", "TinyPG");
            settings.AppendChild(node);

            attr = doc.CreateAttribute("Value");
            settings["OpenFilePath"].Attributes.Append(attr);
            if (File.Exists(openFileDialog.FileName))
                attr.Value = new FileInfo(openFileDialog.FileName).Directory.FullName;

            attr = doc.CreateAttribute("Value");
            settings["SaveFilePath"].Attributes.Append(attr);
            if (File.Exists(saveFileDialog.FileName))
                attr.Value = new FileInfo(saveFileDialog.FileName).Directory.FullName;

            attr = doc.CreateAttribute("Value");
            attr.Value = GrammarFile;
            settings["GrammarFile"].Attributes.Append(attr);

            doc.Save(configfile);
        }

        private void SetStatusbar()
        {
            if (textEditor.Focused)
            {
                int pos = textEditor.SelectionStart;
                statusPos.Text = pos.ToString(CultureInfo.InvariantCulture);
                statusCol.Text = (pos - textEditor.GetFirstCharIndexOfCurrentLine() + 1).ToString(CultureInfo.InvariantCulture);
                statusLine.Text = (textEditor.GetLineFromCharIndex(pos) + 1).ToString(CultureInfo.InvariantCulture);

            }
            else if (textInput.Focused)
            {
                int pos = textInput.SelectionStart;
                statusPos.Text = pos.ToString(CultureInfo.InvariantCulture);
                statusCol.Text = (pos - textInput.GetFirstCharIndexOfCurrentLine() + 1).ToString(CultureInfo.InvariantCulture);
                statusLine.Text = (textInput.GetLineFromCharIndex(pos) + 1).ToString(CultureInfo.InvariantCulture);
            }
            else
            {
                statusPos.Text = "-";
                statusCol.Text = "-";
                statusLine.Text = "-";
            }
        }
        #endregion

    }
}
