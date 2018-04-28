using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using TinyPG.Compiler;

namespace TinyPG.UnitTests
{
	/// <summary>
	/// This class provides support for testing Program.cs.
	/// It covers public Methods and Main()
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

		/// <summary>
		/// Test command line parameter, when file doesn't exist.
		/// </summary>
		[TestMethod]
		public void TestFileNOK()
		{
			Assert.AreEqual((int)Program.ExitCode.InvalidFilename, Program.Main(new string[] { "foo" }));
		}

		/// <summary>
		/// Testing grammar parsing when grammar is OK.
		/// </summary>
		[TestMethod]
		public void TestParseGrammar()
		{
			StringBuilder input = new StringBuilder(string.Empty)
				 .Append(@"<% @TinyPG %>")
				 .Append("ALL -> @\".*\";")
				 .Append("Start -> ALL;");

			Program prog = new Program(FailError, new StringBuilder(string.Empty));
			Grammar grammar = prog.ParseGrammar(input.ToString(), "");
			Assert.IsNotNull(grammar);
		}

		/// <summary>
		/// Testing grammar parsing when grammar is NOT OK.
		/// </summary>
		[TestMethod]
		public void TestParseGrammarNOK()
		{
			StringBuilder input = new StringBuilder(string.Empty)
				 .Append(@"<% @TinyPG %>")
				 .Append("ALL -> @\".*Start -> ALL;");

			Program prog = new Program(PassError, new StringBuilder(string.Empty));
			Grammar grammar = prog.ParseGrammar(input.ToString(), "");
			Assert.IsNull(grammar);
		}

		private void FailError(ParseTree tree, StringBuilder output) { Assert.Fail(); }
		private void PassError(ParseTree tree, StringBuilder output)
		{
			Assert.IsTrue(tree.Errors.Count > 0);

		}
	}
}
