using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace TinyPG
{
    public partial class FindDialog : Form
    {
        public string SearchTerm { get; set; }
        public FindDialog()
        {
            InitializeComponent();
        }

        private void FindButton_Click(object sender, EventArgs e)
        {
            SearchTerm = this.txtSearch.Text;
            this.Close();
        }

        private void CancelButton_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
