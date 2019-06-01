using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace wf_SwitchUnit_20190531.About
{
    public partial class ABOUTFORM : Form
    {
        public ABOUTFORM()
        {
            InitializeComponent();
        }

        private void linkLabel1_LinkClicked(object sender, LinkLabelLinkClickedEventArgs e)
        {
            linkLabel1.LinkVisited = true;
            Process.Start("IExplore", "www.grandway.com.cn");
        }

        private void button1_Click(object sender, EventArgs e)
        {
            this.Close();
        }
    }
}
