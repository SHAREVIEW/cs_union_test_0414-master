using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using wf_SwitchUnit_20190531.About;

namespace wf_SwitchUnit_20190531
{

    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        #region     dBm  < >  W  < >  dBw
        private void btn_dBm_Click(object sender, EventArgs e)
        {
            tBx_W.Text = Convert.ToString(Math.Pow(10,(Convert.ToDouble(tBx_dBm.Text) )/10 )*(Math.Pow(10,-3) ) );   //dBm  >> W
            Double W = Math.Pow(10, (Convert.ToDouble(tBx_dBm.Text)) / 10) * (Math.Pow(10, -3));
            tBx_dBw.Text = Double.Parse(Convert.ToString(10 * Math.Log10( W ))).ToString("F3");   //   //dBm  >> dBw; 
            expressions.Add(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "    W: " + tBx_W.Text + "    dBw: " + tBx_dBw.Text + "  dBm: " + tBx_dBm.Text);
            writeLog();
        }

        private void btn_W_Click(object sender, EventArgs e)
        {
            tBx_dBm.Text = Double.Parse(Convert.ToString(10 * Math.Log10((Convert.ToDouble(tBx_W.Text)) / (Math.Pow(10, -3))))).ToString("F3");   //W >> dBm
            tBx_dBw.Text = Double.Parse(Convert.ToString(10 * Math.Log10(Convert.ToDouble(tBx_W.Text)))).ToString("F3");   //W >> dBw
            expressions.Add(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "    W: " + tBx_W.Text + "    dBw: " + tBx_dBw.Text + "    dBm: " + tBx_dBm.Text);
            writeLog();
        }

        private void btn_dBw_Click(object sender, EventArgs e)
        {
            tBx_W.Text = Convert.ToString(Math.Pow(10, (Convert.ToDouble(tBx_dBw.Text)) / 10) );   //dBw  >> W
            Double W = Math.Pow(10, (Convert.ToDouble(tBx_dBw.Text)) / 10);
            tBx_dBm.Text = Double.Parse(Convert.ToString(10 * Math.Log10(( W ) / (Math.Pow(10, -3))))).ToString("F3");    // dBw >> dBm
            expressions.Add(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "    W: " + tBx_W.Text + "    dBw: " + tBx_dBw.Text + "    dBm: " + tBx_dBm.Text);
            writeLog();
        }

        #endregion



        #region   ToolStripMenuItem

        //About
        private ABOUTFORM AboutForm = new ABOUTFORM();
        private void aboutToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AboutForm.ShowInTaskbar = false;
            AboutForm.ShowDialog();
        }

        //Help
        private void helpViewToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName = System.Environment.CurrentDirectory + "\\SmallTool_English.chm";
                p.Start();
            }
            catch
            {
                MessageBox.Show("未找到 SmallTool_English.chm");
            }
        }
        //calculator
        private void calculatorToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                System.Diagnostics.Process p = new System.Diagnostics.Process();
                p.StartInfo.FileName ="calc.exe";
                p.Start();
            }
            catch
            {
                MessageBox.Show("未找到 SmallTool_English.chm");
            }
        }
        //exit
        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        //Open log
        private void openFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                string LogPath = System.Environment.CurrentDirectory + "//log.txt";
                System.Diagnostics.Process.Start(LogPath);
            }
            catch
            {
                MessageBox.Show("未找到 log.txt");
            }
        }
        #endregion

        #region   log
        List<string> expressions = new List<string>();  //save config
        private void writeLog() {

            ////save info log
            //StreamWriter file = new StreamWriter("log.txt", true);
            //file.WriteLine(DateTime.Now.ToString("yyyy-MM-dd-HH-mm-ss-fff") + "        " + expressions);
            //file.Close();
            try
            {
                FileStream SaveFile = new FileStream(System.Environment.CurrentDirectory + "\\log.txt", FileMode.Append);
                StreamWriter streamWriter = new StreamWriter(SaveFile);                // StreamWriter streamWriter = new StreamWriter(history.txt,true);
                foreach (string a in expressions)
                {
                    streamWriter.WriteLine(a);
                }
                streamWriter.Close();
            }
            catch (IOException ex)
            {
                MessageBox.Show("An IO exception has been thrown!");
                MessageBox.Show(ex.ToString());
            }
        }
        #endregion


    }
}
