using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Threading;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace WindowsFormsApp1
{
    public partial class Form1 : Form
    {
        public byte[] a = new byte[13];
        private SerialPort com = new SerialPort();
        public USB usb;
        private Thread thread;

        //union
        [StructLayout(LayoutKind.Explicit, Size = 4)]
        public struct Union
        {
            [FieldOffset(0)]
            public Byte b0;
            [FieldOffset(1)]
            public Byte b1;
            [FieldOffset(2)]
            public Byte b2;
            [FieldOffset(3)]
            public Byte b3;
            [FieldOffset(0)]
            public Int32 i;
            [FieldOffset(0)]
            public Single f;
        }

        public Form1()
        {
            InitializeComponent();
            usb = new USB();
            Init();
        }

        //initial serial port
        private void Init()
        {
            try
            {
                List<string> list = new List<string>();
                string[] ports = USB.GetPorts(); //SerialPort.GetPortNames();//

                PortcomboBox.Items.Clear();
                for (int i = 0; i < ports.Length; i++)
                {
                    PortcomboBox.Items.Add(ports[i]);
                }
                if (ports.Length > 0)
                {
                    PortcomboBox.SelectedIndex = ports.Length - 1;
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        private void PortSearch_Click(object sender, EventArgs e)
        {
            Init();
        }

        //force quit form 
        protected override void WndProc(ref Message msg)
        {
            const int WM_SYSCOMMAND = 0x0112;
            const int SC_CLOSE = 0xF060;

            if (msg.Msg == WM_SYSCOMMAND && ((int)msg.WParam == SC_CLOSE))
            {
                // 点击winform右上关闭按钮 
                usb.CloseCom();
                isExit = true;
            }
            base.WndProc(ref msg);
        }

        //open event 
        private Boolean isOpen = false;
        private void PortOpen_Click(object sender, EventArgs e)
        {
            if (!isOpen)
            {
                PortOpen.Text = "Close";
                String comName = PortcomboBox.SelectedItem.ToString();
                Boolean isOpenSuccess = usb.SetCom(PortcomboBox.SelectedItem.ToString());
                if (isOpenSuccess)
                {
                    isOpen = true;
                }
                else
                {
                    isOpen = false;
                    PortOpen.Text = "Open";
                }
            }
            else
            {
                usb.CloseCom();
                PortOpen.Text = "Open";
                isOpen = false;
            }
        }
        Boolean isExit = false;


        //read event
        private void SendCommand_Click(object sender, EventArgs e)
        {
            //AA 01 00 00 00 00 00 00 00 00 00 00 00
            a[0] = 0xaa;
            a[1] = 0x01;
            a[2] = 0x00;
            a[3] = 0x00;
            a[4] = 0x00;
            a[5] = 0x00;
            a[6] = 0x00;
            a[7] = 0x00;
            a[8] = 0x00;
            a[9] = 0x00;
            a[10] = 0x00;
            a[11] = 0x00;
            a[12] = 0x00;
            usb.SendData(a);
            Thread.Sleep(100);
          //  String msg = "";
            thread = new Thread(new ThreadStart(GetData));
            thread.Start();
        }

        //GetData
        public void GetData()
        {              
            String msg = "";

            Thread.Sleep(500);
       //    if (usb.ReadData().Contains("set ok"))
    //            isExit = false;
      //      else return;
            int i = 0;
          //  while (!isExit)
       //     {
                msg = usb.ReadData();
                //if (msg.Contains("dBm"))
                //{
                //    if (!msg.Contains("=")) continue;
                    Action<int> action = (data) =>
                   {

                       //        int start = msg.IndexOf("=");
                       //        int length = "-00.0".Length;
                       //        String tmpValue = msg.Substring(start + 2, length).Trim();
                       //        float value = float.Parse(tmpValue);
                       //        if (value > -18 || value < -22)
                       //        {
                       //            valueLable.ForeColor = Color.Red;
                       //        }
                       //        else
                       //        {
                       //            valueLable.ForeColor = Color.Black;


                       //字符串转16进制字节
                       string   hexString = msg.Replace(" ", "");
                       if ((hexString.Length % 2) != 0)
                           hexString += " ";
                       byte[] returnBytes = new byte[hexString.Length / 2];
                       for (int ti = 0; ti < returnBytes.Length; ti++)
                           returnBytes[ti] = Convert.ToByte(hexString.Substring(ti * 2, 2), 16);
                  //     return returnBytes;
                    
                       //// union
                       Union u = new Union();
                       u.b0 = returnBytes[5];  //Tx_Buf[5  6  7  8 ]
                       u.b1 = returnBytes[6];
                       u.b2 = returnBytes[7];
                       u.b3 = returnBytes[8];

                       Union voltage  = new Union();
                       voltage.b0 = returnBytes[9];  //Tx_Buf[5  6  7  8 ]
                       voltage.b1 = returnBytes[10];
                       voltage.b2 = 0;
                       voltage.b3 = 0;

                       string msg1 = Double.Parse(Convert.ToString(10 * Math.Log10(u.f))).ToString("F2");   //dBm
                       richTextBox1_valnue.Text += msg1 + " dBm" + "\n";
                       richTextBox2_valnue.Text += msg1 + " dBm     "  + Convert.ToString(u.f) + " mW    " + Convert.ToString(voltage.i) +"    " + returnBytes[12] + "\n";       //mW;

                   };
                   Invoke(action, i);
                    i++;
                  //   }
                     Thread.Sleep(1000);
        //    }
        }

            //exit interface
            private void StopSend_Click(object sender, EventArgs e)
        {
    
            System.Environment.Exit(0);
        }

        //save as
        SaveFileDialog saveFileDialog1 = new SaveFileDialog();
        private void DataSave_Click(object sender, EventArgs e)
        {
            if (this.richTextBox1_valnue.Text == "")
                return;
            saveFileDialog1.DefaultExt = "txt";
            saveFileDialog1.Filter = "Text files (*.txt)|*.txt|All files (*.*)|*.*";
            //if (this.saveFileDialog1.ShowDialog() == DialogResult.Cancel)
            //    return;
            string FileName = saveFileDialog1.FileName;

            if (saveFileDialog1.ShowDialog() == DialogResult.OK && FileName.Length > 0)
            {
                // Save the contents of the RichTextBox into the file.
                richTextBox1_valnue.SaveFile(saveFileDialog1.FileName, RichTextBoxStreamType.PlainText);
                MessageBox.Show("Saved");
            }
        }

        //save
        private void dBm_valnue_save_Click(object sender, EventArgs e)
        {
            string FileName = DateTime.Now.ToString("yyyyMMddhhmmss") + ".txt";
            richTextBox1_valnue.SaveFile(FileName, RichTextBoxStreamType.PlainText);
        }
        
        //cover dBm to W
        private void Btn_cover_Click(object sender, EventArgs e)
        {
            //CoverdBm_textBox1        CovermW_textBox2
            //  string msg1 = Double.Parse(Convert.ToString(10 * Math.Log10(u.f))).ToString("F2");   //dBm   -61.95 dBm     6.381912E-07 mW    45    6

            CovermW_textBox2.Text = Convert.ToString(Math.Pow(10,(Convert.ToDouble( CoverdBm_textBox1.Text) )/10 )*(Math.Pow(10,-3) ) );   //dBm  >> W


        }
    }
}
