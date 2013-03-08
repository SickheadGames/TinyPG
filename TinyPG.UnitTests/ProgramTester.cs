using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyPG.Compiler;

namespace TinyPG.UnitTests
{
    /// <summary>
    /// Summary description for ProgramTester
    /// </summary>
    [TestClass]
    public class ProgramTester
    {
        public ProgramTester()
        {

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

        [TestMethod]
        public void TestFileNOK()
        {
            Assert.AreEqual((int)Program.ExitCode.InvalidFilename, Program.Main(new string[] { "foo" }));
        }

        [TestMethod]
        public void TestParseGrammar()
        {
           StringBuilder input =  new StringBuilder(string.Empty)
                .Append(@"<% @TinyPG %>")
                .Append(@"Start -> .*;");

            Program prog = new Program(ManageError, new StringBuilder(string.Empty));
            Grammar grammar = prog.ParseGrammar(input.ToString(), "");
            Assert.IsNotNull(grammar);
        }

        private void ManageError(ParseTree tree, StringBuilder output) { }
    }
}
