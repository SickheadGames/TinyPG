using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyPG.Compiler;

namespace TinyPG.UnitTests
{
    /// <summary>
    /// Summary description for UnitTest1
    /// </summary>
    [TestClass]
    public class ParserTester
    {
        // TODO: set the correct paths to be able to run the unittests succesfully
        private const string TEMPLATEPATH = @"D:\MyProjects\Net\TinyPG v1.3\TinyPG\Templates\C#\";
        private const string TEMPLATEPATH_VB = @"D:\MyProjects\Net\TinyPG v1.3\TinyPG\Templates\VB\";
        private const string OUTPUTPATH = @"D:\MyProjects\Net\TinyPG v1.3\TinyPG.UnitTests\";
        private const string TESTFILESPATH = @"D:\MyProjects\Net\TinyPG v1.3\TinyPG.UnitTests\Testfiles\";

        public ParserTester()
        {
            //
            // TODO: Add constructor logic here
            //
        }

        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        //
        // You can use the following additional attributes as you write your tests:
        //
        // Use ClassInitialize to run code before running the first test in the class
        // [ClassInitialize()]
        // public static void MyClassInitialize(TestContext testContext) { }
        //
        // Use ClassCleanup to run code after all tests in a class have run
        // [ClassCleanup()]
        // public static void MyClassCleanup() { }
        //
        // Use TestInitialize to run code before running each test 
        // [TestInitialize()]
        // public void MyTestInitialize() { }
        //
        // Use TestCleanup to run code after each test has run
        // [TestCleanup()]
        // public void MyTestCleanup() { }
        //
        #endregion


        private GrammarTree LoadGrammar(string filename)
        {
            string grammarfile = System.IO.File.ReadAllText(filename);
            Scanner scanner = new Scanner();
            Parser parser = new Parser(scanner);
            GrammarTree tree = (GrammarTree)parser.Parse(grammarfile, new GrammarTree());
            return tree;
        }

        [TestMethod]
        public void SimpleExpression1_Test()
        {
            
            GrammarTree GT = LoadGrammar(TESTFILESPATH + @"simple expression1.tpg");
            Grammar G = (Grammar) GT.Eval();


            G.Directives["TinyPG"]["TemplatePath"] = TEMPLATEPATH;
            G.Directives["TinyPG"]["OutputPath"] = OUTPUTPATH;

            // basic checks
            string temp = G.PrintFirsts();
            Assert.IsTrue(!String.IsNullOrEmpty(temp));
            temp = G.GetOutputPath();
            Assert.IsTrue(!String.IsNullOrEmpty(temp));
            temp = G.PrintGrammar();
            Assert.IsTrue(!String.IsNullOrEmpty(temp));

            Compiler.Compiler compiler = new Compiler.Compiler();
            
            compiler.Compile(G);

            Assert.IsTrue(compiler.Errors.Count == 0, "compilation contains errors");

            CompilerResult result = compiler.Run("5+7/3*2+(4*2)");

            

            Assert.IsTrue(result.Output.StartsWith("Parse was successful."));
        }

        [TestMethod]
        public void SimpleExpression1_VB_Test()
        {
            GrammarTree GT = LoadGrammar(TESTFILESPATH + @"simple expression1_vb.tpg");
            Grammar G = (Grammar)GT.Eval();
            G.Directives["TinyPG"]["TemplatePath"] = TEMPLATEPATH_VB;

            Compiler.Compiler compiler = new Compiler.Compiler();

            compiler.Compile(G);
            Assert.IsTrue(compiler.Errors.Count == 0, "compilation contains errors");

            CompilerResult result = compiler.Run("5+7/3*2+(4*2)");

            Assert.IsTrue(result.Output.StartsWith("Parse was successful."));
        }

        [TestMethod]
        public void SimpleExpression2_Test()
        {
            GrammarTree GT = LoadGrammar(TESTFILESPATH + @"simple expression2.tpg");
            Grammar G = (Grammar)GT.Eval();
            G.Directives.Add(new Directive("TinyPG"));
            G.Directives["TinyPG"]["TemplatePath"] = TEMPLATEPATH;

            Compiler.Compiler compiler = new Compiler.Compiler();

            compiler.Compile(G);
            Assert.IsTrue(compiler.Errors.Count == 0, "compilation contains errors");

            CompilerResult result = compiler.Run("5+8/4*2+(4*2)");

            Assert.IsTrue(Convert.ToInt32(result.Value) == 17);
        }

        [TestMethod]
        public void SimpleExpression2_VB_Test()
        {
            GrammarTree GT = LoadGrammar(TESTFILESPATH + @"simple expression2_vb.tpg");
            Grammar G = (Grammar)GT.Eval();
            G.Directives.Add(new Directive("TinyPG"));
            G.Directives["TinyPG"]["TemplatePath"] = TEMPLATEPATH_VB;

            Compiler.Compiler compiler = new Compiler.Compiler();

            compiler.Compile(G);
            Assert.IsTrue(compiler.Errors.Count == 0, "compilation contains errors");

            CompilerResult result = compiler.Run("5+8/4*2+(4*2)");

            Assert.IsTrue(Convert.ToInt32(result.Value) == 17);
        }

        [TestMethod]
        public void SimpleExpression3_Test()
        {
            GrammarTree GT = LoadGrammar(TESTFILESPATH + @"BNFGrammar v1.1.tpg");
            Grammar G = (Grammar)GT.Eval();
            G.Directives.Add(new Directive("TinyPG"));
            G.Directives["TinyPG"]["TemplatePath"] = TEMPLATEPATH;

            Compiler.Compiler compiler = new Compiler.Compiler();

            compiler.Compile(G);
            Assert.IsTrue(compiler.Errors.Count == 0, "compilation contains errors");

            CompilerResult result = compiler.Run("");

            Assert.IsTrue(result.Output.StartsWith("Parse was successful."));
        }

        [TestMethod]
        public void SimpleExpression3_VB_Test()
        {
            GrammarTree GT = LoadGrammar(TESTFILESPATH + @"BNFGrammar_vb v1.1.tpg");
            Grammar G = (Grammar)GT.Eval();
            G.Directives.Add(new Directive("TinyPG"));
            G.Directives["TinyPG"]["TemplatePath"] = TEMPLATEPATH_VB;

            Compiler.Compiler compiler = new Compiler.Compiler();

            compiler.Compile(G);
            Assert.IsTrue(compiler.Errors.Count == 0, "compilation contains errors");

            CompilerResult result = compiler.Run("");

            Assert.IsTrue(result.Output.StartsWith("Parse was successful."));
        }

        [TestMethod]
        public void SimpleExpression4_Test()
        {
            GrammarTree GT = LoadGrammar(TESTFILESPATH + @"GrammarHighlighter.tpg");
            Grammar G = (Grammar)GT.Eval();
            G.Directives.Add(new Directive("TinyPG"));
            G.Directives["TinyPG"]["TemplatePath"] = TEMPLATEPATH;

            Compiler.Compiler compiler = new Compiler.Compiler();

            compiler.Compile(G);
            Assert.IsTrue(compiler.Errors.Count == 0, "compilation contains errors");

            CompilerResult result = compiler.Run("using System.IO;\r\n");

            Assert.IsTrue(result.Output.StartsWith("Parse was successful."));
        }

        [TestMethod]
        public void SimpleExpression4_VB_Test()
        {
            GrammarTree GT = LoadGrammar(TESTFILESPATH + @"GrammarHighlighter_vb.tpg");
            Grammar G = (Grammar)GT.Eval();
            G.Directives.Add(new Directive("TinyPG"));
            G.Directives["TinyPG"]["TemplatePath"] = TEMPLATEPATH_VB;

            Compiler.Compiler compiler = new Compiler.Compiler();

            compiler.Compile(G);
            Assert.IsTrue(compiler.Errors.Count == 0, "compilation contains errors");

            CompilerResult result = compiler.Run("using System.IO;\r\n");

            Assert.IsTrue(result.Output.StartsWith("Parse was successful."));
        }
    }
}
