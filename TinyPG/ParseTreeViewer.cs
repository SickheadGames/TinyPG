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
using System.Windows.Forms;
using System.Drawing;
using TinyPG.Debug;

namespace TinyPG
{
	/// <summary>
	/// this class helps populate the treeview given a parsetree
	/// </summary>
	public sealed class ParseTreeViewer
	{
		private ParseTreeViewer()
		{
		}

		public static void Populate(TreeView treeview, IParseTree parsetree)
		{
			treeview.Visible = false;
			treeview.SuspendLayout();
			treeview.Nodes.Clear();
			treeview.Tag = parsetree;

			IParseNode start = parsetree.INodes[0];
			TreeNode node = new TreeNode(start.Text);
			node.Tag = start;
			node.ForeColor = Color.SteelBlue;
			treeview.Nodes.Add(node);

			PopulateNode(node, start);
			treeview.ExpandAll();
			treeview.ResumeLayout();
			treeview.Visible = true;
		}

		private static void PopulateNode(TreeNode node, IParseNode start)
		{
			foreach (IParseNode ipn in start.INodes)
			{
				TreeNode tn = new TreeNode(ipn.Text);
				tn.Tag = ipn;
				node.Nodes.Add(tn);
				PopulateNode(tn, ipn);
			}
		}

	}
}
